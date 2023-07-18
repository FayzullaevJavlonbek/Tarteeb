//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.DateTimes;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Services.Foundations.Users;

namespace Tarteeb.Api.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingService : IUserProfileProcessingService
    {
        private readonly IUserService userService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserProfileProcessingService(
            IUserService userService,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.userService = userService;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public IQueryable<UserProfile> RetrieveAllUserProfiles() =>
        TryCatch(() =>
        {
            IQueryable<User> users =
                this.userService.RetrieveAllUsers();

            return users.Select(AsUserProfile).AsQueryable();
        });

        public ValueTask<UserProfile> RetrieveUserProfileByIdAsync(Guid userProfileId) =>
        TryCatch(async () =>
        {
            ValidateUserProfileId(userProfileId);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfileId);
            ValidateStorageUser(userProfileId, maybeUser);
            UserProfile mappedUserProfile = MapToUserProfile(maybeUser);

            return mappedUserProfile;
        });

        public ValueTask<UserProfile> ModifyUserProfileAsync(UserProfile userProfile) =>
        TryCatch(async () =>
        {
            ValidateUserProfileOnModify(userProfile);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfile.Id);
            ValidateStorageUser(userProfile.Id, maybeUser);
            ModifyUserProperties(maybeUser, userProfile);
            User modifiedUser = await this.userService.ModifyUserAsync(maybeUser);

            return MapToUserProfile(modifiedUser);
        });

        private static Func<User, UserProfile> AsUserProfile =>
           user => MapToUserProfile(user);

        private static UserProfile MapToUserProfile(User user)
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

        private void ModifyUserProperties(User user, UserProfile userProfile)
        {
            user.Id = userProfile.Id;
            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.PhoneNumber = userProfile.PhoneNumber;
            user.Email = userProfile.Email;
            user.BirthDate = userProfile.BirthDate;
            user.IsActive = userProfile.IsActive;
            user.IsVerified = userProfile.IsVerified;
            user.GitHubUsername = userProfile.GitHubUsername;
            user.TelegramUsername = userProfile.TelegramUsername;
            user.TeamId = userProfile.TeamId;
            user.UpdatedDate = this.dateTimeBroker.GetCurrentDateTime();
        }

    }
}
