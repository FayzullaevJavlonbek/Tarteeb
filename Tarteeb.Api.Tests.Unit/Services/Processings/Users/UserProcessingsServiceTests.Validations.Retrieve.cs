//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using FluentAssertions;
using Moq;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Processings.Users;
using Xunit;

namespace Tarteeb.Api.Tests.Unit.Services.Processings.Users
{
    public partial class UserProcessingsServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnUpsertIfEmailAndPasswordAreInvalidAndLogItAsync()
        {
            //given
            string invalidEmail = string.Empty;
            string invalidPassword = string.Empty;
            var invalidUserProcessingException = new InvalidUserProcessingException();

            invalidUserProcessingException.AddData(
                key: nameof(User.Email),
                values: "Text is required");

            invalidUserProcessingException.AddData(
                key: nameof(User.Password),
                values: "Text is required");

            var expectedUserProcessingValidationException =
                new UserProcessingValidationException(invalidUserProcessingException);

            //when
            Action retrieveUserByAction = () =>
                this.userProcessingsService.RetrieveUserByCredentials(invalidEmail, invalidPassword);

            UserProcessingValidationException actualUserProcessingValidationException =
                 Assert.Throws<UserProcessingValidationException>(retrieveUserByAction);

            //then
            actualUserProcessingValidationException.Should().BeEquivalentTo(
               expectedUserProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserProcessingValidationException))), Times.Once);

            this.userServiceMock.Verify(service =>
                service.RetrieveAllUsers(), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.userServiceMock.VerifyNoOtherCalls();
        }
    }
}
