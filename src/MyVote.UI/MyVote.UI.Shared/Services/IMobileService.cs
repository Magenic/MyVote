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

	public interface IMobileService
#if NETFX_CORE || __MOBILE__
		: System.IDisposable
#endif // NETFX_CORE
	{
		Task<string> AuthenticateAsync(AuthenticationProvider provider);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Task<string> GenerateStorageAccessSignatureAsync(string resourceName);
	}
}