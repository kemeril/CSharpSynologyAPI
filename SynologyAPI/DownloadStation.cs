using SynologyRestDAL;
using SynologyRestDAL.Ds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SynologyAPI
{
    public sealed class DownloadStation : Station
    {
        public DownloadStation()
        {}

        protected override Dictionary<string, int> GetImplementedApi()
        {
            var implementedApi = base.GetImplementedApi();
            implementedApi.Add("SYNO.DownloadStation.Task", 3);
            implementedApi.Add("SYNO.DownloadStation.Info", 3);
            return implementedApi;
        }

        public DownloadStation(Uri url, string username, string password, WebProxy proxy)
            : base(url, username, password, proxy)
        {
        }

        public DownloadStation(Uri url, string username, string password)
            : base(url, username, password)
        {
        }

        public async Task<InfoResult> Info()
        {
            return await CallMethod<InfoResult>("SYNO.DownloadStation.Info", "getinfo");
        }

        public async Task<ListResult> List()
        {
            return await CallMethod<ListResult>("SYNO.DownloadStation.Task", "list");
        }

        public async Task<ListResult> List(string[] additional, int offset = 0, int limit = -1)
        {
            return await CallMethod<ListResult>("SYNO.DownloadStation.Task",
                "list", new ReqParams
                {
                    {"additional", string.Join(",", additional)},
                    {"offset", offset.ToString()},
                    {"limit", limit.ToString()}
                }
            );
        }

        public async Task<ListResult> List(string additional, int offset = 0, int limit = -1)
        {
            return await CallMethod<ListResult>("SYNO.DownloadStation.Task",
                "list", new ReqParams
                    {
                        {"additional", additional},
                        {"offset", offset.ToString()},
                        {"limit", limit.ToString()}
                    }
           );
        }

        public async Task<TaskOperationResult> PauseTasks(string[] taskIds)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "pause", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                }
            );
        }

        public async Task<TaskOperationResult> PauseTasks(string taskIds)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "pause", new ReqParams
                {
                    {"id", taskIds},
                }
            );
        }

        public async Task<TaskOperationResult> ResumeTasks(string[] taskIds)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "resume", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                }
            );
        }

        public async Task<TaskOperationResult> ResumeTasks(string taskIds)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "resume", new ReqParams
                {
                    {"id", taskIds},
                }
            );
        }

        public async Task<TaskOperationResult> DeleteTasks(string[] taskIds, bool forceComplete = false)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "delete", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                    {"force_complete", forceComplete.ToString().ToLower()}
                }
            );
        }

        public async Task<TaskOperationResult> DeleteTasks(string taskIds, bool forceComplete = false)
        {
            return await CallMethod<TaskOperationResult>("SYNO.DownloadStation.Task",
                "delete", new ReqParams
                {
                    {"id", taskIds},
                    {"force_complete", forceComplete.ToString().ToLower()}
                }
            );
        }

        public async Task<ListResult> GetTasks(string[] taskIds, string[] additional)
        {
            return await CallMethod<ListResult>("SYNO.DownloadStation.Task",
                "getinfo", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                    {"additional", string.Join(",", additional)}
                }
            );
        }

        public async Task<ListResult> GetTasks(string taskIds, string additional = "detail")
        {
            return await CallMethod<ListResult>("SYNO.DownloadStation.Task",
                "getinfo", new ReqParams
                {
                    {"id", taskIds},
                    {"additional", additional}
                }
            );
        }

        public async Task<TResult<object>> CreateTask(string url)
        {
            return await CallMethod<TResult<object>>("SYNO.DownloadStation.Task",
                "create", new ReqParams
                {
                    {"uri", url}
                }
            );
        }

        public async Task<TResult<object>> CreateTask(string fileName, Stream fileStream)
        {
            return await PostFile<TResult<object>>("SYNO.DownloadStation.Task", "create", fileName, fileStream);
        }
    }
}