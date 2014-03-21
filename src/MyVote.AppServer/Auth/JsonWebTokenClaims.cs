using System;
using System.Runtime.Serialization;

namespace MyVote.AppServer.Auth
{
    [DataContract]
    public class JsonWebTokenClaims
    {
        // NOTE: spec indicates integer, but Azumore Mobile JWT includes decimal seconds
        [DataMember(Name = "exp")]
        private double ExpUnixTime { get; set; }

        private DateTime? _expiration = null;
        public DateTime Expiration
        {
            get
            {
                if (_expiration == null)
                {
                    _expiration = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ExpUnixTime);
                }
                return (DateTime)_expiration;
            }
        }

        [DataMember(Name = "iss")]
        public string Issuer { get; set; }

        [DataMember(Name = "aud")]
        public string Audience { get; set; }

        [DataMember(Name = "uid")]
        public string UserId { get; set; }

        [DataMember(Name = "ver")]
        public int Version { get; set; }

        [DataMember(Name = "urn:microsoft:appuri")]
        public string ClientIdentifier { get; set; }

        [DataMember(Name = "urn:microsoft:appid")]
        public string AppId { get; set; }
    }
}