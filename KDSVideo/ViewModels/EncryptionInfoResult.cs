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

        public EncryptionInfo? EncryptionInfo { get; }
        public Exception? Exception { get; }
        public bool Success => EncryptionInfo != null && Exception == null;

        public int ErrorCode
        {
            get => Exception switch
            {
                SynoRequestException synoRequestException => synoRequestException.ErrorCode,
                LoginException loginException => loginException.ErrorCode,
                _ => Success ? 0 : int.MinValue
            };
        }

        public string ErrorMessage => ApplicationLevelErrorMessages.GetErrorMessage(ApplicationLevelErrorCodes.UNKNOWN_ERROR);
    }
}
