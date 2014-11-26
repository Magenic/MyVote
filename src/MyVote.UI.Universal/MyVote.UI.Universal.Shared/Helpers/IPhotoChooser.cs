using MyVote.UI.ViewModels;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
	public interface IPhotoChooser
	{
#if !ANDROID
		Task<UploadViewModel> ShowChooser();
#endif
	}
}
