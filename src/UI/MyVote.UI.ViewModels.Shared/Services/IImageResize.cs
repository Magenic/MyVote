using System.IO;

namespace MyVote.UI.Services
{
    public interface IImageResize
    {
        Stream ResizeImage(Stream imageStream, float maxWidth, float maxHeight);
    }
}