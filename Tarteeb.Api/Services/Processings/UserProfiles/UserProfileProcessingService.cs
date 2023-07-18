//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Tarteeb.Api.Brokers.DateTimes;
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
            var retrievedUser = await this.userService.RetrieveUserByIdAsync(userProfileId);

            return MapToUserProfile(retrievedUser); ;
        });

        private static Func<User, UserProfile> AsUserProfile =>
            user => MapToUserProfile(user);

        public ValueTask<UserProfile> ModifyUserProfileAsync(UserProfile userProfile) =>
        TryCatch(async () =>
        {
            ValidateUserProfileOnModify(userProfile);
            var maybeUser = await this.userService.RetrieveUserByIdAsync(userProfile.Id);
            ValidateStorageUser(userProfile.Id, maybeUser);
            User populatedUser = MapToUser(userProfile);

            populatedUser.CreatedDate = maybeUser.CreatedDate;
            populatedUser.Password = maybeUser.Password;
            populatedUser.UpdatedDate = this.dateTimeBroker.GetCurrentDateTime();
            
            User modifiedUser = await this.userService.ModifyUserAsync(populatedUser);
            UserProfile populatedUserProfile = MapToUserProfile(modifiedUser);

            return populatedUserProfile;
        });
        

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

        private static User MapToUser(UserProfile user)
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
