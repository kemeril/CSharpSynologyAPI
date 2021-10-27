using SynologyAPI.Exception;
using System;
using SynologyAPI.SynologyRestDAL;

namespace KDSVideo.ViewModels
{
    public class EncryptionInfoResult
    {
        public EncryptionInfoResult(EncryptionInfo encryptionInfo)
        {
            EncryptionInfo = encryptionInfo;
            Exception = null;
        }

        public EncryptionInfoResult(Exception exception)
        {
            EncryptionInfo = null;
            Exception = exception;
        }

        public EncryptionInfo EncryptionInfo { get; }
        public Exception Exception { get; }
        public bool Success => EncryptionInfo != null && Exception == null;

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

        public string ErrorMessage => ApplicationLevelErrorMessages.GetErrorMessage(ApplicationLevelErrorCodes.UnknownError);
    }
}
