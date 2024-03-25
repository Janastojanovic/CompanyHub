using CompanyHub.Data;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CompanyHub.Services
{
    public class MessageService: IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Message> Create(Message message)
        {
            _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task Delete(int id)
        {
            var messageToDelete = await _context.Messages.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Messages.Remove(messageToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> Get()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<Message> Get(int id)
        {
            return await _context.Messages.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(int id, Message message)
        {
            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }
        public async Task<IEnumerable<Message>> GetRecentMessagesForChat(int chatId, int count)
        {
            var allMessages = await _context.Messages
                                     .AsNoTracking()
                                     .Where(x => x.ChatId == chatId)
                                     .ToListAsync();

            // Ako je broj poruka manji ili jednak broju traženih poruka, jednostavno vraćamo sve poruke
            if (allMessages.Count <= count)
                return allMessages;

            // Inače, preskočimo prvih 'broj - count' poruka i uzmemo preostale poruke
            var recentMessages = allMessages.Skip(allMessages.Count - count);

            return recentMessages;
        }

    }
}
