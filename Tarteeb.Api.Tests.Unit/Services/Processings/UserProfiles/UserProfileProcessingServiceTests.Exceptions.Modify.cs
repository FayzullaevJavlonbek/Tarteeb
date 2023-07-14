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
            UserProfile randomUserProfile = CreateRandomUserProfile();
            var inputUserProfile = randomUserProfile;
            Guid inputUserProfileGuid = inputUserProfile.Id;

            var expectedUserProfileProcessingValidationException =
                new UserProfileProcessingDependencyValidationException(dependencyValidationException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyValidationException);
            
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
                service.RetrieveUserByIdAsync(inputUserProfileGuid), 
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserProfileProcessingValidationException))), 
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            UserProfile randomUserProfile = CreateRandomUserProfile();
            var inputUserProfile = randomUserProfile;
            Guid inputUserProfileGuid = inputUserProfile.Id;

            var expectedUserProfileProcessingDependencyException = 
                new UserProfileProcessingDependencyException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<UserProfile> modifyUserProfileTask =
                this.userProfileProcessingService.ModifyUserProfileAsync(inputUserProfile);

            UserProfileProcessingDependencyException actualUserProfileProcessingDependencyException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyException>(modifyUserProfileTask.AsTask);

            // then
            actualUserProfileProcessingDependencyException.Should().BeEquivalentTo(expectedUserProfileProcessingDependencyException);   

            this.userServiceMock.Verify(service => 
                service.RetrieveUserByIdAsync(inputUserProfileGuid), 
                    Times.Once); 

            this.loggingBrokerMock.Verify(broker => 
                broker.LogError(It.Is(SameExceptionAs(expectedUserProfileProcessingDependencyException))),
                    Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
