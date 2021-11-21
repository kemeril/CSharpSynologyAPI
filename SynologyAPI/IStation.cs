using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SynologyAPI.SynologyRestDAL;

namespace SynologyAPI
{
    public interface IStation
    {
        /// <summary>
        /// Gets Encryption information. This information can used by <see cref="LoginAsync"/> method cipherText param.
        /// </summary>
        /// <param name="baseUri">The VideoStation base uri. Required. Does not store <paramref name="baseUri"/> for further operations.</param>
        /// <param name="proxy">Optional.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException">Synology NAS returns an error.</exception>
        Task<EncryptionInfo> GetEncryptionInfoAsync(Uri baseUri, IWebProxy proxy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logins to the Synology NAS.
        /// Support 2-way authentication.
        /// </summary>
        /// <param name="baseUri">The VideoStation base url. Required. Stores <paramref name="baseUri"/> for further operations.</param>
        /// <param name="username">
        /// The login account username. Required.
        /// Available DSM 3 and onward.
        /// </param>
        /// <param name="password">
        /// The login account password. Required.
        /// Available DSM 3 and onward.
        /// </param>
        /// <param name="otpCode">
        /// 6-digit otp code. Optional. Available DSM 3 and onward.
        /// First try to login with optCode left null or empty string.
        /// If login has failed with error code 403 ask user for 6-digit optCode and try to login again but pass the optCode as well.
        /// 
        /// OptCode is used when 2-way authentication shall be used and the code can be obtained by Google 2-Step Authentication service.
        /// Optional - 2-factor authentication option with an OTP code. If it’s enabled, the user requires a verification code to log into DSM sessions.
        /// Available DSM 3 and onward.
        /// </param>
        /// <param name="deviceId">
        /// Device id.
        /// Optional - If 2-factor authentication (OTP) has omitted the same enabled device id, pass this value to skip it.
        /// Available DSM 6 and onward.
        /// </param>
        /// <param name="deviceName">
        /// Device name.
        /// Optional - To identify which device can be omitted from 2-factor authentication (OTP), pass this value will skip it.
        /// Available DSM 6 and onward.
        /// </param>
        /// <param name="cipherText">Optional.</param>
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
        /// <remarks>
        /// Login Api:  https://global.download.synology.com/download/Document/Software/DeveloperGuide/Os/DSM/All/enu/DSM_Login_Web_API_Guide_enu.pdf
        /// </remarks>
        Task<LoginInfo> LoginAsync(Uri baseUri, string username, string password, string otpCode = null, string deviceId = null, string deviceName = null, string cipherText = null, IWebProxy proxy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException">Synology NAS returns an error.</exception>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Clear cookies.
        /// </summary>
        void ClearCookies();
    }
}
