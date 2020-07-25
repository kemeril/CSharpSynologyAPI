namespace SynologyAPI.Exception
{
    public class SynoRequestException : System.Exception
    {
        private const string Unknown = "Unknown";

        // ReSharper disable InconsistentNaming
        public string ApiName { get; }
        // ReSharper restore InconsistentNaming

        public string Method { get; }

        public int ErrorCode { get; }

        public SynoRequestException(string apiName, string method, int errorCode)
            : base($"Synology error. ApiName: {(string.IsNullOrWhiteSpace(apiName) ? Unknown : apiName)}. Method: {(string.IsNullOrWhiteSpace(method) ? Unknown : method)}. Error code: {errorCode}.")
        {
            ApiName = string.IsNullOrWhiteSpace(apiName) ? Unknown : apiName;
            Method = string.IsNullOrWhiteSpace(method) ? Unknown : method;
            ErrorCode = errorCode;
        }
    }
}