namespace KDSVideo.Infrastructure
{
    public interface IAuthenticationIdProvider
    {
        /// <summary>
        /// Create new AuthenticationId for "SYNO.API.Encryption" and "SYNO.API.Auth" web API.
        /// Sample return value: "i_pI9DZgwA-PXYIvIkqrbWRbP6A5QTUKGCNA2xAvR347RigtO9QsMUQO5u0crwrW2lWGaW2406BhQTIi5H7nfI"
        /// </summary>
        /// <returns>The authentication Id.</returns>
        string GetNewAuthenticationId();
    }
}