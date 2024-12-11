namespace SharpGDX.Graphics.G3D.Models.Data;

public class ModelNodeKeyframe<T>
{
    /// <summary>
    ///     The timestamp of the keyframe in seconds.
    /// </summary>
    public float keytime;

    /// <summary>
    ///     The value of the keyframe.
    /// </summary>
    public T? value = default;
}