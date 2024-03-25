using CompanyHub.Data;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services
{
    public class ChatUserMappingService : IChatUserMappingService
    {
        private readonly ApplicationDbContext _context;

        public ChatUserMappingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatUserMapping> Create(ChatUserMapping chat)
        {
            _context.ChatUserMappings.AddAsync(chat);
            await _context.SaveChangesAsync();

            return chat;
        }

        public async Task Delete(int id)
        {
            var chatToDelete = await _context.ChatUserMappings.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.ChatUserMappings.Remove(chatToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatUserMapping>> Get()
        {
            return await _context.ChatUserMappings.ToListAsync();
        }


        public async Task<ChatUserMapping> Get(int id)
        {
            return await _context.ChatUserMappings.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(int id, ChatUserMapping chat)
        {
            _context.Entry(chat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        //public async Task<IEnumerable<ChatUserMapping>> DeleteMappingsForUser(string userId)
        //{
        //    var chats = new List<ChatUserMapping>();
        //    var mapings = await _context.ChatUserMappings.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
        //    foreach (var maping in mapings)
        //    {
        //        _context.ChatUserMappings.Remove(maping);
        //    }
        //    return chats;
        //}
        public async Task DeleteMappingsForChat(int chatId)
        {
            //var chats = new List<ChatUserMapping>();
            var mapings = await _context.ChatUserMappings.AsNoTracking().Where(x => x.ChatId == chatId.ToString()).ToListAsync();
            foreach (var maping in mapings)
            {
                _context.ChatUserMappings.Remove(maping);
            }
        }
        public async Task<IEnumerable<ChatUserMapping>> GetForChat(int chatId)
        {
            return await _context.ChatUserMappings.AsNoTracking().Where(x => x.ChatId == chatId.ToString()).ToListAsync();
        }
    }
}
