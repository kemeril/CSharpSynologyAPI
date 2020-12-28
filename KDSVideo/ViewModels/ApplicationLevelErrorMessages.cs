namespace KDSVideo.ViewModels
{
    public static class ApplicationLevelErrorMessages
    {
        public static string GetErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                // Application level error codes
                case ApplicationLevelErrorCodes.InvalidHost: return "Invalid host.";
                case ApplicationLevelErrorCodes.QuickConnectIsNotSupported: return "QuickConnect connection type is not supported.";
                case ApplicationLevelErrorCodes.OperationTimeOut: return "Operation time out.";
                case ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished: return "Connection with the server could not be established.";
                case ApplicationLevelErrorCodes.NoVideoLibraries: return "There is no video libraries is available for the user logged in.";
                case ApplicationLevelErrorCodes.UnknownError: return "Unknown error.";
                default:
                    return string.Empty;
            }
        }
    }
}