using SynologyAPI;
using SynologyRestDAL.Vs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace KDSVideo.ViewModels.NavigationViewModels
{
    public abstract class MediaMetaDataItem
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);

        protected MediaMetaDataItem(MetaDataItem metaDataItem)
        {
            MetaDataItem = metaDataItem;
        }

        public MetaDataItem MetaDataItem { get; }

        public SoftwareBitmap Poster { get; set; }

        /// <summary>
        /// Resizes the specified stream.
        /// </summary>
        /// <param name="sourceStream">The source stream to resize.</param>
        /// <param name="newWidth">The width of the resized image.</param>
        /// <param name="newHeight">The height of the resized image.</param>
        /// <returns>The resized image stream.</returns>
        private static async Task<InMemoryRandomAccessStream> ResizeAsync(IRandomAccessStream sourceStream, uint newWidth, uint newHeight)
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

        public async Task<SoftwareBitmap> GetPoster(IVideoStation videoStation, uint width, uint height)
        {
            var cts = new CancellationTokenSource(_timeout);
            var webRequest = await videoStation.PosterGetImageAsync(MetaDataItem.Id, MetaDataItem.MediaType, cts.Token).ConfigureAwait(false);

            cts = new CancellationTokenSource(_timeout);
            using (var webResponse = await webRequest.GetResponseAsync(cts.Token).ConfigureAwait(false))
            {
                using (var posterStream = webResponse.GetResponseStream())
                {
                    if (posterStream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await posterStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            var randomAccessStream = memoryStream.AsRandomAccessStream();

                            var resizedRandomAccessStream = await ResizeAsync(randomAccessStream, width, height).ConfigureAwait(false);
                            //var resizedRandomAccessStream = randomAccessStream;

                            var decoder = await BitmapDecoder.CreateAsync(resizedRandomAccessStream);
                            var transform = new BitmapTransform { ScaledWidth = width, ScaledHeight = height };
                            var sourceSoftwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                            return sourceSoftwareBitmap;
                        }
                    }
                }
            }

            return null;
        }
    }
}