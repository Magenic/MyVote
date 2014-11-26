using MyVote.UI.Models;
using System;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
#endif

namespace MyVote.UI.Services
{
	public enum AuthenticationProvider
	{
		Facebook,
		Google,
		Microsoft,
		Twitter
	}

	public interface IMobileService
#if NETFX_CORE
		: IDisposable
#endif // NETFX_CORE
	{
#if ANDROID
		Task<string> AuthenticateAsync(Context context, AuthenticationProvider provider);
#else
		Task<string> AuthenticateAsync(AuthenticationProvider provider);
#endif

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Task<string> GenerateStorageAccessSignatureAsync(string resourceName);
	}
}
