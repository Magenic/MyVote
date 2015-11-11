using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MyVote.UI.Services
{
	public enum AuthenticationProvider
	{
		Facebook,
		Google,
		Microsoft,
		Twitter
	}

    public interface IMobileService : IDisposable
    {
		Task<string> AuthenticateAsync(AuthenticationProvider provider);

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Task<string> GenerateStorageAccessSignatureAsync();

#if WINDOWS_PHONE_APP
		void AuthenticationComplete(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args);
#endif // WINDOWS_PHONE_APP
    }
}
