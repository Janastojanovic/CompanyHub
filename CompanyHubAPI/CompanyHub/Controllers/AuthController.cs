using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using CompanyHub.DTOs;
using CompanyHub.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using CompanyHub.Services;
using CompanyHub.Services.Interfaces;
using Azure.Core;
using System.Numerics;
using System.Text.Json;

namespace CompanyHub.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppUserService _userService;
        private readonly ICacheService _cacheService;
        public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IAppUserService userService, ICacheService cacheService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _cacheService = cacheService;
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpPut]
        [Route("ChagePassword/{id}/{oldPassword}/{newPassword}")]
        public async Task<IActionResult> ChangePassword(string id, string oldPassword, string newPassword)
        {
            var changePsswordResult = new IdentityResult();
            var onlineUsers = _cacheService.GetData<List<AppUser>>("onlineUsers");
            if (onlineUsers != null)
            {
                var existingUser = onlineUsers.FirstOrDefault(user => user.Id == id);
                if (existingUser != null)
                {
                    changePsswordResult = await _userManager.ChangePasswordAsync(existingUser, oldPassword, newPassword);
                    if (!changePsswordResult.Succeeded)
                    {
                        return BadRequest();
                    }

                    return Ok();
                }
            }
            var userExist = await _userManager.FindByIdAsync(id);
            if (userExist == null)
            {
                return NotFound();
            }

            changePsswordResult = await _userManager.ChangePasswordAsync(userExist, oldPassword, newPassword);
            if (!changePsswordResult.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize(Roles = "user,ADMIN")]
        //[Authorize(Policy = "AdminOrUserPolicy")]
        [HttpGet]
        [Route("CheckRole/{userId}")]
        public async Task<IActionResult> CheckRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Korisnik nije pronađen
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                // Korisnik nema dodijeljene uloge
                return NotFound();
            }
            // Uzmi prvu ulogu korisnika (možete prilagoditi logiku za više uloga)
            return Ok(new { role = roles[0] });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        [Route("ApproveUser/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var existingUser = new AppUser();
            var updatedUser = new AppUser();
            var invalidUsers = _cacheService.GetData<List<AppUser>>("invalidUsers");
            if (invalidUsers != null)
            {
                existingUser = invalidUsers.FirstOrDefault(user => user.Id == userId);
                if (existingUser != null)
                {
                    updatedUser = new AppUser
                    {
                        Id = existingUser.Id,
                        Firstname = existingUser.Firstname,
                        Lastname = existingUser.Lastname,
                        Phone = existingUser.Phone,
                        Email = existingUser.Email,
                        UserName = existingUser.UserName,
                        ConcurrencyStamp = existingUser.ConcurrencyStamp,
                        Online = false,
                        NormalizedEmail = existingUser.NormalizedEmail,
                        PasswordHash = existingUser.PasswordHash,
                        NormalizedUserName = existingUser.NormalizedUserName,
                        Approved = true,
                    };

                    await _userService.Update(userId, updatedUser);

                    var removedFromInvalid = invalidUsers.RemoveAll(u => u.Id == userId);
                    if (removedFromInvalid > 0)
                    {
                        _cacheService.SetData("invalidUsers", invalidUsers, DateTimeOffset.Now.AddMinutes(5));
                    }

                    var validUsers = _cacheService.GetData<List<AppUser>>("validUsers") ?? new List<AppUser>();
                    validUsers.Add(updatedUser);
                    _cacheService.SetData("validUsers", validUsers, DateTimeOffset.Now.AddMinutes(5));

                    return Ok(updatedUser);
                }
            }

            existingUser = await _userService.Get(userId);

            if (existingUser == null)
            {
                return NotFound("User not found");
            }
            updatedUser = new AppUser
            {
                Id = existingUser.Id,
                Firstname = existingUser.Firstname,
                Lastname = existingUser.Lastname,
                Phone = existingUser.Phone,
                Email = existingUser.Email,
                UserName = existingUser.UserName,
                ConcurrencyStamp = existingUser.ConcurrencyStamp,
                Online = false,
                NormalizedEmail = existingUser.NormalizedEmail,
                PasswordHash = existingUser.PasswordHash,
                NormalizedUserName = existingUser.NormalizedUserName,
                Approved = true,
            };
            await _userService.Update(userId, updatedUser);

            return Ok(updatedUser);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var appRole = new ApplicationRole { Name = request.Role };
            var createRole = await _roleManager.CreateAsync(appRole);

            return Ok(new { message = "role created successfully" });
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        private async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                {
                    return new RegisterResponse { Message = "User aleready exists", Success = false };
                }

                userExist = new AppUser
                {
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    Email = request.Email,
                    Phone = request.Phone,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.Username,
                    Approved = false
                };
                var createUserResult = await _userManager.CreateAsync(userExist, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return new RegisterResponse { Message = $"Create user failed {createUserResult?.Errors?.First()?.Description}", Success = false };
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExist, "USER");
                if (!addUserToRoleResult.Succeeded)
                {
                    return new RegisterResponse { Message = $"Create user succeded, but could not add user to role {createUserResult?.Errors?.First()?.Description}", Success = false };
                }

                return new RegisterResponse { Message = "User register successfully", Success = true };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = ex.Message, Success = false };
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            var result = await RegisterAsyncAdmin(request);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        private async Task<RegisterResponse> RegisterAsyncAdmin(RegisterRequest request)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                {
                    return new RegisterResponse { Message = "User aleready exists", Success = false };
                }

                userExist = new AppUser
                {
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    Email = request.Email,
                    Phone = request.Phone,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.Username,
                    Approved = true,
                };
                var createUserResult = await _userManager.CreateAsync(userExist, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return new RegisterResponse { Message = $"Create user failed {createUserResult?.Errors?.First()?.Description}", Success = false };
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(userExist, "ADMIN");
                if (!addUserToRoleResult.Succeeded)
                {
                    return new RegisterResponse { Message = $"Create user succeded, but could not add user to role {createUserResult?.Errors?.First()?.Description}", Success = false };
                }

                return new RegisterResponse { Message = "User register successfully", Success = true };
            }
            catch (Exception ex)
            {
                return new RegisterResponse { Message = ex.Message, Success = false };
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(new { message = result.Message });
        }
        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                //var user = await _userManager.FindByEmailAsync(request.Email);
                var user = await _userService.GetByEmail(request.Email);
                if (user == null)
                {
                    return new LoginResponse { Message = "Invalid email/password", Success = false };
                }
                if (!user.Approved)
                {
                    return new LoginResponse { Message = "Admin didn't approved you,try again later", Success = false };
                }

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7AHevdx6ac/zUhjT7+0O/G2M63itIwbM/JGRkkGpMT0="));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expires = DateTime.Now.AddMinutes(30);
                var token = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds);

                await _userService.SetUserOnline(user.Id, true);

                var onlineUsers = _cacheService.GetData<List<AppUser>>("onlineUsers");
                if (onlineUsers != null)
                {
                    onlineUsers.Add(user);
                    _cacheService.SetData("onlineUsers", onlineUsers, DateTimeOffset.Now.AddMinutes(10));
                }
                else
                {
                    onlineUsers = (await _userService.GetOnlineUsers()).ToList();
                    _cacheService.SetData("onlineUsers", onlineUsers, DateTimeOffset.Now.AddMinutes(10));
                }

                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login Seccesful",
                    Email = user?.Email,
                    Success = true,
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new LoginResponse { Success = false, Message = ex.Message };
            }

        }

    }
}
