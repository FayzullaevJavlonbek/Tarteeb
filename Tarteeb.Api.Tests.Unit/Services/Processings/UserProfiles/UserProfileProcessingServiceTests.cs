﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Linq;
using System.Linq.Expressions;
using Tarteeb.Api.Brokers.DateTimes;
using Moq;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.UserProfiles;
using Tarteeb.Api.Models.Processings.UserProfiles.Exceptions;
using Tarteeb.Api.Services.Foundations.Users;
using Tarteeb.Api.Services.Processings.UserProfiles;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.UserProfiles
{
    public partial class UserProfileProcessingServiceTests
    {
        private readonly Mock<IUserService> userServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly UserProfileProcessingService userProfileProcessingService;

        public UserProfileProcessingServiceTests()
        {
            this.userServiceMock = new Mock<IUserService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.userProfileProcessingService = new UserProfileProcessingService(
                userService: this.userServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }

        public static TheoryData<Xeption> UserDependencyExceptions()
        {
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new UserDependencyException(someInnerException),
                new UserServiceException(someInnerException)
            };
        }

        public static TheoryData<Xeption> UserDependencyValidationExceptions()
        {
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new UserValidationException(someInnerException)
            };
        }

        public static TheoryData<Xeption> UserProfileDependencyExceptions()
        {
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new UserProfileProcessingDependencyException(someInnerException),
                new UserProfileProcessingServiceException(someInnerException)
            };
        }

        private static IQueryable<User> MapToUsers(dynamic[] userProperties)
        {
            return userProperties.Select(userProperty => new User
            {
                Id = userProperty.Id,
                FirstName = userProperty.FirstName,
                LastName = userProperty.LastName,
                PhoneNumber = userProperty.PhoneNumber,
                Email = userProperty.Email,
                BirthDate = userProperty.BirthDate,
                IsActive = userProperty.IsActive,
                IsVerified = userProperty.IsVerified,
                GitHubUsername = userProperty.GitHubUsername,
                TelegramUsername = userProperty.TelegramUsername,
                TeamId = userProperty.TeamId
            }).AsQueryable();
        }

        private static IQueryable<UserProfile> MapToUsersPropfiles(dynamic[] userProperties)
        {
            return userProperties.Select(userProperty => new UserProfile
            {
                Id = userProperty.Id,
                FirstName = userProperty.FirstName,
                LastName = userProperty.LastName,
                PhoneNumber = userProperty.PhoneNumber,
                Email = userProperty.Email,
                BirthDate = userProperty.BirthDate,
                IsActive = userProperty.IsActive,
                IsVerified = userProperty.IsVerified,
                GitHubUsername = userProperty.GitHubUsername,
                TelegramUsername = userProperty.TelegramUsername,
                TeamId = userProperty.TeamId
            }).AsQueryable();
        }

        private static dynamic[] CreateRandomUsersProperties()
        {
            return Enumerable.Range(0, GetRandomNumber()).Select(
                items => CreateRandomUserProfileProperties()).ToArray();
        }

        private static dynamic CreateRandomUserProfileProperties()
        {
            return new
            {
                Id = Guid.NewGuid(),
                FirstName = GetRandomString(),
                LastName = GetRandomString(),
                PhoneNumber = GetRandomString(),
                Email = GetRandomString(),
                BirthDate = GetRandomDateTimeOffset(),
                IsActive = GetRandomBool(),
                IsVerified = GetRandomBool(),
                GitHubUsername = GetRandomString(),
                TelegramUsername = GetRandomString(),
                TeamId = Guid.NewGuid()
            };
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static bool GetRandomBool() =>
            new Random().NextDouble() is >= 0.5;

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private User CreateRandomUser() =>
            this.CreateUserFiller(GetRandomDateTimeOffset()).Create();

        private IQueryable<User> CreateRandomUsers() =>
            this.CreateUserFiller(GetRandomDateTimeOffset()).Create(count: GetRandomNumber())
                .ToList().AsQueryable();

        private Filler<User> CreateUserFiller(DateTimeOffset dates)
        {
            var filler = new Filler<User>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }

        private UserProfile CreateRandomUserProfile() =>
            this.CreateUserProfileFiller(GetRandomDateTimeOffset()).Create();

        private Filler<UserProfile> CreateUserProfileFiller(DateTimeOffset dates)
        {
            var filler = new Filler<UserProfile>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }

        private static Expression<Func<User, bool>> SameUserAs(User expectedUser)
        {
            return actualUser =>
                    actualUser.Id == expectedUser.Id
                    && actualUser.FirstName == expectedUser.FirstName
                    && actualUser.LastName == expectedUser.LastName
                    && actualUser.PhoneNumber == expectedUser.PhoneNumber
                    && actualUser.Email == expectedUser.Email
                    && actualUser.BirthDate == expectedUser.BirthDate
                    && actualUser.CreatedDate == expectedUser.CreatedDate
                    && actualUser.UpdatedDate == expectedUser.UpdatedDate
                    && actualUser.Password == expectedUser.Password
                    && actualUser.IsActive == expectedUser.IsActive
                    && actualUser.IsVerified == expectedUser.IsVerified
                    && actualUser.GitHubUsername == expectedUser.GitHubUsername
                    && actualUser.TelegramUsername == expectedUser.TelegramUsername
                    && actualUser.TeamId == expectedUser.TeamId;
        }
    }
}
