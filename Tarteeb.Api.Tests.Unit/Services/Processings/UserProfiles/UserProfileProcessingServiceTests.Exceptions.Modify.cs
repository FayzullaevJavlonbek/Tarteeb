using FluentAssertions;
using Microsoft.OData.UriParser;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(UserDependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            dynamic randomUserProfileProperties = CreateRandomUserProfileProperties();

            var inputUserProfile = new UserProfile
            {
                Id = randomUserProfileProperties.Id,
                FirstName = randomUserProfileProperties.FirstName,
                LastName = randomUserProfileProperties.LastName,
                PhoneNumber = randomUserProfileProperties.PhoneNumber,
                Email = randomUserProfileProperties.Email,
                BirthDate = randomUserProfileProperties.BirthDate,
                IsActive = randomUserProfileProperties.IsActive,
                IsVerified = randomUserProfileProperties.IsVerified,
                GitHubUsername = randomUserProfileProperties.GitHubUsername,
                TelegramUsername = randomUserProfileProperties.TelegramUsername,
                TeamId = randomUserProfileProperties.TeamId
            };

            var expectedUserProfileProcessingValidationException =
                new UserProfileProcessingDependencyValidationException(dependencyValidationException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.IsAny<User>()))
                    .ThrowsAsync(expectedUserProfileProcessingValidationException);
            
            // when
            ValueTask<UserProfile> modifyUserProfileTask =
                this.userProfileProcessingService.ModifyUserProfileAsync(inputUserProfile);

            UserProfileProcessingDependencyValidationException actualUserProfileProcessingDependencyValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyValidationException>(
                    modifyUserProfileTask.AsTask);

            // then

            actualUserProfileProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserProfileProcessingValidationException);

            this.userServiceMock.Verify(service => 
                service.ModifyUserAsync(It.IsAny<User>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserProfileProcessingValidationException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
