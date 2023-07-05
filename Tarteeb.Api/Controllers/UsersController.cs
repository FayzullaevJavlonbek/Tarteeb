//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tarteeb.Api.Models.Foundations.Tickets.Exceptions;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Services.Foundations.Users;
using Tarteeb.Api.Services.Processings.Users;

namespace Tarteeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserProcessingService userProcessingService;
        private readonly IUserService userService;

        public UsersController(IUserProcessingService userProcessingService, IUserService userService)
        {
            this.userProcessingService = userProcessingService;
            this.userService = userService;
        }
            

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<Guid>> VerifyUserByIdAsync(Guid userId)
        {
            Guid verifiedId = await this.userProcessingService.VerifyUserByIdAsync(userId);

            return Ok(verifiedId);
        }

        [HttpPost]
        public async ValueTask<ActionResult<Guid>> ActivateUserByIdAsync([FromBody] Guid userId)
        {
            Guid activatedId = await this.userProcessingService.ActivateUserByIdAsync(userId);

            return Ok(activatedId);
        }
        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<User>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                return await this.userService.RetrieveUserByIdAsync(userId);
            }
            catch (UserDependencyException userDependencyException)
            {
                return InternalServerError(userDependencyException.InnerException);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is InvalidUserException)
            {
                return BadRequest(userValidationException.InnerException);
            }
            catch (UserValidationException userValidationException)
                when (userValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userValidationException.InnerException);
            }
            catch (UserServiceException userServiceException)
            {
                return InternalServerError(userServiceException.InnerException);
            }
        }
    }
}
