using CompanyHub.Data;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> Create(Chat chat)
        {
            _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            return chat;
        }

        public async Task Delete(int id)
        {
            var chatToDelete = await _context.Chats.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Chats.Remove(chatToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chat>> Get()
        {
            return await _context.Chats.ToListAsync();
        }

        public async Task<Chat> Get(int id)
        {
            return await _context.Chats.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(int id, Chat chat)
        {
            _context.Entry(chat).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Chat>> GetChatsForUser(string userId)
        {
            var chats= new List<Chat>();
            var mapings = await _context.ChatUserMappings.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            foreach (var maping in mapings)
            {
                chats.Add(await _context.Chats.AsNoTracking().Where(x => x.Id == int.Parse(maping.ChatId)).FirstOrDefaultAsync());
            }
            return chats;
        }
        public async Task<IEnumerable<int>> GetChatsIdsForUser(string userId)
        {
            var chats = new List<int>();
            var mapings = await _context.ChatUserMappings.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            foreach (var maping in mapings)
            {
                chats.Add(await _context.Chats.AsNoTracking().Where(x => x.Id == int.Parse(maping.ChatId)).Select(x=>x.Id).FirstOrDefaultAsync());
            }
            return chats;
        }
        public async Task<int> CountChatMembers(int id)
        {
            var members = new List<AppUser>();
            var mapings = await _context.ChatUserMappings.AsNoTracking().Where(x => x.ChatId == id.ToString()).ToListAsync();
            foreach (var maping in mapings)
            {
                members.Add(await _context.Users.AsNoTracking().Where(x => x.Id == maping.UserId).FirstOrDefaultAsync());
            }
            return members.Count();
        }
    }
}
