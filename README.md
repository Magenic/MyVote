MyVote
------
MyVote is an app developed by [Magenic](http://www.magenic.com) as a comprehensive demo for the [Modern Apps Live!](http://www.modernappslive.com) conference held twice a year as part of [Live! 360 Dev](http://www.live360events.com).

The code in this repo has been scrubbed to remove sensitive key values for encryption and services. The following is a list of changes you'll need to make to insert your own keys into the codebase:

**src/MyVote.UI.W8/Services/MobileService.cs**
````
	private readonly MobileServiceClient mobileService = new MobileServiceClient(
		new Uri("MyUrl", UriKind.Absolute),
		"MyKey"
	);
````
	
* Replace "MyUrl" with your Azure Mobile Services url.
* Replace "MyKey" with your Azure Mobile Services application key.
	
**src/MyVote.AppServer/Web.config**

* Replace the value of the zumoMaster appSettings entry with your Azure Mobile Service master key.

**src/MyVote.AppServer/Web.Release.config and Web.Staging.config**

* Replace the connection string with your production and staging environment connection strings.

**src/MyVote.AzureService/ServiceConfiguration.Cloud.cscfg**

* If you want to enable remote desktop access to your Cloud Service VM, follow the instructions found [here](http://msdn.microsoft.com/en-us/library/windowsazure/hh124130.aspx). Set the	thumbprint value in the Certificate tag.

**src/MyVote/AzureService/Profiles**

* To publish the Cloud Service to Azure, configure it in the Azure Management Portal, download the Publish profile, and import it into this project.

**src/MyVote.Client.W8/app.xaml.cs and src/MyVote.Client.Wp8/Bootstrapper.cs -> Configure()**

* Replace Csla.ApplicationContext.DataPortalUrlString values with your server urls.

**src/MyVote.Client.Web/Web.config**

* Replace the value of the zumoKey entry with your Azure Mobile Service application key.

**src/MyVote.Client.Web/Web.Release.config and Web.Staging.config**

* Replace the value of the apiUrl appSettings entries with the URLs of your	Production and Staging cloud services.

MyVote iPad Setup
-----------------

**src/iPad/MyVoteLV/MyVoteLV/HelperClasses/Constants.m**

Replace "kURL" with your Azure Mobile Service url.
Replace "kAppKey" with your Azure Mobile Services application key.
Replace "kMALAPIBaseURLString" with your REST API endpoint.