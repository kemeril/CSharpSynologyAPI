namespace KDSVideo.Infrastructure
{
    public class QuickConnectLoginNotSupportedException : System.Exception
    {
        public QuickConnectLoginNotSupportedException()
            : base($"QuickConnect connection type is not supported.")
        {
        }
    }
}
