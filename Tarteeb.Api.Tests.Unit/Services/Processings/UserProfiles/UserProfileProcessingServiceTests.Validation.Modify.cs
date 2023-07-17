//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserProfileIsNullAndLogItAsync()
        {
            // given
            UserProfile nullUserProfile = null;
            var nullUserProfileProcessingException = new NullUserProfileProcessingException();

            var expectedUserProfileProcessingValidationException = 
                new UserProfileProcessingValidationException(nullUserProfileProcessingException);

            // when 
            ValueTask<UserProfile> modifyUserTask = 
                this.userProfileProcessingService.ModifyUserProfileAsync(nullUserProfile);
            
            UserProfileProcessingValidationException actualUserProfileProcessingValidationException =
                await Assert.ThrowsAsync<UserProfileProcessingValidationException>(modifyUserTask.AsTask);

            // then
            actualUserProfileProcessingValidationException.Should().BeEquivalentTo(expectedUserProfileProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserProfileProcessingValidationException))));

            this.userServiceMock.Verify(service => 
                service.RetrieveUserByIdAsync(It.IsAny<Guid>()), 
                    Times.Never);

            this.userServiceMock.Verify(service => 
                service.ModifyUserAsync(It.IsAny<User>()), 
                    Times.Never);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
