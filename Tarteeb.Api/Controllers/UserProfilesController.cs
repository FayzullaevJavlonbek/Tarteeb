//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RESTFulSense.Controllers;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Tarteeb.Api.Services.Processings.UserProfiles;

namespace Tarteeb.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : RESTFulController
    {
        private readonly IUserProfileProcessingService userProfileProcessingService;

        public UserProfilesController(IUserProfileProcessingService userProfileProcessingService) =>
            this.userProfileProcessingService = userProfileProcessingService;

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<UserProfile>> GetAllUserProfiles()
        {
            try
            {
                IQueryable<UserProfile> allUserProfiles =
                    this.userProfileProcessingService.RetrieveAllUserProfiles();

                return Ok(allUserProfiles);
            }
            catch (UserProfileProcessingDependencyException userProfileProcessingDependencyException)
            {
                return InternalServerError(userProfileProcessingDependencyException.InnerException);
            }
            catch (UserProfileProcessingServiceException userProfileProcessingServiceException)
            {
                return InternalServerError(userProfileProcessingServiceException.InnerException);
            }
        }


        [HttpGet("{userProfileId}")]
        public async ValueTask<ActionResult<UserProfile>> GetUserProfileByIdAsync(Guid userProfileId)
        {
            try
            {
                return await this.userProfileProcessingService.RetrieveUserProfileByIdAsync(userProfileId);
            }
            catch (UserProfileProcessingValidationException userProfileProcessingValidationException)
            {
                return BadRequest(userProfileProcessingValidationException.InnerException);
            }
            catch (UserProfileProcessingDependencyValidationException userProfileProcessingDependencyValidationException)
                when(userProfileProcessingDependencyValidationException.InnerException is NotFoundUserException)
            {
                return NotFound(userProfileProcessingDependencyValidationException.InnerException);
            }
            catch (UserProfileProcessingDependencyValidationException userProfileProcessingDependencyValidationException)
            {
                return BadRequest(userProfileProcessingDependencyValidationException.InnerException);
            }
            catch (UserProfileProcessingDependencyException userProfileProcessingDependencyException)
            {
                return InternalServerError(userProfileProcessingDependencyException.InnerException);
            }
            catch (UserProfileProcessingServiceException userProfileProcessingServiceException)
            {
                return InternalServerError(userProfileProcessingServiceException.InnerException);
            }
        }

    }
}
