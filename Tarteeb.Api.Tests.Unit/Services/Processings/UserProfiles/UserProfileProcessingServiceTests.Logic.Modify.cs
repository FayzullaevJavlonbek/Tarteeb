//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserProfileAsync()
        {
            // given
            var randomDateTimeOffset = GetRandomDateTimeOffset();
            dynamic randomUserProfileProperties = CreateRandomUserProfileProperties();

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

            var randomUser = new User
            {
                Id = randomUserProfileProperties.Id,
                FirstName = randomUserProfileProperties.FirstName,
                LastName = randomUserProfileProperties.LastName,
                PhoneNumber = randomUserProfileProperties.PhoneNumber,
                Email = randomUserProfileProperties.Email,
                UpdatedDate = randomDateTimeOffset,
                BirthDate = randomUserProfileProperties.BirthDate,
                IsActive = randomUserProfileProperties.IsActive,
                IsVerified = randomUserProfileProperties.IsVerified,
                GitHubUsername = randomUserProfileProperties.GitHubUsername,
                TelegramUsername = randomUserProfileProperties.TelegramUsername,
                TeamId = randomUserProfileProperties.TeamId
            };

            UserProfile inputUserProfile = randomUserProfile;
            UserProfile expectedUserProfile = inputUserProfile.DeepClone();

            User storageUser = randomUser;
            User inputUser = randomUser;
            User modifiedUser = inputUser;

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Returns(randomDateTimeOffset);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(It.Is(SameUserAs(inputUser))))
                    .ReturnsAsync(modifiedUser);

            // when
            UserProfile actualUserProfile =
                await this.userProfileProcessingService
                    .ModifyUserProfileAsync(inputUserProfile);

            // then
            actualUserProfile.Should().BeEquivalentTo(expectedUserProfile);

            this.userServiceMock.Verify(service =>
                service.RetrieveUserByIdAsync(inputUser.Id), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(It.Is(SameUserAs(inputUser))), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
