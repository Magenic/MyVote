MyVote
------
MyVote is an app developed by [Magenic](http://www.magenic.com) as a comprehensive demo for the [Modern Apps Live!](http://www.modernappslive.com) conference held twice a year as part of [Live! 360 Dev](http://www.live360events.com).

The code in this repo has been scrubbed to remove sensitive key values for encryption and services. The following is a list of changes you'll need to make to insert your own keys into the codebase:

**src/MyVote.UI.ViewModels.Shared/Services/MobileService.cs**
````
	private readonly MobileServiceClient mobileService = new MobileServiceClient(
		new Uri("MyUrl", UriKind.Absolute),
		"MyKey"
	);
````
	
* Replace "MyUrl" with your Azure Mobile Services url.
* Replace "MyKey" with your Azure Mobile Services application key.

````
	client.BaseAddress = new Uri("https://myapp.azure-mobile.net/");
````

* Replace "myapp" with your Azure Mobile Services url.
	
**src/Services/MyVote.Services.AppServer/Web.config**

* Replace the value of the zumoMaster appSettings entry with your Azure Mobile Service master key.

**src/Services/MyVote.Services.AppServer/Web.Release.config**

* Replace the connection string with your production environment connection strings.

**src/Services/MyVote.Services.AppServer/App_Start/WebApiConfig.cs**

* Update and add urls for your Azure Website and domain name.

**src/UI/MyVote.UI.ViewModels.Shared/Bootstrapper.cs -> Configure()**

* Replace "DataPortalUrl" values with your server urls.

**src/UI/MyVote.UI.ViewModels.Shared/ViewModels/PollImageViewModelBase.cs**

* Replace "PollPicturesUrlBase" with your Azure Storage url.

**src/UI/MyVote.UI.Web/Web.config**

* Replace the value of the zumoKey entry with your Azure Mobile Service application key.

**src/UI/MyVote.UI.Web/Web.Release.config and Web.Staging.config**

* Replace the value of the apiUrl appSettings entries with the URLs of your	Production and Staging cloud services.

**src/Services/MyVote.Services.MobileServices/Web.config**

* Replace "StorageConnectionString" with your Azure Storage url.

**src/Services/MyVote.Services.AppServer/Web.myvoteapi.config**

* Replace connection string with your production environment connection string.

**src/Services/MyVote.Services.AppServer/Controller/PollImageController.cs**

* Update "STORAGE_CONNECTION_STRING" with your Azure Storage Account Name and Account Key.