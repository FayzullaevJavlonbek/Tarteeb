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
        public async Task ShouldModifyUserProfile()
        {
            // given
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

            UserProfile inputUserProfile = randomUserProfile;
            UserProfile expectedUserProfile = inputUserProfile.DeepClone();

            User randomUser = ConvertToUser(randomUserProfile);
            User storageUser = randomUser;
            User inputUser = ConvertToUser(inputUserProfile);

            this.userServiceMock.Setup(service =>
                service.RetrieveUserByIdAsync(inputUser.Id))
                    .ReturnsAsync(storageUser);

            this.userServiceMock.Setup(service =>
                service.ModifyUserAsync(inputUser)).ReturnsAsync(inputUser);

            // when
            UserProfile actualUserProfile =
                await this.userProfileProcessingService
                    .ModifyUserProfileAsync(inputUserProfile);

            // then
            actualUserProfile.Should().BeEquivalentTo(expectedUserProfile);

            this.userServiceMock.Verify(service => 
                service.RetrieveUserByIdAsync(inputUser.Id), Times.Once);

            this.userServiceMock.Verify(service =>
                service.ModifyUserAsync(inputUser), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
