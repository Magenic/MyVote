using MyVote.UI.ViewModels;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyVote.UI.Services
{
	public sealed class AzureStorageService : IAzureStorageService
	{
		private readonly IMobileService mobileService;

		public AzureStorageService(IMobileService mobileService)
		{
			this.mobileService = mobileService;
		}

		public async Task UploadPicture(UploadViewModel uploadViewModel)
		{
			var sasUrl = await this.mobileService.GenerateStorageAccessSignatureAsync(uploadViewModel.ImageIdentifier);

			var request = HttpWebRequest.Create(sasUrl) as HttpWebRequest;
			request.Method = "PUT";

			uploadViewModel.PictureStream.Position = 0;

#if ANDROID
            request.ContentLength = uploadViewModel.PictureStream.Length;
#else
            request.Headers["Content-Length"] = uploadViewModel.PictureStream.Length.ToString();
#endif
			request.Headers["x-ms-blob-type"] = "BlockBlob";

			var pictureData = new byte[uploadViewModel.PictureStream.Length];

			await uploadViewModel.PictureStream.ReadAsync(pictureData, 0, (int)uploadViewModel.PictureStream.Length);
			using (var requestStream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null))
			{
				await requestStream.WriteAsync(pictureData, 0, pictureData.Length);
			}

			var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
		}
	}
}
