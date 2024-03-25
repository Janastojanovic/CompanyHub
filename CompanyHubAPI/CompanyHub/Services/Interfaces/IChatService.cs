using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<Chat>> Get();
        Task<Chat> Get(int id);
        Task<Chat> Create(Chat chat);
        Task Update(int id, Chat chat);
        Task Delete(int id);
        //Task<IEnumerable<Chat>> GetRecentMessagesForChat(int chatId, int count);
        Task<IEnumerable<Chat>> GetChatsForUser(string userId);
        Task<IEnumerable<int>> GetChatsIdsForUser(string userId);
        Task<int> CountChatMembers(int id);
    }
}
