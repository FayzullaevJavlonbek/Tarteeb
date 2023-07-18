//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.Users;
using Tarteeb.Api.Services.Processings.Users;

namespace Tarteeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : RESTFulController
    {
        private readonly IUserProcessingService userProcessingService;

        public UsersController(IUserProcessingService userProcessingService) =>
            this.userProcessingService = userProcessingService;

        [HttpGet("{userId}")]
        public async ValueTask<ActionResult<Guid>> VerifyUserByIdAsync(Guid userId)
        {
            try
            {
                Guid verifiedId = await this.userProcessingService.VerifyUserByIdAsync(userId);

                return Ok(verifiedId);
            }
            catch (UserProcessingValidationException userProcessingValidationException)
            {
                return BadRequest(userProcessingValidationException.InnerException);
            }
            catch (UserProcessingDependencyValidationException userProcessingDependencyValidationException)
                when(userProcessingDependencyValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userProcessingDependencyValidationException.InnerException);
            }
            catch (UserProcessingDependencyValidationException userProcessingDependencyValidationException)
            {
                return BadRequest(userProcessingDependencyValidationException.InnerException);
            }
            catch (UserProcessingDependencyException userProcessingDependencyException)
            {
                return InternalServerError(userProcessingDependencyException.InnerException);
            }
            catch(UserProcessingServiceException userProcessingServiceException)
            {
                return InternalServerError(userProcessingServiceException.InnerException);
            }
        }

        [HttpPost]
        public async ValueTask<ActionResult<Guid>> ActivateUserByIdAsync([FromBody] Guid userId)
        {
            Guid activatedId = await this.userProcessingService.ActivateUserByIdAsync(userId);

            return Ok(activatedId);
        }
    }
}
