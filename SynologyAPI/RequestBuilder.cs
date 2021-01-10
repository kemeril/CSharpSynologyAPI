using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynologyAPI
{
    public class ReqParams : Dictionary<string, string>
    {
    }

    public class RequestBuilder
    {
        internal const string DID = "did";
        
        private readonly ReqParams _reqData = new ReqParams
        {
            { "api", "SYNO.API.Info" },
            { "cgiPath", "query.cgi" },
            { "version", "1" },
            { "method", "query" },
            { "sid", string.Empty }
        };

        private ReqParams _params = new ReqParams();

        private readonly string[] _headBuildOrder = { "api", "version", "method" };

        public RequestBuilder()
        {
        }

        public RequestBuilder(string sessionId)
            : this()
        {
            Session(sessionId);
        }

        public RequestBuilder Api(string apiName)
        {
            _reqData["api"] = apiName;
            return this;
        }

        public RequestBuilder CgiPath(string path)
        {
            _reqData["cgiPath"] = path;
            return this;
        }

        public RequestBuilder Version(string version)
        {
            _reqData["version"] = version;
            return this;
        }

        public RequestBuilder Method(string method)
        {
            _reqData["method"] = method;
            return this;
        }

        public RequestBuilder Method(string method, ReqParams args)
        {
            _reqData["method"] = method;
            SetParams(args);
            return this;
        }

        public RequestBuilder Session(string sid)
        {
            _reqData["sid"] = sid;
            return this;
        }

        public RequestBuilder AddParam(string key, string value)
        {
            _params.Add(key, value);
            return this;
        }

        public ReqParams Params
        {
            get => _params;
            set => _params = value;
        }

        public RequestBuilder SetParams(ReqParams newParams)
        {
            Params = newParams;
            return this;
        }

        public string WebApi() => string.Format("{0}{1}", "webapi", _reqData["cgiPath"] != string.Empty ? "/" + _reqData["cgiPath"] : "");

        public IDictionary<string, string> CallParams =>
            _reqData
                .Where(k => k.Key != "cgiPath")
                .OrderBy(k => k.Key == "sid" ? -1 : _headBuildOrder.ToList().IndexOf(k.Key))
                .ToDictionary(k => k.Key == "sid" ? "_sid" : k.Key, v => v.Value);

        public string Build()
        {
            var request = new StringBuilder();

            request.Append(WebApi());

            var reqHead = (from s in _headBuildOrder where _reqData[s] != string.Empty select s + "=" + System.Web.HttpUtility.UrlEncode(_reqData[s])).ToList();
            var reqParams = _params
                .Select(param => param.Key + "=" + System.Web.HttpUtility.UrlEncode(param.Value))
                .ToList();
           
            if (reqHead.Any() || reqParams.Any())
            {
                request.Append("?");
            }
            
            if (reqHead.Any())
            {
                request.Append(string.Join("&", reqHead));
            }
            
            if (reqParams.Any())
            {
                request.Append("&" + string.Join("&", reqParams));
            }
            
            if (!string.IsNullOrWhiteSpace(_reqData["sid"]))
            {
                request.Append("&_sid=" + _reqData["sid"]);
            }
            
            return request.ToString();
        }

    }

    internal static class ReqParamsExt
    {
        internal static ReqParams SortBy(this ReqParams reqParams, VideoStation.SortBy sortBy,
            VideoStation.SortDirection sortDirection)
        {
            switch (sortBy)
            {
                case VideoStation.SortBy.Added:
                    reqParams.Add("sort_by", "added");
                    break;
                case VideoStation.SortBy.Title:
                    reqParams.Add("sort_by", "title");
                    break;
                case VideoStation.SortBy.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null);
            }

            switch (sortDirection)
            {
                case VideoStation.SortDirection.Ascending:
                    reqParams.Add("sort_direction", "asc");
                    break;
                case VideoStation.SortDirection.Descending:
                    reqParams.Add("sort_direction", "desc");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
            }

            return reqParams;
        }

        internal static ReqParams Offset(this ReqParams reqParams, int offset)
        {
            if (offset > 0)
            {
                reqParams.Add("offset", offset.ToString());
            }

            return reqParams;
        }

        internal static ReqParams Limit(this ReqParams reqParams, int limit)
        {
            if (limit >= 0)
            {
                reqParams.Add("limit", limit.ToString());
            }

            return reqParams;
        }
    }
}