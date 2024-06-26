﻿using Domain.Models.ApiModels.OrganizationEmployee.Request;
using Domain.Models.ApiModels.Auth.Response;
using Domain.Models.ApiModels.Organization.Response;
using Domain.Models.ApiModels.OrganizationEmployee.response;
using Domain.Models.ServiceResponses.Organization;
using Domain.Models.ServiceResponses.OrganizationEmployee;
using Domain.Services.ApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Models.Dtos.Organization;

namespace ProjectManagementSystem.Controllers.Employee
{
    /// <summary>
    /// Provides OrganizationEmployeeService using REST-ful services
    /// </summary>
    /// <param name="employeeService"></param>
    [Route("organization/employee")]
    [ApiController]
    public class EmployeeController(IOrganizationEmployeeService employeeService) : ControllerBase
    {
        private readonly IOrganizationEmployeeService _employeeService = employeeService;

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> ChangeRole(ChangeEmployeeRoleRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest,
                       new ChangeEmployeeRoleResponse
                       {
                           Message = "DetailsAreRequired!"
                       });

            var userId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var serviceResponse = await _employeeService.ChangeEmployeeRole(request, userId);

            if (serviceResponse.Status.Equals(ChangeEmployeeRoleServiceResponseStatus.OrganizationNotExists) ||
                serviceResponse.Status.Equals(ChangeEmployeeRoleServiceResponseStatus.AccessDenied) ||
                serviceResponse.Status.Equals(ChangeEmployeeRoleServiceResponseStatus.EmployeeNotExists))
                return StatusCode(StatusCodes.Status400BadRequest,
                     new ChangeEmployeeRoleResponse
                     {
                         Message = serviceResponse.Status,
                     });

            if (serviceResponse.Status.Equals(ChangeEmployeeRoleServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new ChangeEmployeeRoleResponse
                     {
                         Message = serviceResponse.Status,
                     });

            return Ok(new ChangeEmployeeRoleResponse
            {
                Message = serviceResponse.Status
            });
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(RemoveEmployeeRequest request)
        {
            var userId = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var serviceResponse = await _employeeService
                .RemoveEmployee(request, userId);

            if (serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.EmployeeIsBusy) ||
               serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.AccessDenied) ||
               serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.EmployeeNotExists))
                return StatusCode(StatusCodes.Status400BadRequest,
                     new RemoveEmployeeResponse
                     {
                         Message = serviceResponse.Status,
                     });

            if (serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new RemoveEmployeeResponse
                     {
                         Message = serviceResponse.Status,
                     });

            return Ok(new RemoveEmployeeResponse
            {
                Message = serviceResponse.Status
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string organizationId, [FromQuery] string query)
        {
            var userId = HttpContext.User.Claims
               .FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var serviceResponse = await _employeeService
                .SearchEmployee(new()
                {
                    OrganizationId
                    = organizationId,
                    Query = query
                }, userId);

            if (serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.AccessDenied))
                return StatusCode(StatusCodes.Status400BadRequest,
                     new RemoveEmployeeResponse
                     {
                         Message = serviceResponse.Status,
                     });

            if (serviceResponse.Status.Equals(RemoveEmployeeServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new RemoveEmployeeResponse
                     {
                         Message = serviceResponse.Status,
                     });

            List<OrganizationEmployeeForResponseDto> employees = [];
            foreach (var e in serviceResponse.Emloyees)
            {
                employees.Add(new()
                {
                    Id = e.Id.ToString(),
                    Role = e.Role,
                    Email = e.User.Email,
                    FullName = e.User.FullName
                });
            }

            return Ok(new RemoveEmployeeResponse
            {
                Message = serviceResponse.Status,
                Employees = employees
            });
        }
    }
}
