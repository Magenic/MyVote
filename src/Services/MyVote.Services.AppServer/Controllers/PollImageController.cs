using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MyVote.Services.AppServer.Controllers
{
	public class PollImageController : ApiController
	{
        const string STORAGE_CONNECTION_STRING = "My Storage Connection String";

		[Authorize]
		public HttpResponseMessage Put(HttpRequestMessage request)
		{
			string imageId = Guid.NewGuid().ToString("N") + ".jpg"; // TODO: file extension from client side
			var imageBytes = request.Content.ReadAsByteArrayAsync().Result;

			var pollPicturesContainer = CloudStorageAccount
				.Parse(STORAGE_CONNECTION_STRING)
				.CreateCloudBlobClient()
				.GetContainerReference("pollimages");

			var imageBlockBlob = pollPicturesContainer.GetBlockBlobReference(imageId);
			using (var imageStream = new MemoryStream(imageBytes))
			{
				imageBlockBlob.UploadFromStream(imageStream, null, new BlobRequestOptions{ ServerTimeout = TimeSpan.FromSeconds(15)});
			}

			return request.CreateResponse(
				HttpStatusCode.OK,
				new { imageUrl = "https://myapp.blob.core.windows.net/pollimages/" + imageId});
		}
	}
}