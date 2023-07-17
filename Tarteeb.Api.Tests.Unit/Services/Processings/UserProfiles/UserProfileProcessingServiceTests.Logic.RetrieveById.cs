//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Teams;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveUserByIdAsync()
        {
            // given
            Guid randomUserProfileId = Guid.NewGuid();
            Guid inputUserProfileId = randomUserProfileId;
            dynamic randomUserProfileProperties = CreateRandomUserProfileProperties();

            var randomUser = new User
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

            var randomUserProfile = new UserProfile
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

            User returnedUser = randomUser;
            UserProfile expectedUserProfile = randomUserProfile.DeepClone();

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUserProfileId))
                    .ReturnsAsync(returnedUser);

            // when
            UserProfile actualUserProfile = 
                await this.userProfileProcessingService
                    .RetrieveUserProfileByIdAsync(inputUserProfileId);

            // then
            actualUserProfile.Should().BeEquivalentTo(expectedUserProfile);
            
            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUserProfileId), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
