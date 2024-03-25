using CompanyHub.Models;

namespace CompanyHub.Services.Interfaces
{
    public interface IProjectTaskService
    {
        Task<IEnumerable<ProjectTask>> Get();
        Task<ProjectTask> Get(string id);
        Task<ProjectTask> Create(ProjectTask task, string username);
        Task Update(string id, ProjectTask task);
        Task Delete(string id);
        Task<IEnumerable<ProjectTask>> GetForProject(string id);
        Task UpdateProcentage(string id, int procentage);
        Task UpdateCompleted(string id);
        Task<IEnumerable<ProjectTask>> GetForUser(string id);
    }
}
