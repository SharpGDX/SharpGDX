using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    /**
 * A parser of Windows path strings
 */

class WindowsPathParser {

    

        private WindowsPathParser() { }

    /**
     * The result of a parse operation
     */
    internal class Result {
        private readonly WindowsPathType _type;
        private readonly String _root;
        private readonly String _path;

        internal Result(WindowsPathType type, String root, String path) {
            this._type = type;
            this._root = root;
            this._path = path;
        }

        /**
         * The path type
         */
        internal WindowsPathType type() {
            return _type;
        }

        /**
         * The root component
         */
        internal String root() {
            return _root;
        }

        /**
         * The normalized path (includes root)
         */
        internal String path() {
            return _path;
        }
    }

    /**
     * Parses the given input as a Windows path
     */
    internal static Result parse(String input) {
        return parse(input, true);
    }

    /**
     * Parses the given input as a Windows path where it is known that the
     * path is already normalized.
     */
    static Result parseNormalizedPath(String input) {
        return parse(input, false);
    }

    /**
     * Parses the given input as a Windows path.
     *
     * @param   requireToNormalize
     *          Indicates if the path requires to be normalized
     */
    private static Result parse(String input, bool requireToNormalize) {
        String root = "";
        WindowsPathType? type = null;

        int len = input.Length;
        int off = 0;
        if (len > 1) {
            char c0 = input[0];
            char c1 = input[1];
            char c = (char)0;
            int next = 2;
            if (isSlash(c0) && isSlash(c1)) {
                // UNC: We keep the first two slash, collapse all the
                // following, then take the hostname and share name out,
                // meanwhile collapsing all the redundant slashes.
                type = WindowsPathType.UNC;
                off = nextNonSlash(input, next, len);
                next = nextSlash(input, off, len);
                if (off == next)
                {
                        // TODO: throw new InvalidPathException(input, "UNC path is missing hostname");
                        throw new Exception("UNC path is missing hostname");
                    }
                String host = input.Substring(off, next);  //host
                off = nextNonSlash(input, next, len);
                next = nextSlash(input, off, len);
                if (off == next)
                {
                        // TODO: throw new InvalidPathException(input, "UNC path is missing sharename");
                        throw new Exception( "UNC path is missing sharename");
                    }
                root = "\\\\" + host + "\\" + input.Substring(off, next) + "\\";
                off = next;
            } else {
                if (isLetter(c0) && c1 == ':') {
                    char c2;
                    if (len > 2 && isSlash(c2 = input[2])) {
                        // avoid concatenation when root is "D:\"
                        if (c2 == '\\') {
                            root = input.Substring(0, 3);
                        } else {
                            root = input.Substring(0, 2) + '\\';
                        }
                        off = 3;
                        type = WindowsPathType.ABSOLUTE;
                    } else {
                        root = input.Substring(0, 2);
                        off = 2;
                        type = WindowsPathType.DRIVE_RELATIVE;
                    }
                }
            }
        }
        if (off == 0) {
            if (len > 0 && isSlash(input[0])) {
                type = WindowsPathType.DIRECTORY_RELATIVE;
                root = "\\";
            } else {
                type = WindowsPathType.RELATIVE;
            }
        }

        if (requireToNormalize) {
            StringBuilder sb = new StringBuilder(input.Length);
            sb.Append(root);
            return new Result(type.Value, root, normalize(sb, input, off));
        } else {
            return new Result(type.Value, root, input);
        }
    }

    /**
     * Remove redundant slashes from the rest of the path, forcing all slashes
     * into the preferred slash.
    */
    private static String normalize(StringBuilder sb, String path, int off) {
        int len = path.Length;
        off = nextNonSlash(path, off, len);
        int start = off;
        char lastC = (char)0;
        while (off < len) {
            char c = path[off];
            if (isSlash(c)) {
                if (lastC == ' ')
                {
                        // TODO: throw new InvalidPathException(path, "Trailing char <" + lastC + ">", off - 1);
                        throw new Exception( "Trailing char <" + lastC + ">");

                    }
                sb.Append(path, start, off);
                off = nextNonSlash(path, off, len);
                if (off != len)   //no slash at the end of normalized path
                    sb.Append('\\');
                start = off;
            } else {
                if (isInvalidPathChar(c))
                {
                        // TODO: throw new InvalidPathException(path, "Illegal char <" + c + ">", off);
                        throw new Exception("Illegal char <" + c + ">");
                    }
                lastC = c;
                off++;
            }
        }
        if (start != off) {
            if (lastC == ' ')
            {
                    // TODO: throw new InvalidPathException(path, "Trailing char <" + lastC + ">", off - 1);
                    throw new Exception( "Trailing char <" + lastC + ">");
                }
            sb.Append(path, start, off);
        }
        return sb.ToString();
    }

    private static bool isSlash(char c) {
        return (c == '\\') || (c == '/');
    }

    private static int nextNonSlash(String path, int off, int end) {
        while (off < end && isSlash(path[off])) { off++; }
        return off;
    }

    private static int nextSlash(String path, int off, int end) {
        char c;
        while (off < end && !isSlash(c=path[off])) {
            if (isInvalidPathChar(c))
            {
                    // TODO: throw new InvalidPathException(path, "Illegal character [" + c + "] in path", off);
                    throw new Exception( "Illegal character [" + c + "] in path");
                }
            off++;
        }
        return off;
    }

    private static bool isLetter(char c) {
        return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'));
    }

    // Reserved characters for window path name
    private static readonly String reservedChars = "<>:\"|?*";
    private static bool isInvalidPathChar(char ch) {
        return ch < '\u0020' || reservedChars.IndexOf(ch) != -1;
    }
}
}
