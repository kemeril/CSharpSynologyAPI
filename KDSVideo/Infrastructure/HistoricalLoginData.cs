using System.Runtime.Serialization;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class HistoricalLoginData : ICloneable<HistoricalLoginData>
    {
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "account")]
        public string Account { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        public HistoricalLoginData Clone() =>
            new HistoricalLoginData
            {
                Host = Host,
                Account = Account,
                Password = Password
            };
    }
}