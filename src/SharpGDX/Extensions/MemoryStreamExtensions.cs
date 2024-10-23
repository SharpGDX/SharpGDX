using System.Runtime.InteropServices;

namespace SharpGDX.Extensions;

internal static class MemoryStreamExtensions
{
    public static void Write(this MemoryStream stream, float[] values)
    {
        stream.Write(MemoryMarshal.Cast<float, byte>(values));
    }
}