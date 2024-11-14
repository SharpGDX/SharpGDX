using SharpGDX.Graphics;

namespace SharpGDX;

/// <summary>
///     Environment class holding references to the <see cref="IApplication" />, <see cref="IGraphics" />,
///     <see cref="IAudio" />, <see cref="IFiles" /> and <see cref="IInput" /> instances.
/// </summary>
/// <remarks>
///     <para>
///         The references are held in public static fields which allows static access to all sub systems. Do not use
///         <see cref="Graphics" /> in a thread that is not the rendering thread.
///     </para>
///     <para>
///         This is normally a design faux pas but in this case is better than the alternatives.
///     </para>
/// </remarks>
public class Gdx
{
    public static IApplication App = null!;
    public static IAudio Audio = null!;
    public static IFiles Files = null!;
    public static IGL20 GL = null!;
    public static IGL20 GL20 = null!;
    public static IGL30 GL30 = null!;
    public static IGL31 GL31 = null!;
    public static IGL32 GL32 = null!;
    public static IGraphics Graphics = null!;
    public static IInput Input = null!;
    public static INet Net = null!;
}