using SynologyAPI.Exception;
using System;

namespace KDSVideo.ViewModels
{
    public class EncryptionInfoResult
    {
        public EncryptionInfoResult(SynologyRestDAL.EncryptionInfo encryptionInfo)
        {
            EncryptionInfo = encryptionInfo;
            Exception = null;
        }

        public EncryptionInfoResult(Exception exception)
        {
            EncryptionInfo = null;
            Exception = exception;
        }

        public SynologyRestDAL.EncryptionInfo EncryptionInfo { get; }
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
