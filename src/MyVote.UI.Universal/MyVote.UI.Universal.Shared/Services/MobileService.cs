using Microsoft.WindowsAzure.MobileServices;
using MyVote.UI.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
#if ANDROID
using Android.Content;
#endif

namespace MyVote.UI.Services
{
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class MobileService : IMobileService
	{
		private readonly MobileServiceClient mobileService = new MobileServiceClient(
			new Uri("MyUrl", UriKind.Absolute),
			"MyKey"
		);

#if ANDROID
		public async Task<string> AuthenticateAsync(Context context, AuthenticationProvider provider)
#else
		public async Task<string> AuthenticateAsync(AuthenticationProvider provider)
#endif
		{
#if ANDROID
		    var user = await this.mobileService.LoginAsync(context, ToMobileServiceProvider(provider));
#else
			var user = await this.mobileService.LoginAsync(ToMobileServiceProvider(provider));
#endif
			return user.UserId;
		}

		public async Task<string> GenerateStorageAccessSignatureAsync(string resourceName)
		{
			var activeUser = new ActiveUsers
			{
				ContainerName = "pollpictures",
				ResourceName = resourceName
			};
			var table = this.mobileService.GetTable<ActiveUsers>();

			await table.InsertAsync(activeUser);

			return activeUser.SharedAccessSignature;
		}

		private static MobileServiceAuthenticationProvider ToMobileServiceProvider(AuthenticationProvider provider)
		{
			switch (provider)
			{
				case AuthenticationProvider.Facebook:
					return MobileServiceAuthenticationProvider.Facebook;

				case AuthenticationProvider.Google:
					return MobileServiceAuthenticationProvider.Google;

				case AuthenticationProvider.Twitter:
					return MobileServiceAuthenticationProvider.Twitter;

				case AuthenticationProvider.Microsoft:
				default:
					return MobileServiceAuthenticationProvider.MicrosoftAccount;
			}
		}

#if NETFX_CORE
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing) 
			{
				if (this.mobileService != null)
				{
					this.mobileService.Dispose();
				}
			}
		}
#endif // NETFX_CORE
	}
}
