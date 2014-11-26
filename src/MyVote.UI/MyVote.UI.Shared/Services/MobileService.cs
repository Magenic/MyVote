using Microsoft.WindowsAzure.MobileServices;
using MyVote.UI.Models;
using System;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
#if __MOBILE__
using MyVote.UI.Services;
#endif
#if __ANDROID__
using Android.Content;
#elif __IOS__
using MonoTouch.UIKit;
#endif

namespace MyVote.UI.Services
{
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public sealed class MobileService : IMobileService
	{
#if __MOBILE__
	    private IUIContext currentUIContext { get; set; }

	    public MobileService(IUIContext uiContext)
	    {
	        this.currentUIContext = uiContext;
	    }
#endif
		private readonly MobileServiceClient mobileService = new MobileServiceClient(
			new Uri("https://myvote.azure-mobile.net/", UriKind.Absolute),
			"ZvcAeJgeuBlZVTAiPYEhfDDloLsJUb20"
		);

		public async Task<string> AuthenticateAsync(AuthenticationProvider provider)
		{
#if __MOBILE__
		    var user = await this.mobileService.LoginAsync(currentUIContext.CurrentContext, ToMobileServiceProvider(provider));
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

#if NETFX_CORE || __MOBILE__
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
