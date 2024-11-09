using SharpGDX.Files;
using SharpGDX.Shims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
    public interface BaseJsonReader
    {
        JsonValue parse(InputStream input);

        JsonValue parse(FileHandle file);
    }
}
