using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IMessageService _messageService;

        public MessageController(ILogger<MessageController> logger, ICacheService cacheService, IMessageService messageService)
        {
            _logger = logger;
            _cacheService = cacheService;
            _messageService = messageService;
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetMessage")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Message>>("messages");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _messageService.Get();

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<IEnumerable<Message>>("messages", cacheData, expiryTime);
            return Ok(cacheData);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetRecentMessages/{chatId}")]
        public async Task<IActionResult> GetRecent(int chatId)
        {
            var cacheData = _cacheService.GetData<IEnumerable<Message>>($"recentMessages{chatId}");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _messageService.GetRecentMessagesForChat(chatId, 50);

            if (cacheData == null)
            {
                return NotFound();
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            _cacheService.SetData<IEnumerable<Message>>($"recentMessages{chatId}", cacheData, expiryTime);
            return Ok(cacheData);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetLastMessage/{chatId}")]
        public async Task<IActionResult> GetLast(int chatId)
        {
            var lastMessage = new Message();
            var cacheData = _cacheService.GetData<IEnumerable<Message>>($"recentMessages{chatId}");

            if (cacheData != null && cacheData.Count() > 0)
            {
                lastMessage = cacheData.Last();
                return Ok(lastMessage);
            }

            cacheData = await _messageService.GetRecentMessagesForChat(chatId, 1);
            lastMessage = cacheData.FirstOrDefault();

            if (lastMessage == null)
            { return Ok(new { message = "" }); }

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            return Ok(new { message = lastMessage.Text });
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpPost]
        [Route("AddMessage/{chatId}")]
        public async Task<IActionResult> Add(Message message, int chatId)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
            {
                return BadRequest("Empty message");
            }

            message.Time = DateTime.Now;
            message.ChatId = chatId;
            var newMessage = await _messageService.Create(message);

            var recentMessages = _cacheService.GetData<List<Message>>($"recentMessages{chatId}");

            if(recentMessages != null && recentMessages.Count() > 0)
            {
                recentMessages.Add(newMessage);
                _cacheService.SetData($"recentMessages{chatId}", recentMessages, DateTimeOffset.Now.AddMinutes(5));

                return Ok(message);
            }
            var recentMessagesDb = await _messageService.GetRecentMessagesForChat(chatId, 50);
            _cacheService.SetData($"recentMessages{chatId}", recentMessagesDb, DateTimeOffset.Now.AddMinutes(5));

            return Ok(message);
        }
        //[HttpDelete]
        //[Route("DeleteChat/{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var chat = await _messageService.Get(id);
        //    if (chat is null)
        //    {
        //        return NotFound();
        //    }

        //    await _messageService.Delete(id);
        //    //await _chatUserMappingService.DeleteMappingsForUser(id);
        //    //_cacheService.RemoveData<AppUser>($"user{id}");
        //    //await _chatUserMappingService.DeleteMappingsForChat(id);
        //    return Ok(chat);
        //}
    }
}
