using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SynologyAPI
{
    public static class Extensions
    {
        public static async Task<WebResponse> GetResponseAsync(this WebRequest request, CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(request.Abort, useSynchronizationContext: false))
            {
                try
                {
                    return await request.GetResponseAsync();
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, cancellationToken);
                    }

                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}