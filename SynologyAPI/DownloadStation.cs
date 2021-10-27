using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SynologyAPI.SynologyRestDAL;
using SynologyAPI.SynologyRestDAL.Ds;

namespace SynologyAPI
{
    public sealed class DownloadStation : Station
    {
        protected override string GetSessionName()
        {
            return "DownloadStation";
        }

        protected override Dictionary<string, int> GetImplementedApi()
        {
            var implementedApi = base.GetImplementedApi();
            implementedApi.Add("SYNO.DownloadStation.Task", 3);
            implementedApi.Add("SYNO.DownloadStation.Info", 3);
            return implementedApi;
        }

        public async Task<InfoResult> InfoAsync(CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<InfoResult>("SYNO.DownloadStation.Info", "getinfo", cancellationToken).ConfigureAwait(false);
        }

        public async Task<ListResult> ListAsync(CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<ListResult>("SYNO.DownloadStation.Task", "list", cancellationToken).ConfigureAwait(false);
        }

        public async Task<ListResult> ListAsync(string[] additional, int offset = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<ListResult>("SYNO.DownloadStation.Task",
                "list", new ReqParams
                {
                    {"additional", string.Join(",", additional)},
                    {"offset", offset.ToString()},
                    {"limit", limit.ToString()}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<ListResult> ListAsync(string additional, int offset = 0, int limit = -1, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<ListResult>("SYNO.DownloadStation.Task",
                "list", new ReqParams
                    {
                        {"additional", additional},
                        {"offset", offset.ToString()},
                        {"limit", limit.ToString()}
                    },
                    cancellationToken
           ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> PauseTasksAsync(string[] taskIds, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "pause", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                }
                , cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> PauseTasksAsync(string taskIds, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "pause", new ReqParams
                {
                    {"id", taskIds},
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> ResumeTasksAsync(string[] taskIds, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "resume", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> ResumeTasksAsync(string taskIds, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "resume", new ReqParams
                {
                    {"id", taskIds},
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> DeleteTasks(string[] taskIds, bool forceComplete = false, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "delete", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                    {"force_complete", forceComplete.ToString().ToLower()}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TaskOperationResult> DeleteTasksAsync(string taskIds, bool forceComplete = false, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TaskOperationResult>("SYNO.DownloadStation.Task",
                "delete", new ReqParams
                {
                    {"id", taskIds},
                    {"force_complete", forceComplete.ToString().ToLower()}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<ListResult> GetTasksAsync(string[] taskIds, string[] additional, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<ListResult>("SYNO.DownloadStation.Task",
                "getinfo", new ReqParams
                {
                    {"id", string.Join(",", taskIds)},
                    {"additional", string.Join(",", additional)}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<ListResult> GetTasksAsync(string taskIds, string additional = "detail", CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<ListResult>("SYNO.DownloadStation.Task",
                "getinfo", new ReqParams
                {
                    {"id", taskIds},
                    {"additional", additional}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TResult<object>> CreateTaskAsync(string url, CancellationToken cancellationToken = default)
        {
            return await CallMethodAsync<TResult<object>>("SYNO.DownloadStation.Task",
                "create", new ReqParams
                {
                    {"uri", url}
                },
                cancellationToken
            ).ConfigureAwait(false);
        }

        public async Task<TResult<object>> CreateTaskAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
        {
            return await PostFileAsync<TResult<object>>("SYNO.DownloadStation.Task", "create", fileName, fileStream, "file", cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
