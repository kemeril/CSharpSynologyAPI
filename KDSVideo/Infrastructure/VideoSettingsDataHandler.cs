using StdUtils;
using SynologyAPI;
using Windows.Storage;

namespace KDSVideo.Infrastructure
{
    public class VideoSettingsDataHandler : IVideoSettingsDataHandler
    {
        private const string VIDEO_SETTINGS_DATA_KEY = nameof(VIDEO_SETTINGS_DATA_KEY);

        public VideoSettings Get()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (!settingValues.TryGetValue(VIDEO_SETTINGS_DATA_KEY, out var valueObject))
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
            settingValues[VIDEO_SETTINGS_DATA_KEY] = json;
        }

        public void Clear()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[VIDEO_SETTINGS_DATA_KEY] = string.Empty;
        }
    }
}
