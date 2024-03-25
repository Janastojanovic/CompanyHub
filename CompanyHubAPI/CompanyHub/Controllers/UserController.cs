using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CompanyHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IAppUserService _userService;
        private readonly IChatUserMappingService _chatUserMappingService;

        public UserController(ILogger<UserController> logger, ICacheService cacheService, IAppUserService userService, IChatUserMappingService chatUserMappingService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _userService = userService;
            _chatUserMappingService = chatUserMappingService;
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> Get(string id)
        {
            var cacheData = _cacheService.GetData<AppUser>($"user{id}");

            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            cacheData = await _userService.Get(id);
            if (cacheData == null)
            {
                return NotFound();
            }
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<AppUser>($"user{id}", cacheData, expiryTime);
            return Ok(cacheData);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetUserName/{id}")]
        public async Task<IActionResult> GetName(string id)
        {
            var cacheData = _cacheService.GetData<AppUser>($"user{id}");

            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            cacheData = await _userService.Get(id);
            if (cacheData == null)
            {
                return NotFound();
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<AppUser>($"user{id}", cacheData, expiryTime);
            return Ok(new { name = cacheData.Firstname + " " + cacheData.Lastname });
        }
        //[Authorize(Roles = "ADMIN,user")]
        [HttpPut]
        [Route("SetOnlineStatus")]
        public async Task<IActionResult> SetOnlineStatus(string id, bool status)
        {
            var cacheData = _cacheService.GetData<AppUser>($"user{id}");

            if (cacheData != null)
            {
                await _userService.SetUserOnline(id, status);
                return Ok(cacheData);
            }

            cacheData = await _userService.Get(id);
            if (cacheData == null)
            {
                return NotFound();
            }

            await _userService.SetUserOnline(id, status);

            if (status == false)
            {
                var onlineUsers = _cacheService.GetData<List<AppUser>>("onlineUsers");
                if (onlineUsers != null)
                {
                    var existingUser = onlineUsers.FirstOrDefault(user => user.Id == id);
                    if (existingUser != null)
                    {
                        var removedFromOnline = onlineUsers.RemoveAll(u => u.Id == id);
                        if (removedFromOnline > 0)
                        {
                            _cacheService.SetData("onlineUsers", onlineUsers, DateTimeOffset.Now.AddMinutes(5));
                        }

                    }
                }
            }
            return Ok(cacheData);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpPut]
        [Route("UpdateUser/{id}/{firstname}/{lastname}/{phone}")]
        public async Task<IActionResult> UpdateUser(string id, string firstname, string lastname, string phone)
        {

            var existingUser = _cacheService.GetData<AppUser>($"user{id}");
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            var updatedUser = new AppUser();
            if (existingUser != null)
            {
                updatedUser = new AppUser
                {
                    Id = existingUser.Id,
                    Firstname = firstname,
                    Lastname = lastname,
                    Phone = phone,
                    Email = existingUser.Email,
                    UserName = existingUser.UserName,
                    ConcurrencyStamp = existingUser.ConcurrencyStamp,
                    Online = true,
                    NormalizedEmail = existingUser.NormalizedEmail,
                    PasswordHash = existingUser.PasswordHash,
                    NormalizedUserName = existingUser.NormalizedUserName,
                    Approved = existingUser.Approved,
                };
                await _userService.Update(id, updatedUser);
                _cacheService.SetData<AppUser>($"user{id}", updatedUser, expiryTime);
                return Ok(updatedUser);
            }

            existingUser = await _userService.Get(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            updatedUser = new AppUser
            {
                Id = existingUser.Id,
                Firstname = firstname,
                Lastname = lastname,
                Phone = phone,
                Email = existingUser.Email,
                UserName = existingUser.UserName,
                ConcurrencyStamp = existingUser.ConcurrencyStamp,
                Online = true,
                NormalizedEmail = existingUser.NormalizedEmail,
                PasswordHash = existingUser.PasswordHash,
                NormalizedUserName = existingUser.NormalizedUserName,
                Approved = existingUser.Approved,
            };
            await _userService.Update(id, updatedUser);
            _cacheService.SetData<AppUser>($"user{id}", updatedUser, expiryTime);
            return Ok(updatedUser);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.Get(id);
            if (user is null)
            {
                return NotFound();
            }

            await _userService.Delete(id);
            //await _chatUserMappingService.DeleteMappingsForUser(id);
            _cacheService.RemoveData<AppUser>($"user{id}");
            return Ok(user);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetOnlineUsers")]
        public async Task<IActionResult> GetOnline()
        {
            var onlineUsers = _cacheService.GetData<List<AppUser>>("onlineUsers");
            if (onlineUsers != null && onlineUsers.Count() > 0)
            {
                return Ok(onlineUsers);
            }
            onlineUsers = (await _userService.GetOnlineUsers()).ToList();
            if (onlineUsers is null)
            {
                return NotFound();
            }
            var expiryTime = DateTimeOffset.Now.AddMinutes(15);
            _cacheService.SetData<List<AppUser>>("onlineUsers", onlineUsers, expiryTime);
            return Ok(onlineUsers);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetValidUsers")]
        public async Task<IActionResult> GetValid()
        {
            var validUsers = _cacheService.GetData<List<AppUser>>("validUsers");
            if (validUsers != null && validUsers.Count() > 0)
            {
                return Ok(validUsers);
            }
            validUsers = (await _userService.GetValidUsers()).ToList();
            if (validUsers is null)
            {
                return NotFound();
            }
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<List<AppUser>>("validUsers", validUsers, expiryTime);
            return Ok(validUsers);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("GetInvalidUsers")]
        public async Task<IActionResult> GetInvalid()
        {
            var invalidUsers = _cacheService.GetData<List<AppUser>>("invalidUsers");
            if (invalidUsers != null && invalidUsers.Count() > 0)
            {
                return Ok(invalidUsers);
            }
            invalidUsers = (await _userService.GetInvalidUsers()).ToList();
            if (invalidUsers is null)
            {
                return NotFound();
            }
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<List<AppUser>>("invalidUsers", invalidUsers, expiryTime);
            return Ok(invalidUsers);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("SearchByName")]
        public async Task<IActionResult> SearchByName(string search)
        {
            var names = await _userService.GetNamesForUsers();
            foreach (var name in names)
            {
                //name.ToLower();
                if (name.ToLower().Contains(search.ToLower()))
                {
                    var users = await _userService.GetUsersByName(name);
                    return Ok(users);
                }
            }
            return NotFound();
        }
    }
}
