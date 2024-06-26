﻿using Domain.Models.Dtos.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ApiModels.OrganizationEmployee.response
{
    public class RemoveEmployeeResponse
    {
        public string Message { get; set; }
        public List<OrganizationEmployeeForResponseDto> Employees { get; set; }
    }
}
