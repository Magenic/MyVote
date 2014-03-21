using System.Runtime.Serialization;

namespace MyVote.AppServer.Auth
{
    [DataContract]
    public class JsonWebTokenEnvelope
    {
        [DataMember(Name = "typ")]
        public string Type { get; set; }

        [DataMember(Name = "alg")]
        public string Algorithm { get; set; }

        [DataMember(Name = "kid")]
        public int KeyId { get; set; }
    }
}