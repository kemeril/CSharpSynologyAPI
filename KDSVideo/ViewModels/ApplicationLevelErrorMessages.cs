namespace KDSVideo.ViewModels
{
    public static class ApplicationLevelErrorMessages
    {
        public static string GetErrorMessage(int errorCode) =>
            errorCode switch
            {
                // Application level error codes
                ApplicationLevelErrorCodes.InvalidHost => "Invalid host.",
                ApplicationLevelErrorCodes.QuickConnectIsNotSupported => "QuickConnect connection type is not supported.",
                ApplicationLevelErrorCodes.OperationTimeOut => "Operation time out.",
                ApplicationLevelErrorCodes.ConnectionWithTheServerCouldNotBeEstablished => "Connection with the server could not be established.",
                ApplicationLevelErrorCodes.NoVideoLibraries => "There is no video libraries is available for the user logged in.",
                ApplicationLevelErrorCodes.UnknownError => "Unknown error.",
                _ => string.Empty
            };
    }
}
