using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.VisualBasic;
using Neo4jClient;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CompanyHub.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IGraphClient _client;
        public ProjectTaskService(IGraphClient client)
        {
            _client = client;
        }
        public async Task<ProjectTask> Create(ProjectTask task, string username)
        {

            var nodeReference = await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == task.ProjectId)
                .Create("(t: Task $newTask)-[:PART_OF]->(p)")
                .WithParam("newTask", task)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            var newTask = nodeReference.FirstOrDefault();

            var newUser = new UserNeoHelper { Id = task.UserId, UserName = username };
            await _client.Cypher
                .Merge("(user:User { Id: $id })")
                .OnCreate()
                .Set("user = $newUser")
                .WithParams(new
                {
                    id = newUser.Id,
                    newUser
                })
                .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .Match("(t:Task), (user:User)")
                .Where((ProjectTask t, UserNeoHelper user) => t.Id == newTask.Id && user.Id == task.UserId)
                .Create("(t)-[:ASSIGNED_TO]->(user)")
                .ExecuteWithoutResultsAsync();

            //await _client.Cypher
            //    .Match("(p:Project)", "(user:User)")
            //    .Where((Project p) => p.Id == task.ProjectId)
            //    .AndWhere((UserNeoHelper user) => user.Id == task.UserId)
            //    .CreateUnique("(user)-[:PARTICIPATES_IN]->(p)")
            //    .ExecuteWithoutResultsAsync();

            var query = _client.Cypher
            .Match("(u:User {Id: $userId})")
            .Match("(p:Project {Id: $projectId})")
            .Merge("(u)-[:PARTICIPATES_IN]->(p)")
            .WithParams(new
            {
                userId = task.UserId,
                projectId = task.ProjectId
            })
            .ExecuteWithoutResultsAsync();

            return newTask;
        }

        public async Task Delete(string id)
        {
            var procentage = 0;

            var task = (await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == id)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync).SingleOrDefault();


            await _client.Cypher
                .OptionalMatch("(t:Task)-[r]-()")
                .Where((ProjectTask t) => t.Id == id)
                .DetachDelete("r, t")
                .ExecuteWithoutResultsAsync();

            var isOnlyParticipantQuery = await _client.Cypher
                    .Match("(t:Task)-[:ASSIGNED_TO]->(user:User)")
                    .Where((UserNeoHelper user) => user.Id == task.UserId)
                    .Return(t => t.CountDistinct())
                    .ResultsAsync;

            var numberOfTasks = isOnlyParticipantQuery.SingleOrDefault();

            // Ako korisnik učestvuje samo u jednom projektu, obrišite ga
            if (numberOfTasks == 1)
            {
                await _client.Cypher
                    .OptionalMatch("(user:User)-[r]-()")
                    .Where((UserNeoHelper user) => user.Id == task.UserId)
                    .DetachDelete("user")
                    .ExecuteWithoutResultsAsync();
            }

            if (task.Completed)
            {
                procentage = task.Procentage;
            }
            else { procentage = 0; }

            var project = (await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == task.ProjectId)
                .Return(p => p.As<Project>())
                .ResultsAsync).SingleOrDefault();

            var procentageCompleted = project.ProcentageCompleted - procentage;

            await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == task.ProjectId)
                .Set("p.ProcentageCompleted = $completed")
                .WithParam("completed", procentageCompleted)
                .ExecuteWithoutResultsAsync();

            var tasksToUpdate = await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.ProjectId == task.ProjectId)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            var numbersOfTasks = (await _client.Cypher
                .Match("(t:Task)-[:PART_OF]->(p:Project)")
                .Where((Project p) => p.Id == task.ProjectId)
                .Return(t => t.CountDistinct())
                .ResultsAsync).SingleOrDefault();

            long newProcentage=0;
            if (numberOfTasks>0)
            {
                newProcentage = 100 / numbersOfTasks;
            }
            else
            {
                newProcentage = 100;
            }

            foreach (var taks in tasksToUpdate)
            {
                await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == taks.Id)
                .Set("t.Procentage = $procentage")
                .WithParam("procentage", newProcentage)
                .ExecuteWithoutResultsAsync();
            }

        }
        public async Task<IEnumerable<ProjectTask>> Get()
        {
            var tasks = await _client.Cypher
                .Match("(t:Task)")
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            return tasks;
        }

        public async Task<ProjectTask> Get(string id)
        {
            var task = await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == id)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            return task.SingleOrDefault();
        }

        public async Task Update(string id, ProjectTask task)
        {
            await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == id)
                .Set("t = $updatedTask")
                .WithParam("updatedTask", task)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetForProject(string id)
        {
            var tasks = await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.ProjectId == id)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            return tasks;
        }
        public async Task<IEnumerable<ProjectTask>> GetForUser(string id)
        {
            var tasks = await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.UserId == id)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            return tasks;
        }
        public async Task UpdateProcentage(string taskId, int procentage)
        {
            await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == taskId)
                .Set("t.Procentage = $procentage")
                .WithParam("procentage", procentage)
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateCompleted(string id)
        {
            var updatedTask = (await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.Id == id)
                .Set("t.Completed = $completed")
                .WithParam("completed", true)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync).SingleOrDefault();

            var project = (await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == updatedTask.ProjectId)
                .Return(p => p.As<Project>())
                .ResultsAsync).SingleOrDefault();

            var procentageCompleted = project.ProcentageCompleted + updatedTask.Procentage;

            await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == updatedTask.ProjectId)
                .Set("p.ProcentageCompleted = $completed")
                .WithParam("completed", procentageCompleted)
                .ExecuteWithoutResultsAsync();
        }
    }
}
