using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    internal sealed class WinNTFileSystem : FileSystem {

    private static readonly String LONG_PATH_PREFIX = "\\\\?\\";

    private readonly char slash;
    private readonly char altSlash;
    private readonly char semicolon;
    private readonly String userDir;

    // Whether to enable alternative data streams (ADS) by suppressing
    // checking the path for invalid characters, in particular ":".
    // By default, ADS support is enabled and will be disabled if and
    // only if the property is set, ignoring case, to the string "false".
    private static readonly bool ENABLE_ADS;
    static WinNTFileSystem() {
        // TODO: ?? String enableADS = GetPropertyAction.privilegedGetProperty("jdk.io.File.enableADS");
        //if (enableADS != null) {
        //    // TODO: Should this be invariant? -RP
        //    ENABLE_ADS = !enableADS.Equals(false.ToString(), StringComparison.InvariantCultureIgnoreCase);
        //} else {
            ENABLE_ADS = true;
        //}
    }

    // Strip a long path or UNC prefix and return the result.
    // If there is no such prefix, return the parameter passed in.
    private static String stripLongOrUNCPrefix(String path) {
        // if a prefix is present, remove it
        if (path.StartsWith(LONG_PATH_PREFIX)) {
            if (path.Substring(4).StartsWith("UNC\\")) {
                path = "\\\\" + path.Substring(8);
            } else {
                path = path.Substring(4);
                // if only "UNC" remains, a trailing "\\" was likely removed
                if (path.Equals("UNC")) {
                    path = "\\\\";
                }
            }
        }

        return path;
    }

    internal WinNTFileSystem() {
        // TODO: Properties props = GetPropertyAction.privilegedGetProperties();
        slash = '/';// props.getProperty("file.separator").charAt(0);
        semicolon = ';';//props.getProperty("path.separator").charAt(0);
        altSlash = '\\';//(this.slash == '\\') ? '/' : '\\';
        // TODO: Verify that this is the correct directory.
        userDir = normalize(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));//normalize(props.getProperty("user.dir"));

      //  initIDs();
        }

    private bool isSlash(char c) {
        return (c == '\\') || (c == '/');
    }

    private bool isLetter(char c) {
        return ((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z'));
    }

    private String slashify(String p) {
        if (p.Length != 0 && p[0] != slash) return slash + p;
        else return p;
    }

    /* -- Normalization and construction -- */

    public override char getSeparator() {
        return slash;
    }

    override public char getPathSeparator() {
        return semicolon;
    }

    /* Check that the given pathname is normal.  If not, invoke the real
       normalizer on the part of the pathname that requires normalization.
       This way we iterate through the whole pathname string only once. */
    override public String normalize(String path) {
        path = stripLongOrUNCPrefix(path);
        int n = path.Length;
        char slash = this.slash;
        char altSlash = this.altSlash;
        char prev = (char)0;
        for (int i = 0; i < n; i++) {
            char c = path[i];
            if (c == altSlash)
                return normalize(path, n, (prev == slash) ? i - 1 : i);
            if ((c == slash) && (prev == slash) && (i > 1))
                return normalize(path, n, i - 1);
            if ((c == ':') && (i > 1))
                return normalize(path, n, 0);
            prev = c;
        }
        if (prev == slash) return normalize(path, n, n - 1);
        return path;
    }

    /* Normalize the given pathname, whose length is len, starting at the given
       offset; everything before this offset is already normal. */
    private String normalize(String path, int len, int off) {
        if (len == 0) return path;
        if (off < 3) off = 0;   /* Avoid fencepost cases with UNC pathnames */
        int src;
        char slash = this.slash;
        StringBuilder sb = new StringBuilder(len);

        if (off == 0) {
            /* Complete normalization, including prefix */
            src = normalizePrefix(path, len, sb);
        } else {
            /* Partial normalization */
            src = off;
            sb.Append(path, 0, off);
        }

        /* Remove redundant slashes from the remainder of the path, forcing all
           slashes into the preferred slash */
        while (src < len) {
            char c = path[src++];
            if (isSlash(c)) {
                while ((src < len) && isSlash(path[src])) src++;
                if (src == len) {
                    /* Check for trailing separator */
                    int sn = sb.Length;
                    if ((sn == 2) && (sb[1] == ':')) {
                        /* "z:\\" */
                        sb.Append(slash);
                        break;
                    }
                    if (sn == 0) {
                        /* "\\" */
                        sb.Append(slash);
                        break;
                    }
                    if ((sn == 1) && (isSlash(sb[0]))) {
                        /* "\\\\" is not collapsed to "\\" because "\\\\" marks
                           the beginning of a UNC pathname.  Even though it is
                           not, by itself, a valid UNC pathname, we leave it as
                           is in order to be consistent with the win32 APIs,
                           which treat this case as an invalid UNC pathname
                           rather than as an alias for the root directory of
                           the current drive. */
                        sb.Append(slash);
                        break;
                    }
                    /* Path does not denote a root directory, so do not append
                       trailing slash */
                    break;
                } else {
                    sb.Append(slash);
                }
            } else {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /* A normal Win32 pathname contains no duplicate slashes, except possibly
       for a UNC prefix, and does not end with a slash.  It may be the empty
       string.  Normalized Win32 pathnames have the convenient property that
       the length of the prefix almost uniquely identifies the type of the path
       and whether it is absolute or relative:

           0  relative to both drive and directory
           1  drive-relative (begins with '\\')
           2  absolute UNC (if first char is '\\'),
                else directory-relative (has form "z:foo")
           3  absolute local pathname (begins with "z:\\")
     */
    private int normalizePrefix(String path, int len, StringBuilder sb) {
        int src = 0;
        while ((src < len) && isSlash(path[src])) src++;
        char c;
        if ((len - src >= 2)
            && isLetter(c = path[src])
            && path[src + 1] == ':') {
            /* Remove leading slashes if followed by drive specifier.
               This hack is necessary to support file URLs containing drive
               specifiers (e.g., "file://c:/path").  As a side effect,
               "/c:/path" can be used as an alternative to "c:/path". */
            sb.Append(c);
            sb.Append(':');
            src += 2;
        } else {
            src = 0;
            if ((len >= 2)
                && isSlash(path[0])
                && isSlash(path[1])) {
                /* UNC pathname: Retain first slash; leave src pointed at
                   second slash so that further slashes will be collapsed
                   into the second slash.  The result will be a pathname
                   beginning with "\\\\" followed (most likely) by a host
                   name. */
                src = 1;
                sb.Append(slash);
            }
        }
        return src;
    }

    override public int prefixLength(String path) {
        System.Diagnostics.Debug.Assert(!path.StartsWith(LONG_PATH_PREFIX));

        char slash = this.slash;
        int n = path.Length;
        if (n == 0) return 0;
        char c0 = path[0];
        char c1 = (n > 1) ? path[1] : (char)0;
        if (c0 == slash) {
            if (c1 == slash) return 2;  /* Absolute UNC pathname "\\\\foo" */
            return 1;                   /* Drive-relative "\\foo" */
        }
        if (isLetter(c0) && (c1 == ':')) {
            if ((n > 2) && (path[2] == slash))
                return 3;               /* Absolute local pathname "z:\\foo" */
            return 2;                   /* Directory-relative "z:foo" */
        }
        return 0;                       /* Completely relative */
    }

    public override Path getPath(string first, string[] more)
    {
        String path;
        if (more.Length == 0)
        {
            path = first;
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(first);
            foreach (String segment in more)
            {
                if (segment.Length > 0)
                {
                    if (sb.Length > 0)
                        sb.Append('\\');
                    sb.Append(segment);
                }
            }
            path = sb.ToString();
        }
        return WindowsPath.parse(this, path);
        }

    override public String resolve(String parent, String child) {
        System.Diagnostics.Debug.Assert(!child.StartsWith(LONG_PATH_PREFIX));

        int pn = parent.Length;
        if (pn == 0) return child;
        int cn = child.Length;
        if (cn == 0) return parent;

        String c = child;
        int childStart = 0;
        int parentEnd = pn;

        bool isDirectoryRelative =
            pn == 2 && isLetter(parent[0]) && parent[1] == ':';

        if ((cn > 1) && (c[0] == slash)) {
            if (c[1] == slash) {
                /* Drop prefix when child is a UNC pathname */
                childStart = 2;
            } else if (!isDirectoryRelative) {
                /* Drop prefix when child is drive-relative */
                childStart = 1;

            }
            if (cn == childStart) { // Child is double slash
                if (parent[pn - 1] == slash)
                    return parent.Substring(0, pn - 1);
                return parent;
            }
        }

        if (parent[pn - 1] == slash)
            parentEnd--;

        int strlen = parentEnd + cn - childStart;
        char[] theChars = null;
        if (child[childStart] == slash || isDirectoryRelative) {
            theChars = new char[strlen];

            parent.CopyTo(0, theChars, 0, parentEnd);
            child.CopyTo(childStart, theChars, parentEnd, cn);
        } else {
            theChars = new char[strlen + 1];
            parent.CopyTo(0, theChars, 0, parentEnd);
            theChars[parentEnd] = slash;
            child.CopyTo(childStart, theChars, parentEnd + 1, cn);
        }

        // if present, strip trailing name separator unless after a ':'
        if (theChars.Length > 1
            && theChars[theChars.Length - 1] == slash
            && theChars[theChars.Length - 2] != ':')
            return new String(theChars, 0, theChars.Length - 1);

        return new String(theChars);
    }

    override public String getDefaultParent() {
        return ("" + slash);
    }

    override public String fromURIPath(String path) {
        String p = path;
        if ((p.Length > 2) && (p[2] == ':')) {
            // "/c:/foo" --> "c:/foo"
            p = p.Substring(1);
            // "c:/foo/" --> "c:/foo", but "c:/" --> "c:/"
            if ((p.Length > 3) && p.EndsWith("/"))
                p = p.Substring(0, p.Length - 1);
        } else if ((p.Length > 1) && p.EndsWith("/")) {
            // "/foo/" --> "/foo"
            p = p.Substring(0, p.Length - 1);
        }
        return p;
    }

    /* -- Path operations -- */

    override public bool isAbsolute(File f) {
        String path = f.getPath();
        System.Diagnostics.Debug.Assert(!path.StartsWith(LONG_PATH_PREFIX));

        int pl = f.getPrefixLength();
        return (((pl == 2) && (f.getPath()[0] == slash))
                || (pl == 3));
    }

    override public bool isInvalid(File f) {
        if (f.getPath().IndexOf('\u0000') >= 0)
            return true;

        if (ENABLE_ADS)
            return false;

        // Invalid if there is a ":" at a position greater than 1, or if there
        // is a ":" at position 1 and the first character is not a letter
        String pathname = f.getPath();
        int lastColon = pathname.LastIndexOf(":");

        // Valid if there is no ":" present or if the last ":" present is
        // at index 1 and the first character is a latter
        if (lastColon < 0 ||
            (lastColon == 1 && isLetter(pathname[0])))
            return false;

        // Invalid if path creation fails
        // TODO: This probably doesn't work as well as Java.
        try {
            _ = new System.IO.FileInfo(pathname);
            return false;
        }
        catch (ArgumentException) { }
        catch (System.IO.PathTooLongException) { }
        catch (NotSupportedException) { }

            return true;
    }

    override public String resolve(File f) {
        String path = f.getPath();
        System.Diagnostics.Debug.Assert(!path.StartsWith(LONG_PATH_PREFIX));

        int pl = f.getPrefixLength();
        if ((pl == 2) && (path[0] == slash))
            return path;                        /* UNC */
        if (pl == 3)
            return path;                        /* Absolute local */
        if (pl == 0)
        {
            // TODO: This absolutely does not work -RP
            // return getUserPath() + slashify(path); /* Completely relative */
            return Environment.CurrentDirectory + slashify(path);
        }
        if (pl == 1) {                          /* Drive-relative */
            String up = getUserPath();
            String ud = getDrive(up);
            if (ud != null) return ud + path;
            return up + path;                   /* User dir is a UNC path */
        }
        if (pl == 2) {                          /* Directory-relative */
            String up = getUserPath();
            String ud = getDrive(up);
            if ((ud != null) && path.StartsWith(ud))
                return up + slashify(path.Substring(2));
            char drive = path[0];
            String dir = getDriveDirectory(drive);
            if (dir != null) {
                /* When resolving a directory-relative path that refers to a
                   drive other than the current drive, insist that the caller
                   have read permission on the result */
                String p = drive + (':' + dir + slashify(path.Substring(2)));
                    // TODO: @SuppressWarnings("removal")
                    // TODO: SecurityManager security = System.getSecurityManager();
                //    try
                //    {
                //    if (security != null) security.checkRead(p);
                //} catch (SecurityException x) {
                //    /* Don't disclose the drive's directory in the exception */
                //    throw new SecurityException("Cannot resolve path " + path);
                //}
                return p;
            }
            return drive + ":" + slashify(path.Substring(2)); /* fake it */
        }
            // TODO: throw new InternalError("Unresolvable path: " + path);
            throw new Exception("Unresolvable path: " + path);
        }

    private String getUserPath() {
            /* For both compatibility and security,
               we must look this up every time */
            // TODO: @SuppressWarnings("removal")
            // TODO: SecurityManager sm = System.getSecurityManager();
        //    if (sm != null) {
        //    sm.checkPropertyAccess("user.dir");
        //}
        return userDir;
    }

    private String getDrive(String path) {
        int pl = prefixLength(path);
        return (pl == 3) ? path.Substring(0, 2) : null;
    }

    private static readonly String[] DRIVE_DIR_CACHE = new String[26];

    private static int driveIndex(char d) {
        if ((d >= 'a') && (d <= 'z')) return d - 'a';
        if ((d >= 'A') && (d <= 'Z')) return d - 'A';
        return -1;
    }

    private extern String getDriveDirectory(int drive);

    private String getDriveDirectory(char drive) {
        int i = driveIndex(drive);
        if (i < 0) return null;
        // Updates might not be visible to other threads so there
        // is no guarantee getDriveDirectory(i+1) is called just once
        // for any given value of i.
        String s = DRIVE_DIR_CACHE[i];
        if (s != null) return s;
        s = getDriveDirectory(i + 1);
        DRIVE_DIR_CACHE[i] = s;
        return s;

    }

    override public String canonicalize(String path) //throws IOException 
    {
        System.Diagnostics.Debug.Assert(!path.StartsWith(LONG_PATH_PREFIX));

        // If path is a drive letter only then skip canonicalization
        int len = path.Length;
        if ((len == 2) &&
            (isLetter(path[0])) &&
            (path[1] == ':')) {
            char c = path[0];
            if ((c >= 'A') && (c <= 'Z'))
                return path;
            return "" + ((char) (c-32)) + ':';
        } else if ((len == 3) &&
                   (isLetter(path[0])) &&
                   (path[1] == ':') &&
                   (path[2] == '\\')) {
            char c = path[0];
            if ((c >= 'A') && (c <= 'Z'))
                return path;
            return "" + ((char) (c-32)) + ':' + '\\';
        }
        return System.IO.Path.GetFullPath(path);
    }
        
    /* -- Attribute accessors -- */

    override public int getBooleanAttributes(File f) {
        return getBooleanAttributes0(f);
    }
    private extern int getBooleanAttributes0(File f);

    override public bool checkAccess(File f, int access) {
        return checkAccess0(f, access);
    }
    private extern bool checkAccess0(File f, int access);

    override public long getLastModifiedTime(File f) {
        return getLastModifiedTime0(f);
    }
    private extern long getLastModifiedTime0(File f);

    override public long getLength(File f) {
        // TODO: Do I need absolute path here? -RP
        return new FileInfo(f.getAbsolutePath()).Length;
    }
    private extern long getLength0(File f);

    override public bool setPermission(File f, int access, bool enable, bool owneronly) {
        return setPermission0(f, access, enable, owneronly);
    }
    private extern bool setPermission0(File f, int access, bool enable, bool owneronly);

    /* -- File operations -- */

    override public bool createFileExclusively(String path)// throws IOException 
    {
        return createFileExclusively0(path);
    }
    private extern bool createFileExclusively0(String path);// throws IOException;

    override public String[] list(File f) {
        return list0(f);
    }
    private extern String[] list0(File f);

    override public bool createDirectory(File f) {
        return createDirectory0(f);
    }
    private extern bool createDirectory0(File f);

    override public bool setLastModifiedTime(File f, long time) {
        return setLastModifiedTime0(f, time);
    }
    private extern bool setLastModifiedTime0(File f, long time);

    override public bool setReadOnly(File f) {
        return setReadOnly0(f);
    }
    private extern bool setReadOnly0(File f);

    override public bool delete(File f) {
        return delete0(f);
    }
    private extern bool delete0(File f);

    override public bool rename(File f1, File f2) {
        return rename0(f1, f2);
    }
    private extern bool rename0(File f1, File f2);

    /* -- Filesystem interface -- */

    override public File[] listRoots() {
        // TODO: return BitSet
        //    .valueOf(new long[] {listRoots0()})
        //    .stream()
        //    .mapToObj(i -> new File((char)('A' + i) + ":" + slash))
        //    .filter(f -> access(f.getPath()))
        //    .toArray(File[]::new);
        throw new NotImplementedException();
    }
    private static extern int listRoots0();

    private bool access(String path) {
        //try {
                // TODO: @SuppressWarnings("removal")
                // TODO: SecurityManager security = System.getSecurityManager();
                //if (security != null) security.checkRead(path);
            return true;
        //} catch (SecurityException x) {
        //    return false;
        //}
    }

    /* -- Disk usage -- */

    override public long getSpace(File f, int t) {
        if (f.exists()) {
            return getSpace0(f, t);
        }
        return 0;
    }
    private extern long getSpace0(File f, int t);

    /* -- Basic infrastructure -- */

    // Obtain maximum file component length from GetVolumeInformation which
    // expects the path to be null or a root component ending in a backslash
    private extern int getNameMax0(String path);

    override public int getNameMax(String path) {
        String s = null;
        if (path != null) {
            File f = new File(path);
            if (f.isAbsolute()) {
                Path root = f.toPath().getRoot();
                if (root != null) {
                    s = root.ToString();
                    if (!s.EndsWith("\\")) {
                        s = s + "\\";
                    }
                }
            }
        }
        return getNameMax0(s);
    }

    override public int compare(File f1, File f2) {
        // TODO: Should this be invariant? -RP
        return string.Compare(f1.getPath(),f2.getPath(), StringComparison.InvariantCultureIgnoreCase);
    }

    override public int hashCode(File f) {
        /* Could make this more efficient: String.hashCodeIgnoreCase */
        return f.getPath().ToLower().GetHashCode() ^ 1234321;
    }

    private static extern void initIDs();
}
}
