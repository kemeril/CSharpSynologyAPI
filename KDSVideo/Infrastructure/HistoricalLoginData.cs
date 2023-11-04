using System.Runtime.Serialization;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class HistoricalLoginData
    {
        [DataMember(Name = "host")]
        public string Host { get; set; } = string.Empty;

        [DataMember(Name = "account")]
        public string Account { get; set; } = string.Empty;

        [DataMember(Name = "password")]
        public string Password { get; set; } = string.Empty;
    }
}
