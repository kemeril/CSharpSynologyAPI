using StdUtils;
using SynologyRestDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        protected async Task<string> _run(RequestBuilder requestBuilder)
        {
            var request = WebRequest.Create(BaseUrl.ToString() + requestBuilder);
            if (Proxy != null)
            {
                request.Proxy = Proxy;
            }

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (response == null) return null;
            var responseStream = response.GetResponseStream();
            if (responseStream == null) return null;

            using (var reader = new StreamReader(responseStream))
            {
                var resJson = await reader.ReadToEndAsync().ConfigureAwait(false);
                return resJson;
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

        public async Task<T> PostFile<T>(string apiName, string method, string fileName, Stream fileStream, string fileParam = "file")
        {
            var stationApi = (await GetApi(apiName).ConfigureAwait(false)).FirstOrDefault();
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

        public async Task<T> Call<T>(string apiName, RequestBuilder requestBuilder)
        {
            var stationApi = (await GetApi(apiName).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? default(T)
                : JsonHelper.FromJson<T>(await _run(CreateRequest(requestBuilder, stationApi)));
        }

        protected async Task<T> CallMethod<T>(string apiName, string method)
        {
            return await Call<T>(apiName, new RequestBuilder(Sid).Session(Sid).Method(method)).ConfigureAwait(false);
        }

        public async Task<T> CallMethod<T>(string apiName, string method, ReqParams param)
        {
            return await Call<T>(apiName, new RequestBuilder(Sid).Method(method, param)).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequest(string apiName, RequestBuilder requestBuilder)
        {
            var stationApi = (await GetApi(apiName).ConfigureAwait(false)).FirstOrDefault();
            return stationApi.Key == null
                ? null
                : _createWebRequest(CreateRequest(requestBuilder, stationApi));
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<WebRequest> GetWebRequest(string apiName, string method)
        {
            return await GetWebRequest(apiName, new RequestBuilder(Sid).Session(Sid).Method(method)).ConfigureAwait(false);
        }

        public async Task<WebRequest> GetWebRequest(string apiName, string method, ReqParams param)
        {
            return await GetWebRequest(apiName, new RequestBuilder(Sid).Session(Sid).Method(method, param)).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, ApiSpec>> GetApi(string apiName)
        {
            if (ApiInfo == null)
            {
                ApiInfo = JsonHelper.FromJson<ApiInfo>(await _run(
                    new RequestBuilder().
                        Api("SYNO.API.Info").
                        AddParam("query", string.Join(",", ImplementedApi.Select(k => k.Key)))
                   )
                );
            }
            return ApiInfo.Data.Where(p => p.Key.StartsWith(apiName)).ToDictionary(t => t.Key, t => t.Value);
        }

        public async Task<bool> Login()
        {
            var loginResult = await CallMethod<LoginResult>("SYNO.API.Auth",
                "login", new ReqParams
                {
                        {"account", Username},
                        {"passwd", Password},
                        {"session", InternalSession},
                        {"format", "sid"}
                    }
            );
            if (loginResult.Success)
            {
                Sid = loginResult.Data.Sid;
            }
            return loginResult.Success;
        }

        public async Task<bool> Logout()
        {
            var logoutResult = await CallMethod<TResult<object>>("SYNO.API.Auth", "logout", new ReqParams { {"session", InternalSession} });
            return logoutResult.Success;
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