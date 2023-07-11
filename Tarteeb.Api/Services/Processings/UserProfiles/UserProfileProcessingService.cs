//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Emails;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Services.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService : IUserProfileProcessingService
    {
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;

        public UserProfileProcessingService(
            IUserService userService,
            ILoggingBroker loggingBroker)
        {
            this.userService = userService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<UserProfile> RetrieveUserProfileByIdAsync(Guid userProfileId) =>
        TryCatch(async () =>
        {
            ValidateUserProfileId(userProfileId);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfileId);
            ValidateStorageUser(userProfileId, maybeUser);
            UserProfile populatedUserProfile = PopulateUserProfile(maybeUser);

            return populatedUserProfile;
        });

        public ValueTask<UserProfile> ModifyUserProfileAsync(UserProfile userProfile) =>
        TryCatch(async () =>
        {
            ValidateUserProfileOnModify(userProfile);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfile.Id);
            ValidateStorageUser(userProfile.Id, maybeUser);
            User populatedUser = PopulateUser(userProfile);
            User modifiedUser = await this.userService.ModifyUserAsync(populatedUser);
            UserProfile populatedUserProfile = PopulateUserProfile(modifiedUser);

            return populatedUserProfile;
        });
        

        private UserProfile PopulateUserProfile(User user)
        {
            return new UserProfile
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                GitHubUsername = user.GitHubUsername,
                TelegramUsername = user.TelegramUsername,
                TeamId = user.TeamId
            };
        }

        private User PopulateUser(UserProfile user)
        {
            return new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                GitHubUsername = user.GitHubUsername,
                TelegramUsername = user.TelegramUsername,
                TeamId = user.TeamId
            };
        }
    }
}
