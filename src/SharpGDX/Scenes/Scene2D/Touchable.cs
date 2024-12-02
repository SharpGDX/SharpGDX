namespace SharpGDX.Scenes.Scene2D;

/// <summary>
///     Determines how touch input events are distributed to an actor and any children.
/// </summary>
public enum Touchable
{
    /// <summary>
    ///     All touch input events will be received by the actor and any children.
    /// </summary>
    Enabled,

    /// <summary>
    ///     No touch input events will be received by the actor or any children.
    /// </summary>
    Disabled,

    /// <summary>
    ///     No touch input events will be received by the actor, but children will still receive events.
    /// </summary>
    /// <remarks>
    ///     Note that events on the children will still bubble to the parent.
    /// </remarks>
    ChildrenOnly
}