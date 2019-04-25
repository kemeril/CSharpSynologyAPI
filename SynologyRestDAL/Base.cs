using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SynologyRestDAL
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
        public int Code;
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
        public T Data  { get; set; }
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
    public class SidContainer
    {
        [DataMember(Name = "sid")]
        public string Sid { get; set; }
    }

    [DataContract]
    public class LoginResult : TResult<SidContainer>
    {
    }
}