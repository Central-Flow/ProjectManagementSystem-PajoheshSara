﻿using Domain.Models.Dtos.ProjectMember.Response;
using Domain.Models.Dtos.ProjectTask.Request;
using Domain.Models.Dtos.ProjectTask.Response;
using Domain.Models.ServiceResponses.ProjectMember;
using Domain.Models.ServiceResponses.ProjectTask;
using Domain.Services.ApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementSystem.Controllers.ProjectTask
{
    [Route("/organization/project/tasklist/task")]
    [ApiController]
    public class ProjectTaskController(IProjectTaskService taskService) : ControllerBase
    {
        private readonly IProjectTaskService _taskService = taskService;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectTaskRequest request)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var serviceResponse = await _taskService
                .CreateTask(request, userId);

            if (serviceResponse.Status.Equals(CreateProjectTaskServiceResponseStatus.AccessDenied)||
                serviceResponse.Status.Equals(CreateProjectTaskServiceResponseStatus.TaskListNotExists))
                return StatusCode(StatusCodes.Status400BadRequest,
                        new CreateProjectTaskResponse
                        {
                            Message = serviceResponse.Status
                        });

            if (serviceResponse.Status.Equals(CreateProjectTaskServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new CreateProjectTaskResponse
                        {
                            Message = serviceResponse.Status
                        });

            return Ok(new CreateProjectTaskResponse
            {
                Message = serviceResponse.Status
            });

        }
    }
}