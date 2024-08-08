
using AuthCheck.Server.Dtos;
using AuthCheck.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RestSharp;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace AuthCheck.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(UserManager<AppUser> userManager,
        //RoleManager<IdentityRole> roleManager,
        IConfiguration configuration) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;

        //private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IConfiguration _configuration = configuration;

        //api/account/register

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDtos registerDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            {
                Email = registerDtos.Email,
                FullName = registerDtos.FullName,
                UserName = registerDtos.FullName,
                ProfileImage = registerDtos.Image
                
            };
            var result = await _userManager.CreateAsync(user, registerDtos.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //if(registerDtos.Roles is null)
            //{
            //    await _userManager.AddToRoleAsync(user, "User");
            //}
            //else
            //{
            //    foreach(var role in registerDtos.Roles)
            //    {
            //        await _userManager.AddToRoleAsync(user, role);
            //    }
            //}
            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Account Created Successfully!"
            });
           

            
        }
        //api/account/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>>Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null)
            {
                return Unauthorized(new AuthResponseDto
                {

                    IsSuccess = false,
                    Message = "User not found with this email",
                });

            }
            var result = await _userManager.
                CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid Password."
                });
            }

            var token = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration.GetSection("JWTSetting").GetSection("RefreshTokenValidityIn").Value!, out int RefreshTokenValidityIn);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidityIn);
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {

                IsSuccess = true,
                Token = token,
                Message = "Login Success.",
               RefreshToken = refreshToken 
            });

        }

        
        [HttpPost("forget-password")]
        public async Task<ActionResult>ForgotPassword(ForgetPasswordDto forgetPasswordDto)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
#pragma warning restore CS8604 // Possible null reference argument.
            if (user is null)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email."
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://localhost:4200/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";


            //using RestSharp;

            //var client = new RestClient("https://send.api.mailtrap.io/api/send");
            //var request = new RestRequest();
            //request.AddHeader("Authorization", "Bearer 045541177c5b382049203934eb71ec39");
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", "{\"from\":{\"email\":\"mailtrap@demomailtrap.com\",\"name\":\"Mailtrap Test\"},\"to\":[{\"email\":\"anutnt123@gmail.com\"}],\"template_uuid\":\"397bf3f0-e4f5-4d22-aec7-25680fe8b36d\",\"template_variables\":{\"user_email\":\"Test_User_email\",\"pass_reset_link\":\"Test_Pass_reset_link\"}}", ParameterType.RequestBody);
            //var response = client.Post(request);
            //System.Console.WriteLine(response.Content);

            var client = new RestClient("https://send.api.mailtrap.io/api/send");
            var request = new RestRequest
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json,
            };
            request.AddHeader("Authorization", "Bearer 045541177c5b382049203934eb71ec39");
            request.AddJsonBody(new
            {
                from = new { email = "mailtrap@demomailtrap.com" },
                to = new[] { new { email = user.Email } },
                template_uuid = "397bf3f0-e4f5-4d22-aec7-25680fe8b36d",
                template_variables = new {user_email= user.Email, pass_reset_link= resetLink}
            });
            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true, 
                    Message = "Email sent with password reset link.Please check your email."
                });
            }
            else
            {
                return BadRequest(
                    new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = response.Content!.ToString()

                    }
                    ) ;
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            //resetPasswordDto.Token = WebUtility.UrlDecode(resetPasswordDto.Token);

            if (user == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email."

                });
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {
                    IsSuccess = true,
                    Message = "Password reset Successfully."
                });
            }
            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = result.Errors.FirstOrDefault()!.Description
            });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult>ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync (changePasswordDto.Email);
            if (user is null) {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User does not exist with this email."
                });
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new AuthResponseDto
                {IsSuccess = true,
                Message ="Password changed successfully."

                });
            }
            return BadRequest(new AuthResponseDto
            {
                IsSuccess = false,
                Message = result.Errors.FirstOrDefault()!.Description
            });
        }

#pragma warning disable CA1822 // Mark members as static
        private string GenerateRefreshToken()
#pragma warning restore CA1822 // Mark members as static
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        private string GenerateToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.
                GetBytes(_configuration.GetSection("JWTSetting")
                .GetSection("securityKey").Value!);
            List<Claim> claims = [
                new (JwtRegisteredClaimNames.Email, user.Email?? ""),
                new (JwtRegisteredClaimNames.Name,user.FullName?? ""),
                 new (JwtRegisteredClaimNames.NameId,user.Id?? ""),
                  new (JwtRegisteredClaimNames.Aud,
                  _configuration
                  .GetSection("JWTSetting").GetSection("validAudience").Value!),
                  new(JwtRegisteredClaimNames.Iss,
                  _configuration.GetSection("JWTSetting").GetSection("validIssuer").Value!)
                ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        //api/account/detail
        [HttpGet("detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);


            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            return Ok(new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,

            });

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDetailDto>>> GetUsers()
        {
            var users = await _userManager.Users.Select(u => new UserDetailDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
            }).ToListAsync();

            return Ok(users);
        }
    }
}
 