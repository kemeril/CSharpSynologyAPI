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
    public class Station
    {
        protected Uri BaseUrl;
        protected string Username;
        protected string Password;
        protected string Sid;
        protected string InternalSession;
        protected IWebProxy Proxy;
        protected ApiInfo ApiInfo;

        private Dictionary<string, int> _implementedApi;
        protected Dictionary<string, int> ImplementedApi => _implementedApi ?? (_implementedApi = GetImplementedApi());

        public Station()
        {
            // **** Ignore any ssl errors
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
      
        public Station(Uri url, string username, string password)
            : this()
        {
            BaseUrl = url;
            Username = username;
            Password = password;
            InternalSession = "DownloadStation";
        }

        protected virtual Dictionary<string, int> GetImplementedApi()
        {
            return  new Dictionary<string, int> { { "SYNO.API.Auth", 3 } };
        }

        public Station(Uri url, string username, string password, IWebProxy proxy)
            : this(url, username, password)
        {
            if (proxy != null)
            {
                Proxy = proxy;
            }
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

        public async Task<T> PostFileAsync<T>(string apiName, string method, string fileName, Stream fileStream, string fileParam = "file", CancellationToken cancellationToken = default(CancellationToken))
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null ? default(T) : 
                JsonHelper.FromJson<T>(
                        _postFile(
                            CreateRequest((new RequestBuilder(Sid)).Session(Sid).Method(method), stationApi),
                            fileName,
                            fileStream,
                            fileParam
                       )
            );
        }

        public async Task<T> CallAsync<T>(string apiName, RequestBuilder requestBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? default(T)
                : JsonHelper.FromJson<T>(await _runAsync(CreateRequest(requestBuilder, stationApi), cancellationToken));
        }

        protected async Task<T> CallMethodAsync<T>(string apiName, string method, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CallAsync<T>(apiName, new RequestBuilder(Sid).Session(Sid).Method(method), cancellationToken).ConfigureAwait(false);
        }

        public async Task<T> CallMethodAsync<T>(string apiName, string method, ReqParams param, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CallAsync<T>(apiName, new RequestBuilder(Sid).Method(method, param), cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequestAsync(string apiName, RequestBuilder requestBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var stationApi = (await GetApiAsync(apiName, cancellationToken).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? null
                : _createWebRequest(CreateRequest(requestBuilder, stationApi));
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<WebRequest> GetWebRequestAsync(string apiName, string method, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(apiName, new RequestBuilder(Sid).Session(Sid).Method(method), cancellationToken).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequestAsync(string apiName, string method, ReqParams param, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetWebRequestAsync(apiName, new RequestBuilder(Sid).Session(Sid).Method(method, param), cancellationToken).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, ApiSpec>> GetApiAsync(string apiName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ApiInfo == null)
            {
                var jsonResult = await _runAsync(
                    new RequestBuilder().Api("SYNO.API.Info").AddParam("query", string.Join(",", ImplementedApi.Select(k => k.Key))),
                    cancellationToken
                ).ConfigureAwait(false);
                ApiInfo = JsonHelper.FromJson<ApiInfo>(jsonResult);
            }
            return ApiInfo.Data.Where(p => p.Key.StartsWith(apiName)).ToDictionary(t => t.Key, t => t.Value);
        }

        public async Task LoginAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var loginResult = await CallMethodAsync<LoginResult>("SYNO.API.Auth",
                "login", new ReqParams
                {
                        {"account", Username},
                        {"passwd", Password},
                        {"session", InternalSession},
                        {"format", "sid"}
                },
                cancellationToken)
                .ConfigureAwait(false);
            if (loginResult.Success)
            {
                Sid = loginResult.Data.Sid;
            }

            if (!loginResult.Success)
                throw new SynoRequestException(@"Synology error code " + loginResult.Error);
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var logoutResult = await CallMethodAsync<TResult<object>>("SYNO.API.Auth", "logout", new ReqParams { {"session", InternalSession} }, cancellationToken)
                .ConfigureAwait(false);

            if (!logoutResult.Success)
                throw new SynoRequestException(@"Synology error code " + logoutResult.Error);
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