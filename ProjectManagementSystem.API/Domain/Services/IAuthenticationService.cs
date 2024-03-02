﻿using Domain.Dtos.Auth.Request;
using Domain.ServiceResponse.User.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAuthenticationService
    {
        public Task<SignUpServiceResponse> SignUpUser(SignUpRequest request);
        public Task<SignInServiceResponse> SignInUser(SignInRequest request);
    }
}
