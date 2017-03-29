using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MyVote.Services.AppServer.Controllers
{
	[Route("api/[controller]")]
	public sealed class PollImageController 
		: Controller
	{
		const string STORAGE_CONNECTION_STRING = "YOUR_AZURE_BLOB_CONNECTION_HERE";

		[Authorize]
		[HttpPost]		
		public async Task<IActionResult> Post()
		{

            //Read file from POST and extract properties
		    var imageFile = Request.Form.Files[0];
		    var fileExtension = Path.GetExtension(imageFile.FileName);
            var imageId = Guid.NewGuid().ToString("N") + fileExtension;
            var imageStream = new MemoryStream(new BinaryReader(imageFile.OpenReadStream()).ReadBytes((int)imageFile.Length));

            // Setup cloud storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONNECTION_STRING);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference("pollimages");
            
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageId);

            await blockBlob.UploadFromStreamAsync(imageStream);

            return new OkObjectResult(new { imageUrl = blockBlob.Uri.ToString() });

		}
	}
}