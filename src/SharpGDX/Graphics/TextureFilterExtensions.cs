using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharpGDX.Graphics.Texture;

namespace SharpGDX.Graphics
{
    public static class TextureFilterExtensions
    {
        public static int getGLEnum(this TextureFilter filter)
        {
            return (int)filter;
        }
    }
}
