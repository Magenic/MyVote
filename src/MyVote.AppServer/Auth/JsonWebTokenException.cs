using System;

namespace MyVote.AppServer.Auth
{
    public class JsonWebTokenException : Exception
    {
        public JsonWebTokenException(string message)
            : base(message)
        {
        }
    }
}