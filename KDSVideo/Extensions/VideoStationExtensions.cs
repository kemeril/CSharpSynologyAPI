using SynologyAPI;
using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using SynologyAPI.SynologyRestDAL.Vs;

namespace KDSVideo.Extensions
{
    public static class VideoStationExtensions
    {
        /// <summary>
        /// Download the poster image for a media.
        /// </summary>
        /// <param name="videoStation">The <see cref="IVideoStation"/> instance.</param>
        /// <param name="id">Id of the media whose poster image wants to be downloaded. <see cref="MetaDataItem.Id"/></param>
        /// <param name="mediaType">Select the of the media Movie, TVShow or TVShowEpisode.</param>
        /// <param name="width">The desired width in pixels of the poster image.</param>
        /// <param name="height">The desired height in pixels of the poster image.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The poster image.</returns>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException"> is throws on error while communication with the NAS.</exception>
        /// <exception cref="CommunicationException">Unknown communication error.</exception>
        /// <exception cref="TimeoutException">Timeout error.</exception>
        /// <exception cref="Exception">Unknown error.</exception>
        public static async Task<SoftwareBitmap> GetPosterSoftwareBitmapAsync(this IVideoStation videoStation, int id, MediaType mediaType, uint width, uint height, CancellationToken cancellationToken = default)
        {
            var webRequest = await videoStation.PosterGetImageAsync(id, mediaType, cancellationToken).ConfigureAwait(false);

            using (var webResponse = await webRequest.GetResponseAsync(cancellationToken).ConfigureAwait(false))
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

                            var resizedRandomAccessStream = await randomAccessStream.ResizeAsync(width, height).ConfigureAwait(false);
                            //var resizedRandomAccessStream = randomAccessStream;

                            var decoder = await BitmapDecoder.CreateAsync(resizedRandomAccessStream);
                            var transform = new BitmapTransform { ScaledWidth = width, ScaledHeight = height };
                            var sourceSoftwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                            return sourceSoftwareBitmap;
                        }
                    }
                }
            }

            throw new CommunicationException($"Error while retrieving poster image. id:{id}, mediaType:{mediaType}");
        }

        /// <summary>
        /// Download the backdrop image for a media.
        /// </summary>
        /// <param name="videoStation">The <see cref="IVideoStation"/> instance.</param>
        /// <param name="mapperId">MapperId of the media whose backdrop image wants to be downloaded. <see cref="MetaDataItem.MapperId"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The backdrop image.</returns>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException"> is throws on error while communication with the NAS.</exception>
        /// <exception cref="CommunicationException">Unknown communication error.</exception>
        /// <exception cref="TimeoutException">Timeout error.</exception>
        /// <exception cref="Exception">Unknown error.</exception>
        public static async Task<SoftwareBitmap> GetBackdropSoftwareBitmapAsync(this IVideoStation videoStation, int mapperId, CancellationToken cancellationToken = default)
        {
            var webRequest = await videoStation.BackdropGetAsync(mapperId, cancellationToken).ConfigureAwait(false);

            using (var webResponse = await webRequest.GetResponseAsync(cancellationToken).ConfigureAwait(false))
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

                            var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);
                            var sourceSoftwareBitmap = await decoder.GetSoftwareBitmapAsync();
                            return sourceSoftwareBitmap;
                        }
                    }
                }
            }

            throw new CommunicationException($"Error while retrieving backdrop image. mapperId:{mapperId}");
        }
    }
}
