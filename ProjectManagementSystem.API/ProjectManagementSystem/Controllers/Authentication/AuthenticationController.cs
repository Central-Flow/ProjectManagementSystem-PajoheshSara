﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Models.ApiModels.Auth.Response;
using Microsoft.AspNetCore.Authorization;
using Domain.Services.ApiServices;
using Domain.Models.ServiceResponses.Auth;
using Serilog;
using Domain.Models.ApiModels.Auth.Request;
namespace ProjectManagementSystem.Controllers.Authentication
{
    /// <summary>
    /// Provides AuthenticationService using REST-ful services
    /// </summary>
    /// <param name="authenticationService"></param>
    [Route("user/auth")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> Register([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest,
                   new SignUpResponse
                   {
                       Message = "UserDetailsAreInvalid!"
                   });

            var serviceResponse = await _authenticationService.SignUpUser(request);

            if (serviceResponse.Status.Equals(SignUpServiceResponseStatus.EmailExists) ||
               serviceResponse.Status.Equals(SignUpServiceResponseStatus.CreationFaild))
                return StatusCode(StatusCodes.Status400BadRequest,
                    new SignUpResponse
                    {
                        Message = serviceResponse.Status
                    });

            if (serviceResponse.Status.Equals(SignUpServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new SignUpResponse
                    {
                        Message = serviceResponse.Status
                    });

            return Ok(new SignUpResponse
            {
                Message = serviceResponse.Status
            });
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> Login([FromBody] SignInRequest request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest,
                   new SignInResponse
                   {
                       Message = "UserDetailsAreRequired!"
                   });

            var serviceResponse = await _authenticationService.SignInUser(request);

            if (serviceResponse.Status.Equals(SignInServiceResponseStatus.InvalidUserCredentials))
                return StatusCode(StatusCodes.Status400BadRequest,
                     new SignInResponse
                     {
                         Message = serviceResponse.Status
                     });

            if (serviceResponse.Status.Equals(SignInServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new SignInResponse
                     {
                         Message = serviceResponse.Status
                     });

            return Ok(new SignInResponse
            {
                Message = serviceResponse.Status,
                IsNewUser = serviceResponse.IsNewUser,
                AccessToken = serviceResponse.Token,
                RefreshToken = serviceResponse.RefreshToken
            });
        }

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var serviceResponse = await _authenticationService.RefreshToken(request);

            if (serviceResponse.Status.Equals(RefreshTokenServiceResponseStatus.InvalidRefreshToken))
                return StatusCode(StatusCodes.Status400BadRequest,
                     new RefreshTokenResponse
                     {
                         Message = serviceResponse.Status
                     });

            if (serviceResponse.Status.Equals(RefreshTokenServiceResponseStatus.InternalError))
                return StatusCode(StatusCodes.Status500InternalServerError,
                     new RefreshTokenResponse
                     {
                         Message = serviceResponse.Status
                     });

            return Ok(new RefreshTokenResponse
            {
                Message = serviceResponse.Status,
                AccessToken = serviceResponse.Token
            });
        }
    }
}
