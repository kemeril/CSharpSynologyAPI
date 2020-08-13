using System.Runtime.Serialization;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class TrustedLoginData : ICloneable<TrustedLoginData>
    {
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "account")]
        public string Account { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "did")]
        public string DeviceId { get; set; }

        public TrustedLoginData Clone() =>
            new TrustedLoginData
            {
                Host = Host,
                Account = Account,
                Password = Password,
                DeviceId = DeviceId,
            };
    }
}