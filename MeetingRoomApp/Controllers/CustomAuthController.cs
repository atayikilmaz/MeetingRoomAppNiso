using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;


namespace MeetingRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, 
            IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User { UserName = model.Email, Email = model.Email, Name = model.Name };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate 2FA token
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                // Send 2FA token via email
                await _emailService.SendEmailAsync(user.Email, "2FA Verification", 
                    $"Your 2FA code is: {token}");

                return Ok(new { message = "User registered successfully. Please check your email for 2FA code." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
        
        

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactorAuth([FromBody] TwoFactorAuthModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid email");

            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Token);
            if (result)
            {
                // Add user to the "User" role
                await _userManager.AddToRoleAsync(user, "User");

                // Set EmailConfirmed to true
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: true);
                string jwtToken = await GenerateJwtToken(user);
                return Ok(new { Token = jwtToken, Message = "2FA verification successful. User role assigned." });
            }

            return BadRequest("Invalid 2FA token");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }

            if (!user.EmailConfirmed)
            {
                return StatusCode(403, new { Message = "Please verify your account with 2FA" });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                string token = await GenerateJwtToken(user);
                return Ok(new { Token = token, Message = "Login successful" });
            }

            if (result.IsLockedOut)
                return StatusCode(423, new { Message = "Account is locked out" });
            if (result.IsNotAllowed)
                return StatusCode(403, new { Message = "Account is not allowed to sign in" });
    
            return Unauthorized(new { Message = "Invalid login attempt" });
        }
        
        [Authorize]
        [HttpGet ("getCurrentUserRole")]
        public async Task<IActionResult> GetCurrentUserRole()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
       
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully" });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return Ok(new { Email = email });
        }
 
        
       
            [HttpGet]
            [Route("google-login")]
            public Task LoginGoogle()
            {
                return HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("SignInGoogle")
                });
            }

            
            
            
            
            [HttpGet]
            [Route("signin")]
            public async Task<IActionResult> SignInGoogle()
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result.Succeeded)
                {
                    var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = email,
                            Email = email,
                            Name = result.Principal.FindFirstValue(ClaimTypes.Name)
                        };
                        var createResult = await _userManager.CreateAsync(user);
                        if (createResult.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                    }

                    await _signInManager.SignInAsync(user, isPersistent: true);

                    // Generate JWT token
                    string token = await GenerateJwtToken(user);

                    // Redirect with token in URL parameters
                    return Redirect($"https://meeting-room-niso-fe.vercel.app/login?token={token}");
                }

                return BadRequest();
            }


            private async Task<string> GenerateJwtToken(User user)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
    
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }


                
            
           
        

           
           

         

    }

    
    
    


    
}