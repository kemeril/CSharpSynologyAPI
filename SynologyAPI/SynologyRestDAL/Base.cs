using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StdUtils;

#pragma warning disable IDE1006 // Naming Styles

namespace SynologyAPI.SynologyRestDAL
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable InconsistentNaming
    public static class ErrorCodes
    {
        #region Common

        /// <summary>
        /// Unknown error
        /// </summary>
        public const int UnknownError = 100;

        /// <summary>
        /// Invalid parameters.
        /// </summary>
        public const int InvalidParameters = 101;

        /// <summary>
        /// API does not exist
        /// </summary>
        public const int APIDoesNotExist = 102;

        /// <summary>
        /// Method does not exist
        /// </summary>
        public const int MethodDoesNotExist = 103;

        /// <summary>
        /// This API version is not supported
        /// </summary>
        public const int ThisAPIVersionIsNotSupported = 104;

        /// <summary>
        /// Insufficient user privilege
        /// </summary>
        public const int InsufficientUserPrivilege = 105;

        /// <summary>
        /// Connection time out
        /// </summary>
        public const int ConnectionTimeOut = 106;

        /// <summary>
        /// Multiple login detected
        /// </summary>
        public const int MultipleLoginDetected = 107;

        #endregion

        #region Authentication

        /// <summary>
        /// The account parameter is not specified.
        /// </summary>
        public const int TheAccountParameterIsNotSpecified = 101;

        /// <summary>
        /// Invalid password
        /// </summary>
        public const int InvalidPassword = 400;

        /// <summary>
        /// Guest or disabled account.
        /// </summary>
        public const int GuestOrDisabledAccount = 401;

        /// <summary>
        /// Permission denied.
        /// </summary>
        public const int PermissionDenied = 402;

        /// <summary>
        /// One time password not specified, 2-way authentication required.
        /// </summary>
        /// <remarks>
        /// Remark: 2-way-authentication = 2 factor authentication.
        /// </remarks>
        public const int OneTimePasswordNotSpecified = 403;

        /// <summary>
        /// One time password authenticate failed while 2-way authentication process.
        /// </summary>
        /// <remarks>
        /// Remark: 2-way-authentication = 2 factor authentication.
        /// </remarks>
        public const int OneTimePasswordAuthenticateFailed = 404;

        /// <summary>
        /// App portal incorrect.
        /// </summary>
        public const int AppPortalIncorrect = 405;

        /// <summary>
        /// One time password (OTP) code enforced.
        /// </summary>
        public const int OTPCodeEnforced = 406;

        /// <summary>
        /// Max Tries (if auto blocking is set to true).
        /// </summary>
        public const int MaxTries = 407;

        /// <summary>
        /// Password Expired Can not Change.
        /// </summary>
        public const int PasswordExpiredCanNotChange = 408;

        /// <summary>
        /// Password Expired.
        /// </summary>
        public const int PasswordExpired = 409;

        /// <summary>
        /// Password must change (when first time use or after reset password by admin).
        /// </summary>
        public const int PasswordMustChange = 410;

        /// <summary>
        /// Account Locked (when account max try exceed).
        /// </summary>
        public const int AccountLocked = 411;

        #endregion
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedMember.Global

    [DataContract]
    public class ErrorCode
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "error")]
        public ErrorCode Error { get; set; }
    }

    [DataContract]
    public class TResult<T> : Result
    {
        [DataMember(Name = "data")]
        public T Data { get; set; }
    }

    [DataContract]
    public class ApiSpec
    {
        [DataMember(Name = "maxVersion")]
        public int MaxVersion { get; set; }
        [DataMember(Name = "minVersion")]
        public int MinVersion { get; set; }
        [DataMember(Name = "path")]
        public string Path { get; set; }
    }

    [DataContract]
    public class ApiInfo : TResult<Dictionary<string, ApiSpec>>
    {
    }

    [DataContract]
    public class LoginInfo
    {
        /// <summary>
        /// Session ID, pass this value by HTTP argument "_sid" or Cookie argument. DSM 2 and onward.
        /// </summary>
        [DataMember(Name = "sid")]
        public string Sid { get; set; } = string.Empty;

        /// <summary>
        /// did, use to skip OTP checking. DSM 6 and onward.
        /// </summary>
        [DataMember(Name = "did")]
        [Obsolete]
        public string Did { get; set; } = string.Empty;

        /// <summary>
        /// Server side generated trusted Device id, use to skip OTP checking. DSM 6 and onward.
        /// </summary>
        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// Login through app portal. DSM 4 and onward.
        /// </summary>
        [DataMember(Name = "is_portal_port")]
        public string IsPortalPort { get; set; }
    }

    [DataContract]
    public class LoginResult : TResult<LoginInfo>
    {
    }

    [DataContract]
    public class EncryptionInfo
    {
        [DataMember(Name = "cipherkey")]
        public string CipherKey { get; set; }

        [DataMember(Name = "__cIpHeRtOkEn")]
        public string CipherToken { get; set; }

        [DataMember(Name = "public_key")]
        public string PublicKey { get; set; }

        [DataMember(Name = "server_time")]
        public long? OriginalServerTime { get; private set; }

        /// <summary>
        ///  Samples:
        /// 1554724904
        /// and it means: Mon Apr 08 2019 13:01:44 GMT+0200 (CEST)
        /// </summary>
        public DateTime? ServerTime
        {
            get => OriginalServerTime.HasValue
                ? DateTimeConverter.FromUnixTime(OriginalServerTime.Value)
                : (DateTime?)null;
            set => OriginalServerTime = value.HasValue
                ? DateTimeConverter.ToUnixTime(value.Value)
                : (long?)null;
        }
    }

    [DataContract]
    public class EncryptionInfoResult : TResult<EncryptionInfo>
    {
    }
}
#pragma warning restore IDE1006 // Naming Styles
