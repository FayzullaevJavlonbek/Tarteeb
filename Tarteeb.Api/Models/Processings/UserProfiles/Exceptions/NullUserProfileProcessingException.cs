using Xeptions;

namespace Tarteeb.Api.Models.Processings.UserProfiles.Exceptions
{
    public class NullUserProfileProcessingException : Xeption
    {
        public NullUserProfileProcessingException()
            : base("User Profile is null.")
        {  }
    }
}
