using Azure.Identity;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace CompanyHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IChatService _chatService;
        private readonly IAppUserService _userService;
        private readonly IChatUserMappingService _chatUserMappingService;

        public ChatController(ILogger<ChatController> logger, ICacheService cacheService, IChatService chatService, IAppUserService userService, IChatUserMappingService chatUserMappingService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _chatService = chatService;
            _userService = userService;
            _chatUserMappingService = chatUserMappingService;
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("GetChats")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Chat>>("chats");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _chatService.Get();

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<List<Chat>>("chats", cacheData.ToList(), expiryTime);
            return Ok(cacheData);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpPost]
        [Route("AddChat/{user1}/{user2}")]
        public async Task<IActionResult> Add(string user1,string user2)
        {

            var chatExists = new Chat();
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            var chatMaps = new List<ChatUserMapping>();

            if (user1==user2)
            {
                return BadRequest("Users must be unique");
            }

            var check1 = await _chatService.GetChatsIdsForUser(user1);
            var check2 = await _chatService.GetChatsIdsForUser(user2);

            var cacheData1 = _cacheService.GetData<List<Chat>>($"user{user1}Chats");
            var cacheData2 = _cacheService.GetData<List<Chat>>($"user{user2}Chats");

            if (check1.Intersect(check2).Any())
            {
                IEnumerable<int> sameChat = check1.Intersect(check2);
                foreach(int i in sameChat)
                {
                    if(await _chatService.CountChatMembers(i)==2)
                    {
                        if (cacheData1!=null)
                        {
                            chatExists =cacheData1.FirstOrDefault(x=>x.Id==i);
                            return Ok(chatExists);
                        }
                        else if(cacheData2 != null)
                        {
                            chatExists = cacheData2.FirstOrDefault(x => x.Id == i);
                            return Ok(chatExists);
                        }
                        else
                        {
                            chatExists = await _chatService.Get(i);
                            var chats1 = await _chatService.GetChatsForUser(user1);
                            var chats2 = await _chatService.GetChatsForUser(user2);
                            _cacheService.SetData<List<Chat>>($"user{user1}Chats", chats1.ToList(), expiryTime);
                            _cacheService.SetData<List<Chat>>($"user{user2}Chats", chats2.ToList(), expiryTime);

                            chatMaps = (await _chatUserMappingService.GetForChat(i)).ToList();
                            _cacheService.SetData<List<ChatUserMapping>>($"chat{i}maps", chatMaps, expiryTime);

                            return Ok(chatExists);
                        } 
                    }
                }
            }

            var sender = await _userService.Get(user1);
            var receiver = await _userService.Get(user2);

            if(sender == null || receiver == null) 
            {
                return BadRequest("Both users must be valid");
            }

            var chat = new Chat();

            chat.Name= receiver.Firstname + " " + receiver.Lastname;
            var newChat = await _chatService.Create(chat);

            var chatmap1 = new ChatUserMapping { ChatId=newChat.Id.ToString(), UserId=sender.Id,UserName=sender.Firstname+" "+sender.Lastname };
            var chatmap2 = new ChatUserMapping { ChatId = newChat.Id.ToString(), UserId = receiver.Id, UserName = receiver.Firstname + " " + receiver.Lastname };
            await _chatUserMappingService.Create(chatmap1);
            await _chatUserMappingService.Create(chatmap2);

            chatMaps.Add(chatmap1);
            chatMaps.Add(chatmap2);
            
            _cacheService.SetData<List<ChatUserMapping>>($"chat{newChat.Id}maps",chatMaps,expiryTime);

            if (cacheData1 != null)
            {
                cacheData1.Add(newChat);
                _cacheService.SetData<List<Chat>>($"user{user1}Chats", cacheData1, expiryTime);
            }
            else
            {
                var chats1 = await _chatService.GetChatsForUser(user1);
                _cacheService.SetData<List<Chat>>($"user{user1}Chats", chats1.ToList(), expiryTime);
            }

            if (cacheData2 != null)
            {
                cacheData2.Add(newChat);
                _cacheService.SetData<List<Chat>>($"user{user2}Chats", cacheData2, expiryTime);
            }
            else
            {
                var chats2 = await _chatService.GetChatsForUser(user2);
                _cacheService.SetData<List<Chat>>($"user{user2}Chats", chats2.ToList(), expiryTime);
            }
            //_cacheService.SetData<Chat>($"chat{chat.Id}", newChat, expiryTime);

            return Ok(newChat);
        }
        //[Authorize(Roles = "ADMIN,user")]
        [HttpPost]
        [Route("AddGroupChat")]
        public async Task<IActionResult> AddGroup(string chatName, [FromBody] List<string> users)
        {
            var chatMaps = new List<ChatUserMapping>();
            bool isUnique=users.Distinct().Count()==users.Count();
            if (!isUnique)
            {
                return BadRequest(new { message = "All chat members must be unique" });
            }
            if (users.Count() <= 2)
            {
                return BadRequest(new { message = "Group chat must have more than 2 members" });
            }

            var chatMember = new AppUser();
            var chat = new Chat { Name = chatName };
            
            foreach (var user in users)
            {
                chatMember = await _userService.Get(user);

                if (chatMember==null)
                {
                    return BadRequest(new { message = "All users must be valid" });
                }
            }

            var newChat = await _chatService.Create(chat);
            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            //_cacheService.SetData<Chat>($"chat{chat.Id}", newChat, expiryTime);

            foreach (var user in users)
            {
                var cacheData1 = _cacheService.GetData<List<Chat>>($"user{user}Chats");
                if (cacheData1 != null)
                {
                    cacheData1.Add(newChat);
                    _cacheService.SetData<List<Chat>>($"user{user}Chats", cacheData1, expiryTime);
                }
                else
                {
                    var chats1 = await _chatService.GetChatsForUser(user);
                    _cacheService.SetData<List<Chat>>($"user{user}Chats", chats1.ToList(), expiryTime);
                }
                var neko = await _userService.Get(user);    
                var chatmap = new ChatUserMapping { ChatId = newChat.Id.ToString(), UserId = user , UserName = neko.Firstname+" "+ neko.Lastname };
                await _chatUserMappingService.Create(chatmap);
                chatMaps.Add(chatmap);
                _cacheService.SetData<List<ChatUserMapping>>($"chat{newChat.Id}maps", chatMaps, expiryTime);
            }
           
            
            return Ok(chat);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetChatsForUser/{id}")]
        public async Task<IActionResult> GetForUser(string id)
        {
            var cacheData = _cacheService.GetData<IEnumerable<Chat>>($"user{id}Chats");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _chatService.GetChatsForUser(id);

            if(cacheData==null)
            {
                return NotFound();
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetData<IEnumerable<Chat>>($"user{id}Chats", cacheData, expiryTime);
            return Ok(cacheData);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetChatMapsForChat/{id}")]
        public async Task<IActionResult> GetMapsForChat(int id)
        {
            var cacheData = _cacheService.GetData<List<ChatUserMapping>>($"chat{id}maps");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = (await _chatUserMappingService.GetForChat(id)).ToList();

            if (cacheData == null)
            {
                return NotFound();
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(30);
            _cacheService.SetData<List<ChatUserMapping>>($"chat{id}maps", cacheData, expiryTime);
            return Ok(cacheData);
        }
        [HttpDelete]
        [Route("DeleteChat/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var chat = await _chatService.Get(id);
            if (chat is null)
            {
                return NotFound();
            }

            await _chatService.Delete(id);
            //await _chatUserMappingService.DeleteMappingsForUser(id);
            //_cacheService.RemoveData<AppUser>($"user{id}");
            await _chatUserMappingService.DeleteMappingsForChat(id);
            return Ok(chat);
        }
        [HttpDelete]
        [Route("DeleteChatMap/{id}")]
        public async Task<IActionResult> DeleteMap(int id)
        {
            var chat = await _chatUserMappingService.Get(id);
            //if (chat is null)
            //{
            //    return NotFound();
            //}

            await _chatUserMappingService.Delete(id);
            //await _chatUserMappingService.DeleteMappingsForUser(id);
            //_cacheService.RemoveData<AppUser>($"user{id}");
            //await _chatUserMappingService.DeleteMappingsForChat(id);
            return Ok(chat);
        }
    }
}
