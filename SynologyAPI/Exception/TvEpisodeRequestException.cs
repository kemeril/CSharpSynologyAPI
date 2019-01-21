using SynologyRestDAL;

namespace SynologyAPI.Exception
{
    public class SynoRequestException : System.Exception
    {
        public SynoRequestException(string error)
            : base(error)
        {
            
        }
    }
}