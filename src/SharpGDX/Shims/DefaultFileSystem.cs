using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    internal sealed class DefaultFileSystem
    {

        private DefaultFileSystem() { }

        /**
         * Return the FileSystem object for Windows platform.
         */
        public static FileSystem getFileSystem()
        {
            return new WinNTFileSystem();
        }
    }
}
