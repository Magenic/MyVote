using MyVote.UI.Models;
using MyVote.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class MobileServiceMock : IMobileService
	{
		public Func<AuthenticationProvider, string> AuthenticateWithProviderDelegate { get; set; }
		public Task<string> AuthenticateAsync(AuthenticationProvider provider)
		{
			if (AuthenticateWithProviderDelegate != null)
			{
				return Task.FromResult<string>(AuthenticateWithProviderDelegate(provider));
			}
			else
			{
				return Task.FromResult<string>(null);
			}
		}

		public Func<string> GenerateStorageAccessSignatureDelegate { get; set; }
		public Task<string> GenerateStorageAccessSignatureAsync(string resourceName)
		{
			if (GenerateStorageAccessSignatureDelegate != null)
			{
				return Task.FromResult<string>(GenerateStorageAccessSignatureDelegate());
			}
			else
			{
				return Task.FromResult<string>((string)null);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		public void Dispose()
		{
		}
	}
}
