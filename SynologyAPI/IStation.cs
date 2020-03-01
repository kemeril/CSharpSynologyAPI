using System.Threading;
using System.Threading.Tasks;
using SynologyRestDAL;

namespace SynologyAPI
{
    public interface IStation
    {
        /// <summary>
        /// Logins to the Synology NAS.
        /// Support 2-way authentication.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="otpCode">
        /// 6-digit otp code. Optional. Available DSM 3 and onward.
        /// First try to login with optCode left null or empty string.
        /// If login has failed with error code 403 ask user for 6-digit optCode and try to login again but pass the optCode as well.
        /// 
        /// OptCode is used when 2-way authentication shall be used and the code can be obtained by Google 2-Step Authentication service.
        /// </param>
        /// <param name="deviceId">Device id (max: 255). Optional. Available DSM 6 and onward.</param>
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
        Task<LoginInfo> LoginAsync(string username, string password, string otpCode = null, string deviceId = null, CancellationToken cancellationToken = default);

        Task LogoutAsync(CancellationToken cancellationToken = default);
    }
}