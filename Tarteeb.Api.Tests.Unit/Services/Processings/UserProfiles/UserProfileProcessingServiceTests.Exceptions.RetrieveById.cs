//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var expectedUserProfileProcessingValidationException =
                new UserProfileProcessingDependencyValidationException(dependencyValidationException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask = 
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileGuid);

            UserProfileProcessingDependencyValidationException actualUserProfileProcessingDependencyValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyValidationException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserProfileProcessingValidationException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingValidationException))),Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var userProfileProcessingDependencyException =
                new UserProfileProcessingDependencyException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask =
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileGuid);

            UserProfileProcessingDependencyException actualUserProfileProcessingDependencyException =
                await Assert.ThrowsAsync<UserProfileProcessingDependencyException>(
                    retrieveUserProfileByIdTask.AsTask);

            // then
            actualUserProfileProcessingDependencyException.Should()
                .BeEquivalentTo(userProfileProcessingDependencyException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    userProfileProcessingDependencyException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccurresAndLogItAsync()
        {
            // given
            Guid randomUserProfileGuid = Guid.NewGuid();
            Guid inputUserProfileGuid = randomUserProfileGuid;

            var serviceException = new Exception();

            var failedUserProfileProcessingServiceException =
                new FailedUserProfileProcessingServiceException(serviceException);

            var expectedUserProfileProcessingServiceException =
                new UserProfileProcessingServiceException(failedUserProfileProcessingServiceException);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserProfile> retrieveUserProfileByIdTask =
                this.userProfileProcessingService.RetrieveUserProfileByIdAsync(inputUserProfileGuid);

            UserProfileProcessingServiceException actualUserProfileProcessingServiceException =
                await Assert.ThrowsAsync<UserProfileProcessingServiceException>(
                    retrieveUserProfileByIdTask.AsTask);

            // given
            actualUserProfileProcessingServiceException.Should()
                .BeEquivalentTo(expectedUserProfileProcessingServiceException);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileGuid), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProfileProcessingServiceException))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
