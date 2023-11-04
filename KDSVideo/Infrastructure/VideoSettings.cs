using System.Runtime.Serialization;
using SynologyAPI;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class VideoSettings
    {
        public static readonly VideoSettings Default = new() { VideoTranscoding = VideoStation.VideoTranscoding.Raw, Ac3PassThrough = true };

        [DataMember(Name = "videoTranscoding")]
        public VideoStation.VideoTranscoding VideoTranscoding { get; set; }

        [DataMember(Name = "ac3PassThrough")]
        public bool Ac3PassThrough { get; set; }
    }
}
