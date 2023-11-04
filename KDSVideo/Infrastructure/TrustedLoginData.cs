using System.Runtime.Serialization;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class TrustedLoginData
    {
        [DataMember(Name = "host")]
        public string Host { get; set; } = string.Empty;

        [DataMember(Name = "account")]
        public string Account { get; set; } = string.Empty;

        [DataMember(Name = "password")]
        public string Password { get; set; } = string.Empty;

        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; } = string.Empty;
    }
}
