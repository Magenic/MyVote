using MyVote.UI.ViewModels;
using System.Threading.Tasks;

namespace MyVote.UI.Services
{
    public interface IAzureStorageService
    {
		Task UploadPicture(UploadViewModel uploadViewModel);
    }
}
