using System;
using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage.Blob;
using MyVote.UI.ViewModels;
using Windows.Storage;

namespace MyVote.UI.Services
{
	public sealed class AzureStorageService : IAzureStorageService
	{
		private const string Container = "pollpictures";

		private readonly IMobileService mobileService;

		public AzureStorageService(IMobileService mobileService)
		{
			this.mobileService = mobileService;
		}

		public async Task UploadPicture(UploadViewModel uploadViewModel)
		{
			var sasUri = new Uri(await this.mobileService.GenerateStorageAccessSignatureAsync(uploadViewModel.ImageIdentifier));
			var sasKey = sasUri.Query.Substring(1);

			//var blobService = new CloudBlobClient(
			//	new Uri(string.Format("{0}://{1}", sasUri.Scheme, sasUri.Host), UriKind.Absolute),
			//	new StorageCredentials(sasKey));

			//var container = blobService.GetContainerReference(Container);
			//var blob = container.GetBlockBlobReference(uploadViewModel.ImageIdentifier);

			//using (var fileStream = await uploadViewModel.PictureFile.OpenAsync(FileAccessMode.Read))
			//{
			//	await blob.UploadFromStreamAsync(fileStream);
			//}
			await Task.Run(() => { });
		}
	}
}
