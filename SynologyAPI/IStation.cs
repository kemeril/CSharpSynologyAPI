using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SynologyRestDAL;

namespace SynologyAPI
{
    public interface IStation
    {
        /// <summary>
        /// Gets Encryption information. This information can used by <see cref="LoginAsync"/> method cipherText param.
        /// </summary>
        /// <param name="baseUri">The VideoStation base uri. Required. Does not store <paramref name="baseUri"/> for further operations.</param>
        /// <param name="id">Authentication Id. Sample value: i_pI9DZgwA-PXYIvIkqrbWRbP6A5QTUKGCNA2xAvR347RigtO9QsMUQO5u0crwrW2lWGaW2406BhQTIi5H7nfI</param>
        /// <param name="deviceId">Device id (max: 255). Optional. Available DSM 6 and onward.
        /// Sample value: 51JDCuT81kbudcKWxgP4MFko142njD8sQ2x7ey1dO04ZwG3cRtHtvBZT0EyoAv6YiFzPcfQvLWN7tnOEfKwdXxbLsluoiR9XnYafddNAOLTkMfAbnLDr4uiiqLQ6yfD</param>
        /// <param name="proxy">Optional.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        /// <exception cref="SynologyAPI.Exception.SynoRequestException">Synology NAS returns an error.</exception>
        Task<EncryptionInfo> GetEncryptionInfoAsync(Uri baseUri, string id, string deviceId, IWebProxy proxy = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logins to the Synology NAS.
        /// Support 2-way authentication.
        /// </summary>
        /// <param name="baseUri">The VideoStation base url. Required. Stores <paramref name="baseUri"/> for further operations.</param>
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
