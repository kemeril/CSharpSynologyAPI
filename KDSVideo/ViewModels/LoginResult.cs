using System;
using System.Collections.Generic;
using System.Linq;
using SynologyAPI.Exception;
using SynologyAPI.SynologyRestDAL;
using SynologyAPI.SynologyRestDAL.Vs;

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
            Libraries = new List<Library>().AsReadOnly();
            Exception = exception;
        }

        public LoginInfo? LoginInfo { get; }
        public IReadOnlyCollection<Library> Libraries { get; }
        public Exception? Exception { get; }
        public bool Success => LoginInfo != null && Exception == null && Libraries.Any();

        public int ErrorCode
        {
            get
            {
                return Exception switch
                {
                    SynoRequestException synoRequestException => synoRequestException.ErrorCode,
                    LoginException loginException => loginException.ErrorCode,
                    _ => Success ? 0 : int.MinValue
                };
            }
        }

        public string ErrorMessage
        {
            get => ErrorCode switch
            {
                // Application level error codes
                ApplicationLevelErrorCodes.InvalidHost => ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode),
                ApplicationLevelErrorCodes.QuickConnectIsNotSupported => ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode),
                ApplicationLevelErrorCodes.OperationTimeOut => ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode),
                ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished => ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode),
                ApplicationLevelErrorCodes.NoVideoLibraries => ApplicationLevelErrorMessages.GetErrorMessage(ErrorCode),

                // Synology error codes
                ErrorCodes.TheAccountParameterIsNotSpecified => "The account is not specified.",
                ErrorCodes.InsufficientUserPrivilege => "Insufficient user privilege.",
                ErrorCodes.ConnectionTimeOut => "Connection time out.",
                ErrorCodes.MultipleLoginDetected => "Multiple login detected.",
                ErrorCodes.InvalidPassword => "Invalid password.",
                ErrorCodes.GuestOrDisabledAccount => "Guest or disabled account.",
                ErrorCodes.PermissionDenied => "Permission denied.",
                ErrorCodes.OneTimePasswordNotSpecified => "One time password not specified, 2-way authentication required.",
                ErrorCodes.OneTimePasswordAuthenticateFailed => "One time password authenticate failed while 2-way authentication process.",
                ErrorCodes.AppPortalIncorrect => "Application portal is incorrect.",
                ErrorCodes.OTPCodeEnforced => "One time password (OTP) code enforced.",
                ErrorCodes.MaxTries => "Maximum login attempt tries ha been reached.",
                ErrorCodes.PasswordExpiredCanNotChange => "Password is expired can not changed.",
                ErrorCodes.PasswordExpired => "Password is expired.",
                ErrorCodes.PasswordMustChange => "Password must change.",
                ErrorCodes.AccountLocked => "Account is locked.",
                _ => string.Empty
            };
        }
    }
}
