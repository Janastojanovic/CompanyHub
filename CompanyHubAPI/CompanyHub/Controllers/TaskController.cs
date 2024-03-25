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
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly IProjectTaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly IAppUserService _userService;

        public TaskController(ILogger<TaskController> logger, IProjectTaskService taskService, IProjectService projectService, IAppUserService userService)
        {
            _logger = logger;
            _taskService = taskService;
            _projectService = projectService;
            _userService = userService;
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetTask")]
        public async Task<IActionResult> Get(string id)
        {
            var task = await _taskService.Get(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        [Route("UpdateTask/{dueDate}/{id}/{description}/{name}")]
        public async Task<IActionResult> UpdateUser( DateTime dueDate,string id, string description, string name)
        {

            var existingTask = await _taskService.Get(id);

            if (existingTask == null)
            {
                return NotFound();
            }

            var updatedTask = new ProjectTask
            {
                Id = existingTask.Id,
                Name = name,
                AssignedDate = existingTask.AssignedDate,
                Description = description,
                Deadline = dueDate,
                Procentage = existingTask.Procentage,
                UserId = existingTask.UserId,
                ProjectId = existingTask.ProjectId,
                Completed = existingTask.Completed,

            };
            await _taskService.Update(id, updatedTask);

            return Ok(updatedTask);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpDelete]
        [Route("DeleteTask/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var task = await _taskService.Get(id);
            if (task is null)
            {
                return NotFound();
            }

            await _taskService.Delete(id);

            return Ok(task);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [Route("AddTask")]
        public async Task<IActionResult> Add(ProjectTask task)
        {
            Guid guid = Guid.NewGuid();
            task.Id = guid.ToString();
            int taskNumber = 0;
            int taskProcentage = 0;
            
            var project = await _projectService.Get(task.ProjectId);
            var user = await _userService.Get(task.UserId);

            if (user == null || project==null) 
            {
                return BadRequest(new { message = "Invalid user or project" });
            }

            task.Procentage = 0;
            task.AssignedDate = DateTime.Now;
            task.UserId = user.Id;
            task.ProjectId = project.Id;
            task.Completed = false;
            var newTask = await _taskService.Create(task,user.UserName);

            var tasks = await _taskService.GetForProject(task.ProjectId);

            foreach (var t in tasks)
            {
                taskNumber++;
            }
            taskProcentage = 100 / taskNumber;
            foreach(var t in tasks)
            {
                await _taskService.UpdateProcentage(t.Id, taskProcentage);
            }

            return Ok(newTask);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetTasksForProject/{id}")]
        public async Task<IActionResult> GetForProject(string id)
        {
            var tasks = await _taskService.GetForProject(id);
            if (tasks is null)
            {
                return NotFound();
            }
            return Ok(tasks);
        }
        [Authorize(Roles = "ADMIN,user")]
        [HttpGet]
        [Route("GetTasksForUser/{id}")]
        public async Task<IActionResult> GetForUser(string id)
        {
            var tasks = await _taskService.GetForUser(id);
            if (tasks is null)
            {
                return NotFound();
            }
            return Ok(tasks);
        }

        [Authorize(Roles = "ADMIN,user")]
        [HttpPut]
        [Route("CompleteTask/{id}")]
        public async Task<IActionResult> CompleteTask(string id)
        {
            var task = await _taskService.Get(id);
            if (task is null)
            {
                return NotFound();
            }
            await _taskService.UpdateCompleted(id);
            return Ok(task);
        }

    }
}
