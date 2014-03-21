using System;
using Windows.ApplicationModel.DataTransfer;

namespace MyVote.UI.Helpers
{
	public interface IShareManager
	{
		void Initialize();
		void Cleanup();
		Action<DataPackage> OnShareRequested { get; set; }
	}
}
