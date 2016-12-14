using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace MyVote.Services.MobileServices.Controllers
{
    [MobileAppController]
    public class SasGeneratorController : ApiController
    {
        [HttpGet, Route("api/SasGenerator")]
        public string Get()
		{
			var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			var blobClient = storageAccount.CreateCloudBlobClient();

			var container = blobClient.GetContainerReference("pollimages");
			container.CreateIfNotExists();

            var sasConstraints = new SharedAccessBlobPolicy
            {
                // To ensure SAS is valid immediately, don’t set start time.
                // This way, you can avoid failures caused by small clock differences.
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Permissions = SharedAccessBlobPermissions.Write |
                   SharedAccessBlobPermissions.Read
            };

			var sasToken = container.GetSharedAccessSignature(sasConstraints);

			return container.Uri + sasToken;
		}
    }
}
