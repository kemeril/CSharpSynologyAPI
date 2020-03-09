using StdUtils;
using SynologyRestDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SynologyAPI.Exception;

namespace SynologyAPI
{
    public class Station : IStation
    {
        // ReSharper disable InconsistentNaming
        private const string ApiSynoApiAuth = "SYNO.API.Auth";
        private const string ApiSynoApiInfo = "SYNO.API.Info";
        // ReSharper restore InconsistentNaming

        private const string MethodLogin = "login";
        private const string MethodLogout = "logout";

        protected Uri BaseUrl;
        protected string Sid;
        protected IWebProxy Proxy;
        // ReSharper disable InconsistentNaming
        protected ApiInfo ApiInfo;
        // ReSharper restore InconsistentNaming

        private Dictionary<string, int> _implementedApi;
        protected Dictionary<string, int> ImplementedApi => _implementedApi ?? (_implementedApi = GetImplementedApi());
      
        public Station()
        {
            // **** Ignore any ssl errors
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        protected virtual string GetSessionName()
        {
            return "DownloadStation";
        }

        protected virtual Dictionary<string, int> GetImplementedApi()
        {
            return new Dictionary<string, int> { { ApiSynoApiAuth, 3 } };
        }

        protected async Task<string> _runAsync(RequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            var request = WebRequest.Create(BaseUrl.ToString() + requestBuilder);
            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            var response = await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);

            var responseStream = response?.GetResponseStream();
            if (responseStream == null) return null;

            using (var reader = new StreamReader(responseStream))
            {
                var jsonResult = await reader.ReadToEndAsync().ConfigureAwait(false);
                return jsonResult;
            }
        }

        protected WebRequest _createWebRequest(RequestBuilder requestBuilder)
        {
            var request = WebRequest.Create(BaseUrl.ToString() + requestBuilder);
            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            return request;
        }

        // ReSharper disable once UnusedMember.Global
        public RequestBuilder CreateRequest(KeyValuePair<string, ApiSpec> apiSpec)
        {
            return new RequestBuilder().
                        Api(apiSpec.Key).
                        CgiPath(apiSpec.Value.Path).
                        Version(apiSpec.Value.MaxVersion.ToString()).
                        Method("");
        }

        public RequestBuilder CreateRequest(RequestBuilder requestBuilder, KeyValuePair<string, ApiSpec> apiSpec)
        {
            return requestBuilder.
                        Api(apiSpec.Key).
                        CgiPath(apiSpec.Value.Path).
                        Version(apiSpec.Value.MaxVersion.ToString());
        }

        public async Task<T> PostFileAsync<T>(string apiName, string method, string fileName, Stream fileStream, string fileParam = "file", CancellationToken cancellationToken = default)
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null ? default : 
                JsonHelper.FromJson<T>(
                        _postFile(
                            CreateRequest((new RequestBuilder(Sid)).Session(Sid).Method(method), stationApi),
                            fileName,
                            fileStream,
                            fileParam
                       )
            );
        }

        public async Task<T> CallAsync<T>(string apiName, RequestBuilder requestBuilder, CancellationToken cancellationToken = default)
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? default
                : JsonHelper.FromJson<T>(await _runAsync(CreateRequest(requestBuilder, stationApi), cancellationToken));
        }

        protected async Task<T> CallMethodAsync<T>(string apiName, string method, CancellationToken cancellationToken = default)
        {
            return await CallAsync<T>(apiName, new RequestBuilder(Sid).Session(Sid).Method(method), cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> CallMethodAsync<T>(string apiName, string method, ReqParams param, CancellationToken cancellationToken = default)
        {
            return await CallAsync<T>(apiName, new RequestBuilder(Sid).Method(method, param), cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequestAsync(string apiName, RequestBuilder requestBuilder, CancellationToken cancellationToken = default)
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? null
                : _createWebRequest(CreateRequest(requestBuilder, stationApi));
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<WebRequest> GetWebRequestAsync(string apiName, string method, CancellationToken cancellationToken = default)
        {
            return await GetWebRequestAsync(apiName, new RequestBuilder(Sid).Session(Sid).Method(method), cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequestAsync(string apiName, string method, ReqParams param, CancellationToken cancellationToken = default)
        {
            return await GetWebRequestAsync(apiName, new RequestBuilder(Sid).Session(Sid).Method(method, param), cancellationToken).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, ApiSpec>> GetApiAsync(string apiName, CancellationToken cancellationToken = default)
        {
            if (ApiInfo == null)
            {
                var jsonResult = await _runAsync(
                    new RequestBuilder().Api(ApiSynoApiInfo).AddParam("query", string.Join(",", ImplementedApi.Select(k => k.Key))),
                    cancellationToken
                ).ConfigureAwait(false);
                ApiInfo = JsonHelper.FromJson<ApiInfo>(jsonResult);
            }
            return ApiInfo.Data.Where(p => p.Key.StartsWith(apiName)).ToDictionary(t => t.Key, t => t.Value);
        }

        /// <summary>
        /// Logins to the Synology NAS.
        /// Support 2-way authentication.
        /// </summary>
        /// <param name="url">THe VideoStation base url. Required.</param>
        /// <param name="username">The username. Required.</param>
        /// <param name="password">The password. Required.</param>
        /// <param name="otpCode">
        /// 6-digit otp code. Optional. Available DSM 3 and onward.
        /// First try to login with optCode left null or empty string.
        /// If login has failed with error code 403 ask user for 6-digit optCode and try to login again but pass the optCode as well.
        /// 
        /// OptCode is used when 2-way authentication shall be used and the code can be obtained by Google 2-Step Authentication service.
        /// </param>
        /// <param name="deviceId">Device id (max: 255). Optional. Available DSM 6 and onward.</param>
        /// <param name="deviceName">Optional.</param>
        /// <param name="cipherText">>Optional.</param>
        /// <param name="proxy">Optional.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// username
        /// or
        /// password
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// username cannot be empty! - username
        /// or
        /// password cannot be empty! - password
        /// </exception>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException">Synology NAS returns an error.</exception>
        public async Task<LoginInfo> LoginAsync(Uri url, string username, string password, string otpCode = null, string deviceId = null, string deviceName = null, string cipherText = null,
             IWebProxy proxy = null, CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            if (username == null) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("username cannot be empty!", nameof(username));

            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("password cannot be empty!", nameof(password));

            if (!string.IsNullOrWhiteSpace(otpCode) && otpCode.Length != 6)
            {
                throw new ArgumentException("otpCode is optional but if it is passed it has to be 6-digits length!", nameof(otpCode));
            }

            BaseUrl = url;

            if (proxy != null)
            {
                Proxy = proxy;
            }

            var param = new ReqParams
            {
                {"account", username},
                {"passwd", password},
                {"session",  GetSessionName()},
                {"format", "sid"}
            };

            if (!string.IsNullOrWhiteSpace(otpCode))
            {
                param.Add("otp_code", otpCode);
            }

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                param.Add("device_id", deviceId);
                if (!string.IsNullOrWhiteSpace(otpCode))
                {
                    param.Add("enable_device_token", "yes");
                }
            }

            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                param.Add("device_name", deviceName);
            }

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                param.Add("__cIpHeRtExT", cipherText);
            }

            var clientTime = DateTimeConverter.ToUnixTime(DateTime.UtcNow);
            param.Add("client_time", clientTime.ToString());

            Sid = string.Empty;

            var loginResult = await CallMethodAsync<LoginResult>(ApiSynoApiAuth,
                    MethodLogin, param,
                    cancellationToken)
                .ConfigureAwait(false);
            if (loginResult.Success)
            {
                Sid = loginResult.Data.Sid;
            }

            if (!loginResult.Success)
                throw new SynoRequestException(ApiSynoApiAuth, MethodLogin, loginResult.Error.Code);

            return loginResult.Data;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            Sid = string.Empty;

            var logoutResult = await CallMethodAsync<TResult<object>>(ApiSynoApiAuth, MethodLogout, new ReqParams { {"session", GetSessionName()} }, cancellationToken)
                .ConfigureAwait(false);

            if (!logoutResult.Success)
                throw new SynoRequestException(ApiSynoApiAuth, MethodLogout, logoutResult.Error.Code);
        }

        protected string _postFile(RequestBuilder requestBuilder, string fileName, Stream fileStream, string fileParam = "file")
        {
            var requestHandler = new HttpClientHandler();
            if (Proxy != null)
            {
                requestHandler.Proxy = Proxy;
            }

            var requestUri = BaseUrl + requestBuilder.WebApi();

            var boundary = $"----------{Guid.NewGuid():N}";

            using (var client = new HttpClient(requestHandler))
            {
                using (var formData = new MultipartFormDataContent(boundary))
                {
                    foreach (var param in requestBuilder.CallParams)
                    {
                        var c = new StringContent(param.Value);
                        c.Headers.Remove("Content-Type");
                        formData.Add(c, StringUtils.Enquote(param.Key));
                    }

                    HttpContent fileStreamContent = new StreamContent(fileStream);

                    // This looks ugly, but won't work otherwise, server api is very sensitive to quotes and stuff
                    fileStreamContent.Headers.Remove("Content-Disposition");
                    fileStreamContent.Headers.ContentDisposition = 
                        new ContentDispositionHeaderValue("form-data") { FileName = StringUtils.Enquote(fileName), Name = StringUtils.Enquote("file") };

                    fileStreamContent.Headers.Remove("Content-Type");
                    fileStreamContent.Headers.TryAddWithoutValidation("Content-Type", "application/octet-stream");

                    formData.Add(fileStreamContent, fileParam, fileName);

                    formData.Headers.Remove("Content-Type");
                    formData.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

                    var response = client.PostAsync(requestUri, formData).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStreamAsync().Result;
                        using (var reader = new StreamReader(result))
                        {
                            var resJson = reader.ReadToEnd();
                            return resJson;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}