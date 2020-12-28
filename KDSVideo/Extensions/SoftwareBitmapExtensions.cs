using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace KDSVideo.Extensions
{
    public static class SoftwareBitmapExtensions
    {
        /// <summary>
        /// Resizes the specified stream.
        /// </summary>
        /// <param name="sourceStream">The source stream to resize.</param>
        /// <param name="newWidth">The width of the resized image.</param>
        /// <param name="newHeight">The height of the resized image.</param>
        /// <returns>The resized image stream.</returns>
        public static async Task<InMemoryRandomAccessStream> ResizeAsync(this IRandomAccessStream sourceStream, uint newWidth, uint newHeight)
        {
            var decoder = await BitmapDecoder.CreateAsync(sourceStream);
            var destinationStream = new InMemoryRandomAccessStream();

            var transform = new BitmapTransform { ScaledWidth = newWidth, ScaledHeight = newHeight };

            var pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.RespectExifOrientation,
                ColorManagementMode.DoNotColorManage);

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);

            //if (decoder.OrientedPixelHeight != decoder.PixelHeight && decoder.OrientedPixelWidth != decoder.PixelWidth)
            //    encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                newWidth, newHeight,
                96, 96,
                pixelData.DetachPixelData());
            await encoder.FlushAsync();

            return destinationStream;
        }
    }
}
