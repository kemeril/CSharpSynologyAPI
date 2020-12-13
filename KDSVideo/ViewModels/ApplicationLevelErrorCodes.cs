namespace KDSVideo.ViewModels
{
    internal class ApplicationLevelErrorCodes
    {
        /// <summary>
        /// Invalid host.
        /// </summary>
        public const int InvalidHost = -1;

        /// <summary>
        /// Operation time out.
        /// </summary>
        public const int OperationTimeOut = -2;

        /// <summary>
        /// A connection with the server could not be established.
        /// </summary>
        public const int ConnectionWithTheServerCouldNotBeEstablished = -3;

        /// <summary>
        /// There is no video libraries is available for the user logged in.
        /// </summary>
        public const int NoVideoLibraries = -4;
    }
}