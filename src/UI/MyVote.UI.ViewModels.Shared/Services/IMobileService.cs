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
		void ResumeWithUrl(Uri uri);

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		Task<string> GenerateStorageAccessSignatureAsync();
    }
}
