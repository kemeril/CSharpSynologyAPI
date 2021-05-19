using System.Runtime.Serialization;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class TrustedLoginData
    {
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "account")]
        public string Account { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; }
    }
}
