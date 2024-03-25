using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IMessageService
    {
            Task<IEnumerable<Message>> Get();
            Task<Message> Get(int id);
            Task<Message> Create(Message message);
            Task Update(int id, Message message);
            Task Delete(int id);
            Task<IEnumerable<Message>> GetRecentMessagesForChat(int chatId,int count);
    }
}
