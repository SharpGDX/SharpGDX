using SharpGDX.Graphics;
using SharpGDX.Input;

namespace SharpGDX
{
    /** Environment class holding references to the {@link Application}, {@link Graphics}, {@link Audio}, {@link Files} and
 * {@link Input} instances. The references are held in public static fields which allows static access to all sub systems. Do not
 * use Graphics in a thread that is not the rendering thread.
 * <p>
 * This is normally a design faux pas but in this case is better than the alternatives.
 * @author mzechner */
    public class Gdx
    {
        public static IApplication App;
        public static IGraphics Graphics;
        public static IAudio Audio;
        public static IInput Input;
        public static IFiles Files;
        public static INet Net;

        public static IGL20 GL;
        public static IGL20 GL20;
        public static IGL30 GL30;
        public static IGL31 GL31;
        public static IGL32 GL32;
    }
}
