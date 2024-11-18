using SharpGDX.Mathematics;
using SharpGDX.Utils;

namespace SharpGDX.Graphics;

/// <summary>
///     A color class, holding the r, g, b and alpha component as floats in the range [0,1].
/// </summary>
/// <remarks>
///     All methods perform clamping on the internal values after execution.
/// </remarks>
public class Color
{
    public static readonly Color Black          = new(0, 0, 0, 1);
    public static readonly Color Blue           = new(0, 0, 1, 1);
    public static readonly Color Brown          = new(unchecked((int)0x8b4513ff));
    public static readonly Color Chartreuse     = new(0x7fff00ff);
    public static readonly Color Clear          = new(0, 0, 0, 0);
    public static readonly Color ClearWhite     = new(1, 1, 1, 0);
    public static readonly Color Coral          = new(unchecked((int)0xff7f50ff));
    public static readonly Color Cyan           = new(0, 1, 1, 1);
    public static readonly Color DarkGray       = new(0x3f3f3fff);
    public static readonly Color Firebrick      = new(unchecked((int)0xb22222ff));
    public static readonly Color Forest         = new(0x228b22ff);
    public static readonly Color Gold           = new(unchecked((int)0xffd700ff));
    public static readonly Color Goldenrod      = new(unchecked((int)0xdaa520ff));
    public static readonly Color Gray           = new(0x7f7f7fff);
    public static readonly Color Green          = new(0x00ff00ff);
    public static readonly Color LightGray      = new(unchecked((int)0xbfbfbfff));
    public static readonly Color Lime           = new(0x32cd32ff);
    public static readonly Color Magenta        = new(1, 0, 1, 1);
    public static readonly Color Maroon         = new(unchecked((int)0xb03060ff));
    public static readonly Color Navy           = new(0, 0, 0.5f, 1);
    public static readonly Color Olive          = new(0x6b8e23ff);
    public static readonly Color Orange         = new(unchecked((int)0xffa500ff));
    public static readonly Color Pink           = new(unchecked((int)0xff69b4ff));
    public static readonly Color Purple         = new(unchecked((int)0xa020f0ff));
    public static readonly Color Red            = new(unchecked((int)0xff0000ff));
    public static readonly Color Royal          = new(0x4169e1ff);
    public static readonly Color Salmon         = new(unchecked((int)0xfa8072ff));
    public static readonly Color Scarlet        = new(unchecked((int)0xff341cff));
    public static readonly Color Sky            = new(unchecked((int)0x87ceebff));
    public static readonly Color Slate          = new(0x708090ff);
    public static readonly Color Tan            = new(unchecked((int)0xd2b48cff));
    public static readonly Color Teal           = new(0, 0.5f, 0.5f, 1);
    public static readonly Color Violet         = new(unchecked((int)0xee82eeff));
    public static readonly Color White          = new(1, 1, 1, 1);
    public static readonly float WhiteFloatBits = White.ToFloatBits();
    public static readonly Color Yellow         = new(unchecked((int)0xffff00ff));

    /// <summary>
    ///     The alpha component.
    /// </summary>
    public float A;

    /// <summary>
    ///     The blue component.
    /// </summary>
    public float B;

    /// <summary>
    ///     The green component.
    /// </summary>
    public float G;

    /// <summary>
    ///     The red component.
    /// </summary>
    public float R;

    /// <summary>
    ///     Constructs a new Color with all components set to 0.
    /// </summary>
    public Color()
    {
    }

    /// <summary>
    ///     See <see cref="RGBA8888ToColor" />
    /// </summary>
    /// <param name="rgba8888"></param>
    public Color(int rgba8888)
    {
        RGBA8888ToColor(this, rgba8888);
    }

    /// <summary>
    ///     Constructor, sets the components of the color.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    public Color(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;

        Clamp();
    }

    /// <summary>
    ///     Constructs a new color using the given color.
    /// </summary>
    /// <param name="color">This color.</param>
    public Color(Color color)
    {
        Set(color);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format ABGR8888.
    /// </summary>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value"></param>
    public static void ABGR8888ToColor(Color color, int value)
    {
        color.A = ((value & 0xff000000) >>> 24) / 255f;
        color.B = ((value & 0x00ff0000) >>> 16) / 255f;
        color.G = ((value & 0x0000ff00) >>> 8) / 255f;
        color.R = (value & 0x000000ff) / 255f;
    }

    /// <summary>
    ///     Sets the Color components using the specified float value in the format ABGR8888.
    /// </summary>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value"></param>
    public static void ABGR8888ToColor(Color color, float value)
    {
        var c = NumberUtils.floatToIntColor(value);
        color.A = ((c & 0xff000000) >>> 24) / 255f;
        color.B = ((c & 0x00ff0000) >>> 16) / 255f;
        color.G = ((c & 0x0000ff00) >>> 8) / 255f;
        color.R = (c & 0x000000ff) / 255f;
    }

    public static int Alpha(float alpha)
    {
        return (int)(alpha * 255.0f);
    }

    public static int ARGB8888(float a, float r, float g, float b)
    {
        return ((int)(a * 255) << 24) | ((int)(r * 255) << 16) | ((int)(g * 255) << 8) | (int)(b * 255);
    }

    public static int ARGB8888(Color color)
    {
        return ((int)(color.A * 255) << 24) | ((int)(color.R * 255) << 16) | ((int)(color.G * 255) << 8) |
               (int)(color.B * 255);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format ARGB8888.
    /// </summary>
    /// <remarks>
    ///     This is the inverse to the argb8888(a, r, g, b) method.
    /// </remarks>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value">An integer color value in ARGB8888 format.</param>
    public static void ARGB8888ToColor(Color color, int value)
    {
        color.A = ((value & 0xff000000) >>> 24) / 255f;
        color.R = ((value & 0x00ff0000) >>> 16) / 255f;
        color.G = ((value & 0x0000ff00) >>> 8) / 255f;
        color.B = (value & 0x000000ff) / 255f;
    }

    public static int LuminanceAlpha(float luminance, float alpha)
    {
        return ((int)(luminance * 255.0f) << 8) | (int)(alpha * 255);
    }

    public static int RGB565(float r, float g, float b)
    {
        return ((int)(r * 31) << 11) | ((int)(g * 63) << 5) | (int)(b * 31);
    }

    public static int RGB565(Color color)
    {
        return ((int)(color.R * 31) << 11) | ((int)(color.G * 63) << 5) | (int)(color.B * 31);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format RGB565.
    /// </summary>
    /// <remarks>
    ///     This is inverse to the <see cref="RGB565(float, float, float)" /> method.
    /// </remarks>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value">An integer color value in RGB565 format.</param>
    public static void RGB565ToColor(Color color, int value)
    {
        color.R = ((value & 0x0000F800) >>> 11) / 31f;
        color.G = ((value & 0x000007E0) >>> 5) / 63f;
        color.B = ((value & 0x0000001F) >>> 0) / 31f;
    }

    public static int RGB888(float r, float g, float b)
    {
        return ((int)(r * 255) << 16) | ((int)(g * 255) << 8) | (int)(b * 255);
    }

    public static int RGB888(Color color)
    {
        return ((int)(color.R * 255) << 16) | ((int)(color.G * 255) << 8) | (int)(color.B * 255);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format RGB888.
    /// </summary>
    /// <remarks>
    ///     This is inverse to the rgb888(r, g, b) method.
    /// </remarks>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value">An integer color value in RGB888 format.</param>
    public static void RGB888ToColor(Color color, int value)
    {
        color.R = ((value & 0x00ff0000) >>> 16) / 255f;
        color.G = ((value & 0x0000ff00) >>> 8) / 255f;
        color.B = (value & 0x000000ff) / 255f;
    }

    public static int RGBA4444(float r, float g, float b, float a)
    {
        return ((int)(r * 15) << 12) | ((int)(g * 15) << 8) | ((int)(b * 15) << 4) | (int)(a * 15);
    }

    public static int RGBA4444(Color color)
    {
        return ((int)(color.R * 15) << 12) | ((int)(color.G * 15) << 8) | ((int)(color.B * 15) << 4) |
               (int)(color.A * 15);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format RGBA4444.
    /// </summary>
    /// <remarks>
    ///     This is inverse to the <see cref="RGBA4444(float, float, float, float)" /> method.
    /// </remarks>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value">An integer color value in RGBA4444 format.</param>
    public static void RGBA4444ToColor(Color color, int value)
    {
        color.R = ((value & 0x0000f000) >>> 12) / 15f;
        color.G = ((value & 0x00000f00) >>> 8) / 15f;
        color.B = ((value & 0x000000f0) >>> 4) / 15f;
        color.A = (value & 0x0000000f) / 15f;
    }

    public static int RGBA8888(float r, float g, float b, float a)
    {
        return ((int)(r * 255) << 24) | ((int)(g * 255) << 16) | ((int)(b * 255) << 8) | (int)(a * 255);
    }

    public static int RGBA8888(Color color)
    {
        return ((int)(color.R * 255) << 24) | ((int)(color.G * 255) << 16) | ((int)(color.B * 255) << 8) |
               (int)(color.A * 255);
    }

    /// <summary>
    ///     Sets the Color components using the specified integer value in the format RGBA8888.
    /// </summary>
    /// <remarks>
    ///     This is inverse to the rgba8888(r, g, b, a) method.
    /// </remarks>
    /// <param name="color">The Color to be modified.</param>
    /// <param name="value">An integer color value in RGBA8888 format.</param>
    public static void RGBA8888ToColor(Color color, int value)
    {
        color.R = ((value & 0xff000000) >>> 24) / 255f;
        color.G = ((value & 0x00ff0000) >>> 16) / 255f;
        color.B = ((value & 0x0000ff00) >>> 8) / 255f;
        color.A = (value & 0x000000ff) / 255f;
    }

    /// <summary>
    ///     Packs the color components into a 32-bit integer with the format ABGR and then converts it to a float.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Note that no range checking is performed for higher performance.
    ///     </para>
    ///     <para>
    ///         See <see cref="NumberUtils.intToFloatColor(int)" />.
    ///     </para>
    /// </remarks>
    /// <param name="r">The red component, 0 - 255.</param>
    /// <param name="g">The green component, 0 - 255.</param>
    /// <param name="b">The blue component, 0 - 255.</param>
    /// <param name="a">The alpha component, 0 - 255.</param>
    /// <returns>The packed color as a float.</returns>
    public static float ToFloatBits(int r, int g, int b, int a)
    {
        var color = (a << 24) | (b << 16) | (g << 8) | r;
        var floatColor = NumberUtils.intToFloatColor(color);

        return floatColor;
    }

    /// <summary>
    ///     Packs the color components into a 32-bit integer with the format ABGR and then converts it to a float.
    /// </summary>
    /// <remarks>
    ///     See <see cref="NumberUtils.intToFloatColor(int)" />
    /// </remarks>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns>The packed color as a 32-bit float.</returns>
    public static float ToFloatBits(float r, float g, float b, float a)
    {
        var color = ((int)(255 * a) << 24) | ((int)(255 * b) << 16) | ((int)(255 * g) << 8) | (int)(255 * r);

        return NumberUtils.intToFloatColor(color);
    }

    /// <summary>
    ///     Packs the color components into a 32-bit integer with the format ABGR.
    /// </summary>
    /// <remarks>
    ///     Note that no range checking is performed for higher performance.
    /// </remarks>
    /// <param name="r">The red component, 0 - 255.</param>
    /// <param name="g">The green component, 0 - 255.</param>
    /// <param name="b">The blue component, 0 - 255.</param>
    /// <param name="a">The alpha component, 0 - 255.</param>
    /// <returns>The packed color as a 32-bit int.</returns>
    public static int ToIntBits(int r, int g, int b, int a)
    {
        return (a << 24) | (b << 16) | (g << 8) | r;
    }

    /// <summary>
    ///     Returns a new color from a hex string with the format RRGGBBAA.
    /// </summary>
    /// <remarks>
    ///     See <see cref="ToString()" />
    /// </remarks>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color ValueOf(string hex)
    {
        return ValueOf(hex, new Color());
    }

    /// <summary>
    ///     Sets the specified color from a hex string with the format RRGGBBAA.
    /// </summary>
    /// <remarks>
    ///     See <see cref="ToString()" />
    /// </remarks>
    /// <param name="hex"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ValueOf(string hex, Color color)
    {
        hex = hex[0] == '#' ? hex.Substring(1) : hex;
        color.R = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        color.G = Convert.ToInt32(hex.Substring(2, 4), 16) / 255f;
        color.B = Convert.ToInt32(hex.Substring(4, 6), 16) / 255f;
        color.A = hex.Length != 8 ? 1 : Convert.ToInt32(hex.Substring(6, 8), 16) / 255f;

        return color;
    }

    /// <summary>
    ///     Adds the given color to this color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>This color.</returns>
    public Color Add(Color color)
    {
        R += color.R;
        G += color.G;
        B += color.B;
        A += color.A;

        return Clamp();
    }

    /// <summary>
    ///     Adds the given color component values to this Color's values.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    /// <returns>This Color.</returns>
    public Color Add(float r, float g, float b, float a)
    {
        R += r;
        G += g;
        B += b;
        A += a;

        return Clamp();
    }

    /// <summary>
    ///     Clamps this Color's components to a valid range [0 - 1].
    /// </summary>
    /// <returns>This color.</returns>
    public Color Clamp()
    {
        if (R < 0)
        {
            R = 0;
        }
        else if (R > 1)
        {
            R = 1;
        }

        if (G < 0)
        {
            G = 0;
        }
        else if (G > 1)
        {
            G = 1;
        }

        if (B < 0)
        {
            B = 0;
        }
        else if (B > 1)
        {
            B = 1;
        }

        if (A < 0)
        {
            A = 0;
        }
        else if (A > 1)
        {
            A = 1;
        }

        return this;
    }

    /// <summary>
    ///     Gets a copy of this color.
    /// </summary>
    /// <returns>A copy of this color.</returns>
    public Color Copy()
    {
        return new Color(this);
    }

    public override bool Equals(object? o)
    {
        if (this == o)
        {
            return true;
        }

        if (o == null || GetType() != o.GetType())
        {
            return false;
        }

        var color = (Color)o;

        return ToIntBits() == color.ToIntBits();
    }

    /// <summary>
    ///     Sets the RGB Color components using the specified Hue-Saturation-Value.
    /// </summary>
    /// <remarks>
    ///     Note that HSV components are voluntary not clamped to preserve high range color and can range beyond typical
    ///     values.
    /// </remarks>
    /// <param name="h">The Hue in degree from 0 to 360.</param>
    /// <param name="s">The Saturation from 0 to 1.</param>
    /// <param name="v">The Value (brightness) from 0 to 1.</param>
    /// <returns>The modified Color for chaining.</returns>
    public Color FromHSV(float h, float s, float v)
    {
        var x = (h / 60f + 6) % 6;
        var i = (int)x;
        var f = x - i;
        var p = v * (1 - s);
        var q = v * (1 - s * f);
        var t = v * (1 - s * (1 - f));

        switch (i)
        {
            case 0:
                R = v;
                G = t;
                B = p;
                break;
            case 1:
                R = q;
                G = v;
                B = p;
                break;
            case 2:
                R = p;
                G = v;
                B = t;
                break;
            case 3:
                R = p;
                G = q;
                B = v;
                break;
            case 4:
                R = t;
                G = p;
                B = v;
                break;
            default:
                R = v;
                G = p;
                B = q;
                break;
        }

        return Clamp();
    }

    /// <summary>
    ///     Sets RGB components using the specified Hue-Saturation-Value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is a convenient method for <see cref="FromHSV(float, float, float)" />.
    ///     </para>
    ///     <para>
    ///         This is the inverse of <see cref="ToHSV(float[])" />.
    ///     </para>
    /// </remarks>
    /// <param name="hsv">The Hue, Saturation and Value components in that order.</param>
    /// <returns>The modified Color for chaining.</returns>
    public Color FromHSV(float[] hsv)
    {
        return FromHSV(hsv[0], hsv[1], hsv[2]);
    }

    public override int GetHashCode()
    {
        var result = R != +0.0f ? NumberUtils.floatToIntBits(R) : 0;

        result = 31 * result + (G != +0.0f ? NumberUtils.floatToIntBits(G) : 0);
        result = 31 * result + (B != +0.0f ? NumberUtils.floatToIntBits(B) : 0);
        result = 31 * result + (A != +0.0f ? NumberUtils.floatToIntBits(A) : 0);

        return result;
    }

    /// <summary>
    ///     Linearly interpolates between this color and the target color by t which is in the range [0,1].
    /// </summary>
    /// <remarks>
    ///     The result is stored in this color.
    /// </remarks>
    /// <param name="target">The target color.</param>
    /// <param name="t">The interpolation coefficient.</param>
    /// <returns>This color.</returns>
    public Color Lerp(Color target, float t)
    {
        R += t * (target.R - R);
        G += t * (target.G - G);
        B += t * (target.B - B);
        A += t * (target.A - A);

        return Clamp();
    }

    /// <summary>
    ///     Linearly interpolates between this color and the target color by t which is in the range [0,1].
    /// </summary>
    /// <remarks>
    ///     The result is stored in this color.
    /// </remarks>
    /// <param name="r">The red component of the target color.</param>
    /// <param name="g">The green component of the target color.</param>
    /// <param name="b">The blue component of the target color.</param>
    /// <param name="a">The alpha component of the target color.</param>
    /// <param name="t">The interpolation coefficient.</param>
    /// <returns>This color.</returns>
    public Color Lerp(float r, float g, float b, float a, float t)
    {
        R += t * (r - R);
        G += t * (g - G);
        B += t * (b - B);
        A += t * (a - A);

        return Clamp();
    }

    /// <summary>
    ///     Multiplies this color and the given color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>This color.</returns>
    public Color Mul(Color color)
    {
        R *= color.R;
        G *= color.G;
        B *= color.B;
        A *= color.A;

        return Clamp();
    }

    /// <summary>
    ///     Multiplies all components of this Color with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>This color.</returns>
    public Color Mul(float value)
    {
        R *= value;
        G *= value;
        B *= value;
        A *= value;

        return Clamp();
    }

    /// <summary>
    ///     Multiplies this Color's color components by the given ones.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    /// <returns>This Color.</returns>
    public Color Mul(float r, float g, float b, float a)
    {
        R *= r;
        G *= g;
        B *= b;
        A *= a;

        return Clamp();
    }

    /// <summary>
    ///     Multiplies the RGB values by the alpha.
    /// </summary>
    /// <returns></returns>
    public Color PreMultiplyAlpha()
    {
        R *= A;
        G *= A;
        B *= A;

        return this;
    }

    /// <summary>
    ///     Sets this color to the given color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>This color.</returns>
    public Color Set(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;

        return this;
    }

    /// <summary>
    ///     Sets this color to the red, green and blue components of the provided Color and a deviating alpha value.
    /// </summary>
    /// <param name="rgb">The desired red, green and blue values (alpha of that Color is ignored).</param>
    /// <param name="alpha">The desired alpha value (will be clamped to the range [0, 1]).</param>
    /// <returns>This color.</returns>
    public Color Set(Color rgb, float alpha)
    {
        R = rgb.R;
        G = rgb.G;
        B = rgb.B;
        A = MathUtils.Clamp(alpha, 0f, 1f);

        return this;
    }

    /// <summary>
    ///     Sets this Color's component values.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    /// <returns>This Color.</returns>
    public Color Set(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;

        return Clamp();
    }

    /// <summary>
    ///     Sets this color's component values through an integer representation.
    /// </summary>
    /// <remarks>
    ///     See <see cref="RGBA8888ToColor(Color, int)" />
    /// </remarks>
    /// <param name="rgba"></param>
    /// <returns>This color.</returns>
    public Color Set(int rgba)
    {
        RGBA8888ToColor(this, rgba);

        return this;
    }

    /// <summary>
    ///     Subtracts the given color from this color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>This color</returns>
    public Color Sub(Color color)
    {
        R -= color.R;
        G -= color.G;
        B -= color.B;
        A -= color.A;

        return Clamp();
    }

    /// <summary>
    ///     Subtracts the given values from this Color's component values.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    /// <returns>This Color.</returns>
    public Color Sub(float r, float g, float b, float a)
    {
        R -= r;
        G -= g;
        B -= b;
        A -= a;

        return Clamp();
    }

    /// <summary>
    ///     Packs the color components into a 32-bit integer with the format ABGR and then converts it to a float.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Alpha is compressed from 0-255 to use only even numbers between 0-254 to avoid using float bits in the
    ///         <see langword="NaN" /> range (see <see cref="NumberUtils.intToFloatColor(int)" />).
    ///     </para>
    ///     <para>
    ///         Converting a color to a float and back can be lossy for alpha.
    ///     </para>
    /// </remarks>
    /// <returns>The packed color as a 32-bit float.</returns>
    public float ToFloatBits()
    {
        var color = ((int)(255 * A) << 24) | ((int)(255 * B) << 16) | ((int)(255 * G) << 8) | (int)(255 * R);

        return NumberUtils.intToFloatColor(color);
    }

    /// <summary>
    ///     Extract Hue-Saturation-Value.
    /// </summary>
    /// <remarks>
    ///     This is the inverse of <see cref="FromHSV(float[])" />.
    /// </remarks>
    /// <param name="hsv">The HSV array to be modified.</param>
    /// <returns>HSV components for chaining.</returns>
    public float[] ToHSV(float[] hsv)
    {
        var max = Math.Max(Math.Max(R, G), B);
        var min = Math.Min(Math.Min(R, G), B);
        var range = max - min;

        if (range == 0)
        {
            hsv[0] = 0;
        }
        else if (max == R)
        {
            hsv[0] = (60 * (G - B) / range + 360) % 360;
        }
        else if (max == G)
        {
            hsv[0] = 60 * (B - R) / range + 120;
        }
        else
        {
            hsv[0] = 60 * (R - G) / range + 240;
        }

        if (max > 0)
        {
            hsv[1] = 1 - min / max;
        }
        else
        {
            hsv[1] = 0;
        }

        hsv[2] = max;

        return hsv;
    }

    /// <summary>
    ///     Packs the color components into a 32-bit integer with the format ABGR.
    /// </summary>
    /// <returns>The packed color as a 32-bit int.</returns>
    public int ToIntBits()
    {
        return ((int)(255 * A) << 24) | ((int)(255 * B) << 16) | ((int)(255 * G) << 8) | (int)(255 * R);
    }

    /// <summary>
    ///     Returns the color encoded as hex string with the format RRGGBBAA.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var value =
            (((int)(255 * R) << 24) | ((int)(255 * G) << 16) | ((int)(255 * B) << 8) | (int)(255 * A)).ToString("x8");

        while (value.Length < 8)
        {
            value = "0" + value;
        }

        return value;
    }
}