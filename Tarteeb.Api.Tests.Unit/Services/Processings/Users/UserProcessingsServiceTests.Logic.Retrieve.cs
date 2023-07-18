//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Linq;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        [Fact]
        public void ShoulRetrieveUserByCredentails()
        {
            // given
            string randomString = GetRandomString();
            string inputEmail = randomString;
            string inputPassword = randomString;

            User randomUser = CreateRandomUserWithCredentials(
                inputEmail,
                inputPassword);

            User existingUser = randomUser;
            User expectedUser = existingUser.DeepClone();
            IQueryable<User> randomUsers = CreateRandomUsersIncluding(existingUser);
            IQueryable<User> retrievedUsers = randomUsers;

            this.userServiceMock.Setup(service =>
                service.RetrieveAllUsers()).Returns(retrievedUsers);

            // when
            User actualUser = userProcessingsService.
                RetrieveUserByCredentials(inputEmail, inputPassword);

            // then
            actualUser.Should().BeEquivalentTo(existingUser);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
