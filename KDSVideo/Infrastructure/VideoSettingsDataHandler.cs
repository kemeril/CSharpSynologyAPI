using StdUtils;
using SynologyAPI;
using Windows.Storage;

namespace KDSVideo.Infrastructure
{
    public class VideoSettingsDataHandler : IVideoSettingsDataHandler
    {
        private const string VideoSettingsDataKey = nameof(VideoSettingsDataKey);

        public VideoSettings Get()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (!settingValues.TryGetValue(VideoSettingsDataKey, out var valueObject))
            {
                return VideoSettings.Default;
            }

            return valueObject != null && !string.IsNullOrWhiteSpace((string)valueObject)
                ? JsonHelper.FromJson<VideoSettings>((string)valueObject)
                : VideoSettings.Default;
        }

        public void SetOrUpdate(VideoStation.VideoTranscoding videoTranscoding, bool ac3PassThrough)
        {
            var json = JsonHelper.ToJson(new VideoSettings
            {
                VideoTranscoding = videoTranscoding,
                Ac3PassThrough = ac3PassThrough
            });
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[VideoSettingsDataKey] = json;
        }

        public void Clear()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[VideoSettingsDataKey] = string.Empty;
        }
    }
}
