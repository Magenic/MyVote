using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyVote.UI.Contracts
{
    public interface IPollImageViewModel
    {
        Task AddImage();
        Task<string> UploadImage();
        bool HasImage { get; }
        Image PollImage { get; set; }
    }
}
