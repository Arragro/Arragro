using Arragro.Common.Models;

namespace Arragro.Common.Interfaces.Providers
{
    public interface IImageProcessor
    {
        ImageProcessResult GetImage(byte[] bytes, int quality = 80, bool asProgressiveJpeg = false);
        ImageProcessResult GetImage(byte[] bytes, int width, int quality = 80, bool asProgressiveJpeg = false);
    }
}
