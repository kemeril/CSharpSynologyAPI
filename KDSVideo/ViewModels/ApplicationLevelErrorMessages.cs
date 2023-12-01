namespace KDSVideo.ViewModels
{
    public static class ApplicationLevelErrorMessages
    {
        public static string GetErrorMessage(int errorCode) =>
            errorCode switch
            {
                // Application level error codes
                ApplicationLevelErrorCodes.INVALID_HOST => "Invalid host.",
                ApplicationLevelErrorCodes.QUICK_CONNECT_IS_NOT_SUPPORTED => "QuickConnect connection type is not supported.",
                ApplicationLevelErrorCodes.OPERATION_TIME_OUT => "Operation time out.",
                ApplicationLevelErrorCodes.CONNECTION_WITH_THE_SERVER_COULD_NOT_BE_ESTABLISHED => "Connection with the server could not be established.",
                ApplicationLevelErrorCodes.NO_VIDEO_LIBRARIES => "No video libraries are available for the user logged in.",
                ApplicationLevelErrorCodes.UNKNOWN_ERROR => "Unknown error.",
                _ => string.Empty
            };
    }
}
