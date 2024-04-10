﻿using Domain.Models.Dtos.Organization.Response;
using Domain.Models.Dtos.ProjectMember.Request;
using Domain.Models.Dtos.ProjectMember.Response;
using Domain.Models.ServiceResponses.Organization;
using Domain.Models.ServiceResponses.ProjectMember;
using Domain.Services.ApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementSystem.Controllers.ProjectMember
{
    [Route("organization/project/member")]
    [ApiController]
    public class ProjectMemberController(IProjectMemberService projectMemberService) : ControllerBase
    {
        private readonly IProjectMemberService _projectMemberService = projectMemberService;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMember(AddMemberRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest,
                        new AddMemberResponse
                        {
                            Message = "DetailsAreInvalid"
                        });

            var userId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var serviceResponse = await _projectMemberService
                .AddMember(request, userId);

            if (serviceResponse.Status.Equals(AddMemberServiceResponseStatus.ProjectNotExists) ||
                serviceResponse.Status.Equals(AddMemberServiceResponseStatus.EmployeeNotExists) ||
                serviceResponse.Status.Equals(AddMemberServiceResponseStatus.AccessDenied))
                return StatusCode(StatusCodes.Status400BadRequest,
                        new AddMemberResponse
                        {
                            Message = serviceResponse.Status
                        });

            if (serviceResponse.Status.Equals(AddMemberServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new AddMemberResponse
                        {
                            Message = serviceResponse.Status
                        });

            return Ok(new AddMemberResponse
            {
                Message= serviceResponse.Status
            });
        }
    }
}