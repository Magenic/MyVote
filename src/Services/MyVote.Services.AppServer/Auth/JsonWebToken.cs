using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MyVote.Services.AppServer.Auth
{
    // From Live Services SDK samples
    // https://github.com/liveservices/LiveSDK/tree/master/Samples/Asp.net/AuthenticationTokenSample

    // Reference: http://tools.ietf.org/search/draft-jones-json-web-token-00
    //
    // JWT is made up of 3 parts: Envelope, Claims, Signature.
    // - Envelope - specifies the token type and signature algorithm used to produce 
    //       signature segment. This is in JSON format
    // - Claims - specifies claims made by the token. This is in JSON format
    // - Signature - Cryptographic signature use to maintain data integrity.
    // 
    // To produce a JWT token:
    // 1. Create Envelope segment in JSON format
    // 2. Create Claims segment in JSON format
    // 3. Create signature
    // 4. Base64url encode each part and append together separated by "." 

    public class JsonWebToken
    {
        private static readonly UTF8Encoding _UTF8Encoder = new UTF8Encoding(true, true);
        private static readonly SHA256Managed _Sha256Provider = new SHA256Managed();
        private static readonly JsonSerializer _JsonSerializer = new JsonSerializer();

        private readonly string _claimsTokenSegment;
        public JsonWebTokenClaims Claims { get; private set; }

        private readonly string _envelopeTokenSegment;
        public JsonWebTokenEnvelope Envelope { get; private set; }

        private readonly string _signatureKey;
        public string Signature { get; private set; }

        public bool IsExpired { get { return Claims.Expiration <= DateTime.UtcNow; } }

        public JsonWebToken(string token, Dictionary<int, string> keyIdsKeys)
        {
            ThrowOnInvalidTokenFormat(token);
            
            string[] tokenSegments = SplitToken(token);

            // Decode and deserialize the claims
            _claimsTokenSegment = tokenSegments[1];
            Claims = GetClaimsFromTokenSegment(_claimsTokenSegment);

            // Decode and deserialize the envelope
            _envelopeTokenSegment = tokenSegments[0];
            Envelope = GetEnvelopeFromTokenSegment(_envelopeTokenSegment);

            // Get the signature
            Signature = tokenSegments[2];

            // Ensure that the tokens KeyId exists in the secret keys list
            if (!keyIdsKeys.ContainsKey(Envelope.KeyId))
            {
                throw new JsonWebTokenException(string.Format("Could not find key with id {0}", Envelope.KeyId));
            }
            _signatureKey = keyIdsKeys[Envelope.KeyId];
        }

        public void Validate(bool validateExpiration = false)
        {
            ValidateEnvelope(Envelope);
            ValidateSignature();

            if (validateExpiration && this.IsExpired)
            {
                throw new JsonWebTokenException("Token is expired.");
            }
        }

        public static byte[] Base64UrlDecode(string encodedSegment)
        {
            string s = encodedSegment;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new JsonWebTokenException("Illegal base64url string");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public static string Base64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        private static JsonWebTokenClaims GetClaimsFromTokenSegment(string claimsTokenSegment)
        {
            byte[] claimsData = Base64UrlDecode(claimsTokenSegment);
            using (var memoryStream = new MemoryStream(claimsData))
            {
                var reader = new JsonTextReader(new StreamReader(memoryStream));
                return _JsonSerializer.Deserialize<JsonWebTokenClaims>(reader);
            }
        }

        private static JsonWebTokenEnvelope GetEnvelopeFromTokenSegment(string envelopeTokenSegment)
        {
            byte[] envelopeData = Base64UrlDecode(envelopeTokenSegment);
            using (var memoryStream = new MemoryStream(envelopeData))
            {
                var reader = new JsonTextReader(new StreamReader(memoryStream));
                return _JsonSerializer.Deserialize<JsonWebTokenEnvelope>(reader);
            }
        }

        /// <summary>
        /// Throw if token violates expected token format: Envelope.Claims.Signature
        /// </summary>
        private static void ThrowOnInvalidTokenFormat(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new JsonWebTokenException("Token is empty or null.");

            string[] segments = SplitToken(token);

            if (segments.Length != 3)
                throw new JsonWebTokenException("Invalid token format. Expected Envelope.Claims.Signature");

            if (string.IsNullOrEmpty(segments[0]))
                throw new JsonWebTokenException("Invalid token format. Envelope must not be empty");

            if (string.IsNullOrEmpty(segments[1]))
                throw new JsonWebTokenException("Invalid token format. Claims must not be empty");

            if (string.IsNullOrEmpty(segments[2]))
                throw new JsonWebTokenException("Invalid token format. Signature must not be empty");
        }

        private static string[] SplitToken(string token)
        {
            return token.Split('.');
        }

        private static void ValidateEnvelope(JsonWebTokenEnvelope envelope)
        {
            if (envelope == null) throw new ArgumentNullException("envelope");

            if (envelope.Type != "JWT")
                throw new JsonWebTokenException("Unsupported token type");

            if (envelope.Algorithm != "HS256")
                throw new JsonWebTokenException("Unsupported crypto algorithm");
        }

        private void ValidateSignature()
        {
            // Derive signing key, Signing key = SHA256(secret + "JWTSig")
            //Note: It appears v2 tokens signed by MSFT from AMS no longer append the "JWTSig" string to the master key. Using this will cause the validation to fail.
            //byte[] bytes = _UTF8Encoder.GetBytes(_signatureKey + "JWTSig");
            byte[] bytes = _UTF8Encoder.GetBytes(_signatureKey);
            byte[] signingKey = _Sha256Provider.ComputeHash(bytes);

            // To Validate:
            // 
            // 1. Take the bytes of the UTF-8 representation of the JWT Claim
            //  Segment and calculate an HMAC SHA-256 MAC on them using the
            //  shared key.
            //
            // 2. Base64url encode the previously generated HMAC as defined in this
            //  document.
            //
            // 3. If the JWT Crypto Segment and the previously calculated value
            //  exactly match in a character by character, case sensitive
            //  comparison, then one has confirmation that the key was used to
            //  generate the HMAC on the JWT and that the contents of the JWT
            //  Claim Segment have not be tampered with.
            //
            // 4. If the validation fails, the token MUST be rejected.

            // UFT-8 representation of the JWT envelope.claim segment
            byte[] input = _UTF8Encoder.GetBytes(_envelopeTokenSegment + "." + _claimsTokenSegment);

            // calculate an HMAC SHA-256 MAC
            using (var hashProvider = new HMACSHA256(signingKey))
            {
                byte[] myHashValue = hashProvider.ComputeHash(input);
                string base64UrlEncodedHash = Base64UrlEncode(myHashValue);
                if (base64UrlEncodedHash != Signature)
                    throw new JsonWebTokenException("Signature does not match.");
            }
        }
    }
}