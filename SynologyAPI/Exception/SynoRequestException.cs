using SynologyRestDAL;

namespace SynologyAPI.Exception
{
    public class SynoRequestException : System.Exception
    {
        public int ErrorCode { get; private set; }

        public SynoRequestException(string error, int errorCode)
            : base(error)
        {
            ErrorCode = errorCode;
        }
    }
}