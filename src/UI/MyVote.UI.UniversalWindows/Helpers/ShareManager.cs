using System;
using Windows.ApplicationModel.DataTransfer;

namespace MyVote.UI.Helpers
{
	public sealed class ShareManager : IShareManager
	{
		private DataTransferManager DataTransferManager { get; set; }

		public void Initialize()
		{
			this.DataTransferManager = DataTransferManager.GetForCurrentView();
			this.DataTransferManager.DataRequested += dataTransferManager_DataRequested;
		}

		public void Cleanup()
		{
			this.DataTransferManager.DataRequested -= dataTransferManager_DataRequested;
		}

		private void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
		{
			if (OnShareRequested != null)
			{
				OnShareRequested(args.Request.Data);
			}
		}

		public Action<DataPackage> OnShareRequested { get; set; }
	}
}
