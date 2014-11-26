using MyVote.UI.ViewModels;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
	public interface IPhotoChooser
	{
#if !__ANDROID__
		Task<UploadViewModel> ShowChooser();
#endif
	}
}
