//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Processings.Users;
using Xeptions;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        [Theory]
        [MemberData(nameof(UserDependencyExceptions))]
        public void ShoudThrowDependencyExceptionOnRetrieveIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            string someString = GetRandomString();

            var expectedUserProcessingDependencyException =
                new UserProcessingDependencyException(dependencyException.InnerException as Xeption);

            this.userServiceMock.Setup(service => service.RetrieveAllUsers())
                .Throws(dependencyException);

            // when
            Action retrieveUserByAction = () =>
                this.userProcessingsService.RetrieveUserByCredentials(email: someString, password: someString);

            UserProcessingDependencyException actualUserProcessingDependencyException =
                Assert.Throws<UserProcessingDependencyException>(retrieveUserByAction);

            // then
            actualUserProcessingDependencyException.Should().BeEquivalentTo(
                expectedUserProcessingDependencyException);

            this.userServiceMock.Verify(service => service.RetrieveAllUsers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingDependencyException))),
                        Times.Once);

            this.userServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
