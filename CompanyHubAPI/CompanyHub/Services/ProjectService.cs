using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Neo4jClient;
using Pipelines.Sockets.Unofficial.Arenas;

namespace CompanyHub.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IGraphClient _client;

        public ProjectService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Project> Create(Project project)
        {
            var nodeReference = await _client.Cypher
                .Create("(p:Project $newProject)")
                .WithParam("newProject", project)
                .Return(p => p.As<Project>())
                .ResultsAsync;

            return nodeReference.FirstOrDefault();
        }

        public async Task Delete(string id)
        {
            var tasks = await _client.Cypher
               .Match("(t:Task)")
               .Where((ProjectTask t) => t.ProjectId == id)
               .Return(t => t.As<ProjectTask>())
               .ResultsAsync;

                foreach (var task in tasks)
                {
                    var isOnlyParticipantQuery = await _client.Cypher
                        .Match("(user:User)-[:PARTICIPATES_IN]->(p:Project)")
                        .Where((UserNeoHelper user) => user.Id == task.UserId)
                        .Return(p => p.CountDistinct())
                        .ResultsAsync;

                    var numberOfProjects = isOnlyParticipantQuery.SingleOrDefault();

                    // Ako korisnik učestvuje samo u jednom projektu, obrišite ga
                    if (numberOfProjects == 1)
                    {
                        await _client.Cypher
                            .OptionalMatch("(user:User)-[r]-()")
                            .Where((UserNeoHelper user) => user.Id == task.UserId)
                            .DetachDelete("user")
                            .ExecuteWithoutResultsAsync();
                    }
                }

                await _client.Cypher
                    .OptionalMatch("(t:Task)-[r]-()")
                    .Where((ProjectTask t) => t.ProjectId == id)
                    .DetachDelete("r, t")
                    .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .OptionalMatch("(p:Project)")
                .Where((Project p) => p.Id == id)
                .Delete("p")
                .ExecuteWithoutResultsAsync();


          
        }

        public async Task<IEnumerable<Project>> Get()
        {
            var projects = await _client.Cypher
                .Match("(p:Project)")
                .Return(p => p.As<Project>())
                .ResultsAsync;

            return projects;
        }

        public async Task<Project> Get(string id)
        {
            var project = await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == id)
                .Return(p => p.As<Project>())
                .ResultsAsync;

            return project.SingleOrDefault();
        }

        public async Task Update(string id, Project project)
        {
            await _client.Cypher
                .Match("(p:Project)")
                .Where((Project p) => p.Id == id)
                .Set("p = $updatedProject")
                .WithParam("updatedProject", project)
                .ExecuteWithoutResultsAsync();
        }
    }
}
