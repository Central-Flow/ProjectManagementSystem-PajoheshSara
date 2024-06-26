﻿using Domain.Models.ApiModels.OrganizationEmployee.Request;
using Domain.Models.ServiceResponses.OrganizationEmployee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.ApiServices
{
    public interface IOrganizationEmployeeService
    {
        public Task<ChangeEmployeeRoleServiceResponse> ChangeEmployeeRole(ChangeEmployeeRoleRequest request, string email);
        public Task<RemoveEmployeeServiceResponse> RemoveEmployee(RemoveEmployeeRequest request, string userId);
        public Task<SearchEmployeeServiceResponse> SearchEmployee(SearchEmployeeRequest request, string userId);
    }
}
