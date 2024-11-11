using SharpGDX.Graphics;
using SharpGDX.Input;

namespace SharpGDX;

/// <summary>
///     Environment class holding references to the {@link Application}, {@link Graphics}, {@link Audio}, {@link Files} and
///     {@link Input} instances.
/// </summary>
/// <remarks>
///     <para>
///         The references are held in public static fields which allows static access to all sub systems. Do not use
///         Graphics in a thread that is not the rendering thread.
///     </para>
///     <para>
///         This is normally a design faux pas but in this case is better than the alternatives.
///     </para>
/// </remarks>
public class Gdx
{
    public static IApplication App;
    public static IAudio Audio;
    public static IFiles Files;
    public static IGL20 GL;
    public static IGL20 GL20;
    public static IGL30 GL30;
    public static IGL31 GL31;
    public static IGL32 GL32;
    public static IGraphics Graphics;
    public static IInput Input;
    public static INet Net;
}