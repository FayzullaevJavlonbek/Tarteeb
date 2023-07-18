﻿//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System;
using System.Threading.Tasks;
using Tarteeb.Api.Models.Foundations.Users;
using Tarteeb.Api.Models.Foundations.Users.Exceptions;
using Tarteeb.Api.Models.Processings.Users;
using Xeptions;

namespace Tarteeb.Api.Services.Processings.Users
{
    public partial class UserProcessingService
    {
        private delegate User ReturningUserFunction();
        private delegate ValueTask<Guid> ReturningUserIdFunction();

        private User TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return returningUserFunction();
            }
            catch (InvalidUserProcessingException invalidUserProcessingException)
            {
                throw CreateAndLogValidationException(invalidUserProcessingException);
            }
            catch (UserDependencyException userDependencyException)
            {
                throw CreateAndLogDependencyException(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                throw CreateAndLogDependencyException(userServiceException);
            }
            catch (Exception serviceException)
            {
                var failedUserProcessingServiceException =
                    new FailedUserProcessingServiceException(serviceException);

                throw CreateAndLogServiceException(failedUserProcessingServiceException);
            }
        }

        private async ValueTask<Guid> TryCatch(ReturningUserIdFunction returningUserFunctionGuid)
        {
            try
            {
                return await returningUserFunctionGuid();
            }
            catch (UserValidationException userValidationException) 
            {
                throw CreateAndLogDependencyValidationException(userValidationException);
            }
            catch (UserDependencyValidationException userDependencyValidationException)
            {
                throw CreateAndLogDependencyValidationException(userDependencyValidationException);
            }
            catch (UserDependencyException userDependencyException)
            {
                throw CreateAndLogDependencyException(userDependencyException);
            }
            catch (UserServiceException userServiceException)
            {
                throw CreateAndLogDependencyException(userServiceException);
            }
            catch (Exception serviceException)
            {
                var failedUserProcessingServiceException =
                    new FailedUserProcessingServiceException(serviceException);

                throw CreateAndLogServiceException(failedUserProcessingServiceException);
            }
        }

        private UserProcessingValidationException CreateAndLogValidationException(Xeption exception)
        {
            var userProcessingValidationException =
                new UserProcessingValidationException(exception);

            this.loggingBroker.LogError(userProcessingValidationException);

            return userProcessingValidationException;
        }

        private UserProcessingDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var userProcessingDependencyException =
                new UserProcessingDependencyException(exception.InnerException as Xeption);

            this.loggingBroker.LogError(userProcessingDependencyException);

            return userProcessingDependencyException;
        }

        private UserProcessingDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var userProcessingDependencyValidationException = 
                new UserProcessingDependencyValidationException(exception.InnerException as Xeption);

            this.loggingBroker.LogError(userProcessingDependencyValidationException);

            return userProcessingDependencyValidationException;
        }

        private Exception CreateAndLogServiceException(Xeption expception)
        {
            var userProcessingServiceException =
                new UserProcessingServiceException(expception);

            this.loggingBroker.LogError(userProcessingServiceException);

            return userProcessingServiceException;
        }
    }
}
