using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Google.AccessTokenAuthorization
{

    [Serializable]
    public class UserInfoException : Exception
    {
        public UserInfoException() { }
        public UserInfoException(string message) : base(message) { }
        public UserInfoException(string message, Exception inner) : base(message, inner) { }
        protected UserInfoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
