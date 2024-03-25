using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> Get();
        Task<Project> Get(string id);
        Task<Project> Create(Project project);
        Task Update(string id, Project project);
        Task Delete(string id);
    }
}
