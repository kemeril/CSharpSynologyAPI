using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Storage;
using StdUtils;

namespace KDSVideo.Infrastructure
{
    [DataContract]
    public class LoginData
    {
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "account")]
        public string Account { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "did")]
        public string DeviceId { get; set; }
    }
    
    public class LoginDataHandler : ILoginDataHandler
    {
        private const string LoginDataKey = nameof(LoginDataKey);

        public IList<LoginData> GetAll()
        {
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.TryGetValue(LoginDataKey, out var listObject))
            {
                return JsonHelper.FromJson<List<LoginData>>((string) listObject);
            }

            return new List<LoginData>();
        }

        public LoginData GetLast()
        {
            var all = GetAll();
            if (all.Count > 0)
            {
                return all[0];
            }

            return null;
        }

        public void AddOrUpdate(LoginData loginData)
        {
            var all = GetAll();
            for (var i = 0; i < all.Count; i++)
            {
                if (all[i].Host == loginData.Host && all[i].Account == loginData.Account)
                {
                    all.RemoveAt(i);
                    break;
                }
            }
            all.Insert(0, loginData);
            all = all.Take(20).ToList();

            var json = JsonHelper.ToJson(all);
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            settingValues[LoginDataKey] = json;
        }
    }
}