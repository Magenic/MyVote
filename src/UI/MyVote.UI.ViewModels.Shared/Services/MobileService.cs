using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.Services
{
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class MobileService : IMobileService
    {
#if MOBILE
		private IUiContext currentUiContext { get; set; }

		public MobileService(IUiContext uiContext)
		{
			this.currentUiContext = uiContext;
		}
#endif // MOBILE

#if DEBUG_OFF && !MOBILE
		private readonly MobileServiceClient mobileService = new MobileServiceClient(
			"https://localhost:44305/");
#else
		private readonly MobileServiceClient mobileService = new MobileServiceClient(
			"https://myapp.azure-mobile.net/",
        #region Private Key
 "**Add Your Private AMS Key Here**"
        #endregion
);
#endif // DEBUG

		public async Task<string> AuthenticateAsync(AuthenticationProvider provider)
		{
#if MOBILE
			var user = await this.mobileService.LoginAsync(currentUiContext.CurrentContext, ToMobileServiceProvider(provider));
#else
			var user = await this.mobileService.LoginAsync(ToMobileServiceProvider(provider));
#endif // MOBILE
			return user.UserId;
		}

		public async Task<string> GenerateStorageAccessSignatureAsync()
		{
			var result = string.Empty;

			using (var client = new HttpClient())
			{
#if DEBUG_OFF && !MOBILE
				client.BaseAddress = new Uri("https://localhost:44305/");
#else
				client.BaseAddress = new Uri("https://myapp.azure-mobile.net/");
#endif // DEBUG

				var response = await client.GetAsync("api/SasGenerator").ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
                    result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				}
			}

			return result;
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

#if NETFX_CORE || MOBILE
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
#endif // NETFX_CORE || MOBILE

#if WINDOWS_PHONE_APP
		public void AuthenticationComplete(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
		{
			this.mobileService.LoginComplete(args);
		}
#endif // WINDOWS_PHONE_APP
	}
}
