namespace KDSVideo.ViewModels
{
    internal class ApplicationLevelErrorCodes
    {
        /// <summary>
        /// Invalid host.
        /// </summary>
        public const int INVALID_HOST = -1;

        /// <summary>
        /// QuickConnect connection type is not supported.
        /// </summary>
        public const int QUICK_CONNECT_IS_NOT_SUPPORTED = -2;

        /// <summary>
        /// Operation time out.
        /// </summary>
        public const int OPERATION_TIME_OUT = -3;

        /// <summary>
        /// Connection with the server could not be established.
        /// </summary>
        public const int CONNECTION_WITH_THE_SERVER_COULD_NOT_BE_ESTABLISHED = -4;

        /// <summary>
        /// There is no video libraries is available for the user logged in.
        /// </summary>
        public const int NO_VIDEO_LIBRARIES = -5;

        /// <summary>
        /// Unknown error.
        /// </summary>
        public const int UNKNOWN_ERROR = int.MinValue;
    }
}
