//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowVlidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidUserProfileId = Guid.Empty;
            var invalidUserProfileProcessingException = new InvalidUserProfileProcessingException();

            invalidUserProfileProcessingException.AddData(
                key: nameof(UserProfile.Id),
                values: "Id is required");

            var expectedUserProfileProcessingValidationException =
                new UserProfileProcessingValidationException(invalidUserProfileProcessingException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask = 
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(invalidUserProfileId);

            UserProfileProcessingValidationException actualUserProfileProcesingValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingValidationException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcesingValidationException.Should().BeEquivalentTo(expectedUserProfileProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingValidationException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserDoesNotExistAndLogItAsync()
        {
            // given
            Guid userRandomProfileId = Guid.NewGuid();
            Guid inputUserProfileId = Guid.NewGuid();
            User noUser = null;

            var notFoundUserException = new NotFoundUserException();
 
            var expectedUserProfileProcessingValidationException = 
                new UserProfileProcessingValidationException(notFoundUserException);

            this.userServiceMock.Setup(service =>
                service.RemoveUserByIdAsync(inputUserProfileId))
                    .ReturnsAsync(noUser);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask =
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileId);

            UserProfileProcessingValidationException actualUserProfileProcessingValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingValidationException>(retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcessingValidationException.Should().BeEquivalentTo(expectedUserProfileProcessingValidationException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileId), Times.Once);

            this.loggingBrokerMock.Verify(service =>
                service.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingValidationException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
