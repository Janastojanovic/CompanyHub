using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IAppUserService
    {
        Task<IEnumerable<AppUser>> Get();
        Task<AppUser> Get(string id);
        Task<AppUser> Create(AppUser chat);
        Task Update(string id, AppUser chat);
        Task Delete(string id);
        Task SetUserOnline(string userId,bool status);
        Task<AppUser> GetByEmail(string email);
        Task<IEnumerable<AppUser>> GetOnlineUsers();
        Task<IEnumerable<AppUser>> GetInvalidUsers();
        Task<IEnumerable<AppUser>> GetValidUsers();
        Task<IEnumerable<string>> GetNamesForUsers();
        Task<IEnumerable<AppUser>> GetUsersByName(string name);
    }
}
