using System;

namespace MyVote.Services.AppServer.Auth
{
    public class JsonWebTokenException : Exception
    {
        public JsonWebTokenException(string message)
            : base(message)
        {
        }
    }
}