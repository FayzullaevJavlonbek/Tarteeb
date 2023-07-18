//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class UserProfileProcessingDependencyValidationException : Xeption
    {
        public UserProfileProcessingDependencyValidationException(Xeption innerException)
            : base(message: "User profile dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
