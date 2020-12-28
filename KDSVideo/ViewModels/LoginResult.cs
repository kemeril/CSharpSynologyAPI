using System;
using System.Collections.Generic;
using System.Linq;
using SynologyAPI.Exception;
using SynologyRestDAL;
using SynologyRestDAL.Vs;

namespace KDSVideo.ViewModels
{
    public class LoginResult
    {
        public LoginResult(LoginInfo loginInfo, IReadOnlyCollection<Library> libraries)
        {
            LoginInfo = loginInfo;
            Libraries = libraries;
            Exception = null;
        }

        public LoginResult(Exception exception)
        {
            LoginInfo = null;
            Libraries = null;
            Exception = exception;
        }

        public LoginInfo LoginInfo { get; }
        public IReadOnlyCollection<Library> Libraries { get; }
        public Exception Exception { get; }
        public bool Success => LoginInfo != null && Exception == null && Libraries != null && Libraries.Any();

        public int ErrorCode
        {
            get
            {
                if (Exception is SynoRequestException synoRequestException)
                {
                    return synoRequestException.ErrorCode;
                }

                if (Exception is LoginException loginException)
                {
                    return loginException.ErrorCode;
                }

                return Success ? 0 : int.MinValue;
            }
        }

        public string ErrorMessage
        {
            get
            {
                switch (ErrorCode)
                {
                    // Application level error codes
                    case ApplicationLevelErrorCodes.InvalidHost:
                    case ApplicationLevelErrorCodes.QuickConnectIsNotSupported:
                    case ApplicationLevelErrorCodes.OperationTimeOut:
                    case ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished:
                    case ApplicationLevelErrorCodes.NoVideoLibraries:
                        return ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode);

                    // Synology error codes
                    case ErrorCodes.TheAccountParameterIsNotSpecified: return "The account is not specified.";
                    case ErrorCodes.InsufficientUserPrivilege: return "Insufficient user privilege.";
                    case ErrorCodes.ConnectionTimeOut: return "Connection time out.";
                    case ErrorCodes.MultipleLoginDetected: return "Multiple login detected.";

                    case ErrorCodes.InvalidPassword: return "Invalid password.";
                    case ErrorCodes.GuestOrDisabledAccount: return "Guest or disabled account.";
                    case ErrorCodes.PermissionDenied: return "Permission denied.";
                    
                    case ErrorCodes.OneTimePasswordNotSpecified: return "One time password not specified, 2-way authentication required.";
                    case ErrorCodes.OneTimePasswordAuthenticateFailed: return "One time password authenticate failed while 2-way authentication process.";
                    
                    case ErrorCodes.AppPortalIncorrect: return "Application portal is incorrect.";
                    case ErrorCodes.OTPCodeEnforced: return "One time password (OTP) code enforced.";

                    case ErrorCodes.MaxTries: return "Maximum login attempt tries ha been reached.";
                    case ErrorCodes.PasswordExpiredCanNotChange: return "Password is expired can not changed.";
                    case ErrorCodes.PasswordExpired: return "Password is expired.";
                    case ErrorCodes.PasswordMustChange: return "Password must change.";
                    case ErrorCodes.AccountLocked: return "Account is locked.";

                    default: return string.Empty;
                }
            }
        }
    }
}