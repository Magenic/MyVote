using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class ShareManagerMock : IShareManager
	{
		public Action InitializeDelegate { get; set; }
		public void Initialize()
		{
			if (InitializeDelegate != null)
			{
				InitializeDelegate();
			}
		}

		public Action CleanupDelegate { get; set; }
		public void Cleanup()
		{
			if (CleanupDelegate != null)
			{
				CleanupDelegate();
			}
		}

		public void ExecuteShareRequested(DataPackage dataPackage)
		{
			OnShareRequested(dataPackage);
		}

		public Action<DataPackage> OnShareRequested { get; set; }
	}
}
