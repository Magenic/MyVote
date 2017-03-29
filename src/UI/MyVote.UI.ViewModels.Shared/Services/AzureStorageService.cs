﻿using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
#endif // NETFX_CORE

namespace MyVote.UI.Services
{
    public sealed class AzureStorageService : IAzureStorageService
    {
		private const string Container = "pollimages";

		private readonly IMobileService mobileService;

		public AzureStorageService(IMobileService mobileService)
		{
			this.mobileService = mobileService;
		}

		public async Task UploadPicture(UploadViewModel uploadViewModel)
		{
			var sasUrl = await this.mobileService.GenerateStorageAccessSignatureAsync().ConfigureAwait(false);

#if NETFX_CORE
            var blobService = new CloudBlobClient(
                new Uri(sasUrl.Replace("\"", string.Empty), UriKind.Absolute));

            var container = blobService.GetContainerReference(Container);
            var blob = container.GetBlockBlobReference(uploadViewModel.ImageIdentifier);

            using (var fileStream = await uploadViewModel.PictureFile.OpenAsync(FileAccessMode.Read))
            {
				// TODO: Make this work with the new Azure Storage package
                //await blob.UploadFromStreamAsync(fileStream);
            }
#else
		    sasUrl = sasUrl.Replace("/pollimages", String.Format("/pollimages/{0}", uploadViewModel.ImageIdentifier));
            sasUrl = sasUrl.Replace("\"", "");
			var request = (HttpWebRequest)WebRequest.Create(sasUrl);
			request.Method = "PUT";
			uploadViewModel.PictureStream.Position = 0;

			request.ContentLength = uploadViewModel.PictureStream.Length;
			request.Headers["x-ms-blob-type"] = "BlockBlob";
            request.Headers["x-ms-blob-content-disposition"] = String.Format("attachment; filename=\"pollimages/{0}\"", uploadViewModel.ImageIdentifier);
			
            var pictureData = new byte[uploadViewModel.PictureStream.Length];

			await uploadViewModel.PictureStream.ReadAsync(pictureData, 0, (int)uploadViewModel.PictureStream.Length);
			using (var requestStream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null).ConfigureAwait(false))
			{
				await requestStream.WriteAsync(pictureData, 0, pictureData.Length).ConfigureAwait(false);
			}

			await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null).ConfigureAwait(false);
#endif // NETFX_CORE
		}
    }
}
