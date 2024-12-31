using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    internal class WindowsPath : Path
    {
        public int CompareTo(Path? other)
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public Path Current { get; }

        object? IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public FileSystem getFileSystem()
        {
            throw new NotImplementedException();
        }

        public bool isAbsolute()
        {
            throw new NotImplementedException();
        }

        public Path getRoot()
        {
            throw new NotImplementedException();
        }

        public Path getFileName()
        {
            throw new NotImplementedException();
        }

        private WinNTFileSystem fs;
        private WindowsPathType type;
        private string root;
        private string path;

        /**
     * Initializes a new instance of this class.
     */
        private WindowsPath(WinNTFileSystem fs,
            WindowsPathType type,
            String root,
            String path)
        {
            this.fs = fs;
            this.type = type;
            this.root = root;
            this.path = path;
        }

        internal static WindowsPath parse(WinNTFileSystem fs, String path)
        {
            WindowsPathParser.Result result = WindowsPathParser.parse(path);
            return new WindowsPath(fs, result.type(), result.root(), result.path());
        }


        public Path getParent()
        {
            throw new NotImplementedException();
        }

        public int getNameCount()
        {
            throw new NotImplementedException();
        }

        public Path getName(int index)
        {
            throw new NotImplementedException();
        }

        public Path subpath(int beginIndex, int endIndex)
        {
            throw new NotImplementedException();
        }

        public bool startsWith(Path other)
        {
            throw new NotImplementedException();
        }

        public bool endsWith(Path other)
        {
            throw new NotImplementedException();
        }

        public Path normalize()
        {
            throw new NotImplementedException();
        }

        public Path resolve(Path other)
        {
            throw new NotImplementedException();
        }

        public Path relativize(Path other)
        {
            throw new NotImplementedException();
        }

        public Path toAbsolutePath()
        {
            throw new NotImplementedException();
        }
    }
}
