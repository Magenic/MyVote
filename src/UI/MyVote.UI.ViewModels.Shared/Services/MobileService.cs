using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

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
            "https://myapp.azurewebsites.net/");
        #region Private Key
 //"heZNtCPzxAcMZxWlLRzxfuhVQPLaeB41"
        #endregion
//);
#endif // DEBUG

		public async Task<string> AuthenticateAsync(AuthenticationProvider provider)
		{
#if IOS
            AppDelegate.ResumeWithURL = url => url.Scheme == "commagenicmyvote" && this.mobileService.ResumeWithURL(url);
#endif
#if MOBILE
			var user = await this.mobileService.LoginAsync(currentUiContext.CurrentContext, ToMobileServiceProvider(provider), "commagenicmyvote");
#else
            var user = await this.mobileService.LoginAsync(ToMobileServiceProvider(provider), "commagenicmyvote");
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
                client.BaseAddress = new Uri("http://myapp.azurewebsites.net/");
#endif // DEBUG

				var response = await client.GetAsync("api/SasGenerator?ZUMO-API-VERSION=2.0.0").ConfigureAwait(false);

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

		public void ResumeWithUrl(Uri uri)
		{
			this.mobileService.ResumeWithURL(uri);
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
	}
}
