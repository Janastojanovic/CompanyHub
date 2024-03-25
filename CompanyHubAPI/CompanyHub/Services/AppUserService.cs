using CompanyHub.Data;
using CompanyHub.Models;
using CompanyHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Neo4jClient;
using System.Threading.Tasks;

namespace CompanyHub.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGraphClient _client;

        public AppUserService(ApplicationDbContext context, IGraphClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task<AppUser> Create(AppUser user)
        {
            _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task Delete(string id)
        {
            await _client.Cypher
                .OptionalMatch("(user:User)-[r]-()")
                .Where((UserNeoHelper user) => user.Id == id)
                .DetachDelete("r,user")
                .ExecuteWithoutResultsAsync();

            var tasks = await _client.Cypher
                .Match("(t:Task)")
                .Where((ProjectTask t) => t.UserId == id)
                .Return(t => t.As<ProjectTask>())
                .ResultsAsync;

            var procentage = 0;
            foreach(var task in tasks)
            {
                if(task.Completed)
                {
                    procentage = task.Procentage;
                }
                else
                {
                    procentage = 0;
                }
                
                await _client.Cypher
                .OptionalMatch("(t:Task)-[r]-()")
                .Where((ProjectTask t )=> t.Id == task.Id)
                .DetachDelete("r,t")
                .ExecuteWithoutResultsAsync();

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
                    .Return(t => t.As<ProjectTask>())
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

                var newProcentage = 100 / numbersOfTasks;

                foreach(var taks in tasksToUpdate)
                {
                    await _client.Cypher
                    .Match("(t:Task)")
                    .Where((ProjectTask t) => t.Id == taks.Id)
                    .Set("t.Procentage = $procentage")
                    .WithParam("procentage", newProcentage)
                    .ExecuteWithoutResultsAsync();
                }
            }
            var userToDelete = await _context.Users.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUser>> Get()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> Get(string id)
        {
            return await _context.Users.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<AppUser> GetByEmail(string email)
        {
            return await _context.Users.AsNoTracking().Where(x => x.Email == email).FirstOrDefaultAsync();
        }
        public async Task Update(string id, AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task SetUserOnline(string userId, bool status)
        {
            var user = await _context.Users.AsNoTracking().Where(x => x.Id == userId).FirstOrDefaultAsync();

            var updatedUser = new AppUser
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Phone = user.Phone,
                Email = user.Email,
                UserName = user.UserName,
                ConcurrencyStamp = user.ConcurrencyStamp,
                Online = status,
                NormalizedEmail = user.NormalizedEmail,
                PasswordHash = user.PasswordHash,
                NormalizedUserName = user.NormalizedUserName,
                Approved = user.Approved,
            };
            //_context.Entry(user).CurrentValues.SetValues(updatedUser);
            _context.Entry(updatedUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUser>> GetOnlineUsers()
        {
            return await _context.Users.AsNoTracking().Where(x => x.Online == true).ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetValidUsers()
        {
            return await _context.Users.AsNoTracking().Where(x => x.Approved == true).ToListAsync();
        }
        public async Task<IEnumerable<AppUser>> GetInvalidUsers()
        {
            return await _context.Users.AsNoTracking().Where(x => x.Approved == false).ToListAsync();
        }
        public async Task<IEnumerable<string>> GetNamesForUsers()
        {
            var names = new List<string>();
            var users = await _context.Users.AsNoTracking().Where(x => x.Approved == true).ToListAsync();

            foreach (var user in users)
            {
                names.Add(await _context.Users.AsNoTracking().Where(x => x.Id == user.Id).Select(x => x.Firstname).FirstOrDefaultAsync()
                    + " " + await _context.Users.AsNoTracking().Where(x => x.Id == user.Id).Select(x => x.Lastname).FirstOrDefaultAsync());
            }
            return names;
        }
        public async Task<IEnumerable<AppUser>> GetUsersByName(string name)
        {
            return await _context.Users
                        .Where(k => (k.Firstname + " " + k.Lastname) == name)
                        .ToListAsync();

        }
    }
}
