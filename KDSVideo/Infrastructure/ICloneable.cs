namespace KDSVideo.Infrastructure
{
    public interface ICloneable<T>
    {
        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        T Clone();
    }
}