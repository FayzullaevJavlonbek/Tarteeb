//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Moq;
using System;
using System.Linq.Expressions;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.UserProfiles;
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
        private readonly UserProfileProcessingService userProfileProcessingService;

        public UserProfileProcessingServiceTests()
        {
            this.userServiceMock = new Mock<IUserService>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.userProfileProcessingService = new UserProfileProcessingService(
                userService: this.userServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private User CreateRandomUser() =>
            this.CreateUserFiller(GetRandomDateTimeOffset()).Create();

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
