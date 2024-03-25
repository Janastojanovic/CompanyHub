using CompanyHub.Models;
using CompanyHub.Services;
using CompanyHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CompanyHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService;

        public ProjectController(ILogger<ProjectController> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetProject")]
        public async Task<IActionResult> Get(string id)
        {
            var project = await _projectService.Get(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        [Route("UpdateProject/{dueDate}/{id}/{description}/{name}")]
        public async Task<IActionResult> UpdateUser(DateTime dueDate, string id, string description,string name)
        {

            var existingProject = await _projectService.Get(id);
           
            if (existingProject == null)
            {
                return NotFound();
            }

            var updatedProject = new Project
            {
                Id = existingProject.Id,
                Name = name,
                StartDate = existingProject.StartDate,
                Description= description,
                Deadline= dueDate,
                ProcentageCompleted=existingProject.ProcentageCompleted,

            };
            await _projectService.Update(id, updatedProject);

            return Ok(updatedProject);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpDelete]
        [Route("DeleteProject/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var project = await _projectService.Get(id);
            if (project is null)
            {
                return NotFound();
            }

            await _projectService.Delete(id);

            return Ok(project);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [Route("AddProject")]
        public async Task<IActionResult> Add(Project project)
        {
            Guid guid = Guid.NewGuid();
            project.Id = guid.ToString();
            project.StartDate = DateTime.Now;
            project.ProcentageCompleted = 0;

            var newProject = await _projectService.Create(project);

            return Ok(newProject);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetProjects")]
        public async Task<IActionResult> Get()
        {
            var projects = await _projectService.Get();
            if (projects is null)
            {
                return NotFound();
            }
            return Ok(projects);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetProjectName/{id}")]
        public async Task<IActionResult> GetName(string id)
        {
            var project = await _projectService.Get(id);
            if (project is null)
            {
                return NotFound();
            }
            return Ok(new { name = project.Name });
        }

    }
}
