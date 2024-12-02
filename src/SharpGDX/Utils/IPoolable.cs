namespace SharpGDX.Utils;

/// <summary>
///     Objects implementing this interface will have <see cref="Reset()" /> called when passed to
///     <see cref="Pool.free(Object)" />.
/// </summary>
public interface IPoolable
{
    /// <summary>
    ///     Resets the object for reuse.
    /// </summary>
    /// <remarks>
    ///     Object references should be set to <see langword="null" /> and fields may be set to default values.
    /// </remarks>
    public void Reset();
}