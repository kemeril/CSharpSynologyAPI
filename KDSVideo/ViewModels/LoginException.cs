namespace KDSVideo.ViewModels
{
    public class LoginException : System.Exception
    {
        public int ErrorCode { get; }

        public LoginException(int errorCode)
            : base($"Application level login error. Error code: {errorCode}.")
        {
            ErrorCode = errorCode;
        }
    }
}