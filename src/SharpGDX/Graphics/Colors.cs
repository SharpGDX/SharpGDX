using SharpGDX.Utils;
using SharpGDX.Shims;

namespace SharpGDX.Graphics
{
	/** A general purpose class containing named colors that can be changed at will. For example, the markup language defined by the
 * {@code BitmapFontCache} class uses this class to retrieve colors and the user can define his own colors.
 * 
 * @author davebaol */
public sealed class Colors {

	private static readonly ObjectMap<String, Color> map = new ObjectMap<String, Color>();

	static Colors() {
		reset();
	}

	/** Returns the color map. */
	public static ObjectMap<String, Color> getColors () {
		return map;
	}

	/** Convenience method to lookup a color by {@code name}. The invocation of this method is equivalent to the expression
	 * {@code Colors.getColors().get(name)}
	 * 
	 * @param name the name of the color
	 * @return the color to which the specified {@code name} is mapped, or {@code null} if there was no mapping for {@code name}
	 *         . */
	public static Color get (String name) {
		return map.get(name);
	}

	/** Convenience method to add a {@code color} with its {@code name}. The invocation of this method is equivalent to the
	 * expression {@code Colors.getColors().put(name, color)}
	 * 
	 * @param name the name of the color
	 * @param color the color
	 * @return the previous {@code color} associated with {@code name}, or {@code null} if there was no mapping for {@code name}
	 *         . */
	public static Color put (String name, Color color) {
		return map.put(name, color);
	}

	/** Resets the color map to the predefined colors. */
	public static void reset () {
		map.clear();
		map.put("CLEAR", Color.Clear);
        map.put("CLEAR_WHITE", Color.ClearWhite);
            map.put("BLACK", Color.Black);

		map.put("WHITE", Color.White);
		map.put("LIGHT_GRAY", Color.LightGray);
		map.put("GRAY", Color.Gray);
		map.put("DARK_GRAY", Color.DarkGray);

		map.put("BLUE", Color.Blue);
		map.put("NAVY", Color.Navy);
		map.put("ROYAL", Color.Royal);
		map.put("SLATE", Color.Slate);
		map.put("SKY", Color.Sky);
		map.put("CYAN", Color.Cyan);
		map.put("TEAL", Color.Teal);

		map.put("GREEN", Color.Green);
		map.put("CHARTREUSE", Color.Chartreuse);
		map.put("LIME", Color.Lime);
		map.put("FOREST", Color.Forest);
		map.put("OLIVE", Color.Olive);

		map.put("YELLOW", Color.Yellow);
		map.put("GOLD", Color.Gold);
		map.put("GOLDENROD", Color.Goldenrod);
		map.put("ORANGE", Color.Orange);

		map.put("BROWN", Color.Brown);
		map.put("TAN", Color.Tan);
		map.put("FIREBRICK", Color.Firebrick);

		map.put("RED", Color.Red);
		map.put("SCARLET", Color.Scarlet);
		map.put("CORAL", Color.Coral);
		map.put("SALMON", Color.Salmon);
		map.put("PINK", Color.Pink);
		map.put("MAGENTA", Color.Magenta);

		map.put("PURPLE", Color.Purple);
		map.put("VIOLET", Color.Violet);
		map.put("MAROON", Color.Maroon);
	}

	private Colors () {
	}

}
}
