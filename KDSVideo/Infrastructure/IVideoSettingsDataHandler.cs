using SynologyAPI;

namespace KDSVideo.Infrastructure
{
    public interface IVideoSettingsDataHandler
    {
        VideoSettings Get();
        void SetOrUpdate(VideoStation.VideoTranscoding videoTranscoding, bool ac3PassThrough);
        void Clear();
    }
}