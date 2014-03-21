using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace MyVote.AppServer.Controllers
{
	public class PollImageController : ApiController
	{
		const string STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=myvotestorage;AccountKey=m2S2Jzbs+9GPvsCicbWNe1jP2+OxkYJ74xjIBLOGU0DBR5vCyFgS3c2tE+6o3x6pgTr/eEn4YahG9glw41tGCw==";

		[Authorize]
		public HttpResponseMessage Put(HttpRequestMessage request)
		{
			string imageId = Guid.NewGuid().ToString("N") + ".jpg"; // TODO: file extension from client side
			var imageBytes = request.Content.ReadAsByteArrayAsync().Result;

			var pollPicturesContainer = CloudStorageAccount
				.Parse(STORAGE_CONNECTION_STRING)
				.CreateCloudBlobClient()
				.GetContainerReference("pollpictures");

			var imageBlockBlob = pollPicturesContainer.GetBlockBlobReference(imageId);
			using (var imageStream = new MemoryStream(imageBytes))
			{
				imageBlockBlob.UploadFromStream(imageStream, new BlobRequestOptions{Timeout = TimeSpan.FromSeconds(15)});
			}

			return request.CreateResponse(
				HttpStatusCode.OK,
				new { imageUrl = "http://myvotestorage.blob.core.windows.net/pollpictures/" + imageId});
		}
	}
}