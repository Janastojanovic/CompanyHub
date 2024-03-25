using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IChatUserMappingService
    {
        Task<IEnumerable<ChatUserMapping>> Get();
        Task<ChatUserMapping> Get(int id);
        Task<ChatUserMapping> Create(ChatUserMapping chat);
        Task Update(int id, ChatUserMapping chat);
        Task Delete(int id);
        //Task<IEnumerable<ChatUserMapping>> DeleteMappingsForUser(string userId);
        Task DeleteMappingsForChat(int userId);
        Task<IEnumerable<ChatUserMapping>> GetForChat(int chatId);
        //Task<IEnumerable<Chat>> GetRecentMessagesForChat(int chatId, int count);
    }
}
