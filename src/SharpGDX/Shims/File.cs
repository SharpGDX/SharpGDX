﻿using System.Text;

namespace SharpGDX.Shims;

[Serializable]
public class File : IComparable<File>
{

    /**
     * The FileSystem object representing the platform's local file system.
     */
    private static readonly FileSystem FS = DefaultFileSystem.getFileSystem();

    /**
     * This abstract pathname's normalized pathname string. A normalized
     * pathname string uses the default name-separator character and does not
     * contain any duplicate or redundant separators.
     *
     * @serial
     */
    private readonly String path;

    /**
     * Enum type that indicates the status of a file path.
     */
    private enum PathStatus { INVALID, CHECKED };

    /**
     * The flag indicating whether the file path is invalid.
     */
    [NonSerialized]
    private PathStatus? status = null;

    /**
     * Check if the file has an invalid path. Currently, the inspection of
     * a file path is very limited, and it only covers Nul character check
     * unless further checking is explicitly enabled by a system property.
     * Returning true means the path is definitely invalid/garbage, but
     * returning false does not guarantee that the path is valid.
     *
     * @return true if the file path is invalid.
     */
    bool isInvalid() {
        PathStatus? s = status;
        if (s == null) {
            s = FS.isInvalid(this) ? PathStatus.INVALID : PathStatus.CHECKED;
            status = s;
        }
        return s == PathStatus.INVALID;
    }

    /**
     * The length of this abstract pathname's prefix, or zero if it has no
     * prefix.
     */
    [NonSerialized]
    private readonly  int prefixLength;

    /**
     * Returns the length of this abstract pathname's prefix.
     * For use by FileSystem classes.
     */
    internal int getPrefixLength() {
        return prefixLength;
    }

    /**
     * The system-dependent default name-separator character.  This field is
     * initialized to contain the first character of the value of the system
     * property {@code file.separator}.  On UNIX systems the value of this
     * field is {@code '/'}; on Microsoft Windows systems it is {@code '\\'}.
     *
     * @see     java.lang.System#getProperty(java.lang.String)
     */
    public static readonly char separatorChar = FS.getSeparator();

    /**
     * The system-dependent default name-separator character, represented as a
     * string for convenience.  This string contains a single character, namely
     * {@link #separatorChar}.
     */
    public static readonly String separator = (separatorChar).ToString();

    /**
     * The system-dependent path-separator character.  This field is
     * initialized to contain the first character of the value of the system
     * property {@code path.separator}.  This character is used to
     * separate filenames in a sequence of files given as a <em>path list</em>.
     * On UNIX systems, this character is {@code ':'}; on Microsoft Windows systems it
     * is {@code ';'}.
     *
     * @see     java.lang.System#getProperty(java.lang.String)
     */
    public static readonly char pathSeparatorChar = FS.getPathSeparator();

    /**
     * The system-dependent path-separator character, represented as a string
     * for convenience.  This string contains a single character, namely
     * {@link #pathSeparatorChar}.
     */
    public static readonly String pathSeparator = (pathSeparatorChar).ToString();


    /* -- Constructors -- */

    /**
     * Internal constructor for already-normalized pathname strings.
     */
    private File(String pathname, int prefixLength) {
        this.path = pathname;
        this.prefixLength = prefixLength;
    }

    /**
     * Internal constructor for already-normalized pathname strings.
     * The parameter order is used to disambiguate this method from the
     * public(File, String) constructor.
     */
    private File(String child, File parent) {
        System.Diagnostics.Debug.Assert(parent.path != null);
        System.Diagnostics.Debug.Assert(parent.path.Length != 0);
        this.path = FS.resolve(parent.path, child);
        this.prefixLength = parent.prefixLength;
    }

    /**
     * Creates a new {@code File} instance by converting the given
     * pathname string into an abstract pathname.  If the given string is
     * the empty string, then the result is the empty abstract pathname.
     *
     * @param   pathname  A pathname string
     * @throws  NullPointerException
     *          If the {@code pathname} argument is {@code null}
     */
    public File(String pathname) {
        if (pathname == null) {
            throw new NullPointerException();
        }
        this.path = FS.normalize(pathname);
        this.prefixLength = FS.prefixLength(this.path);
    }

    /* Note: The two-argument File constructors do not interpret an empty
       parent abstract pathname as the current user directory.  An empty parent
       instead causes the child to be resolved against the system-dependent
       directory defined by the FileSystem.getDefaultParent method.  On Unix
       this default is "/", while on Microsoft Windows it is "\\".  This is required for
       compatibility with the original behavior of this class. */

    /**
     * Creates a new {@code File} instance from a parent pathname string
     * and a child pathname string.
     *
     * <p> If {@code parent} is {@code null} then the new
     * {@code File} instance is created as if by invoking the
     * single-argument {@code File} constructor on the given
     * {@code child} pathname string.
     *
     * <p> Otherwise the {@code parent} pathname string is taken to denote
     * a directory, and the {@code child} pathname string is taken to
     * denote either a directory or a file.  If the {@code child} pathname
     * string is absolute then it is converted into a relative pathname in a
     * system-dependent way.  If {@code parent} is the empty string then
     * the new {@code File} instance is created by converting
     * {@code child} into an abstract pathname and resolving the result
     * against a system-dependent default directory.  Otherwise each pathname
     * string is converted into an abstract pathname and the child abstract
     * pathname is resolved against the parent.
     *
     * @param   parent  The parent pathname string
     * @param   child   The child pathname string
     * @throws  NullPointerException
     *          If {@code child} is {@code null}
     */
    public File(String? parent, String child) {
        if (child == null) {
            throw new NullPointerException();
        }
        if (parent != null) {
            if (parent.Length == 0) {
                this.path = FS.resolve(FS.getDefaultParent(),
                                       FS.normalize(child));
            } else {
                this.path = FS.resolve(FS.normalize(parent),
                                       FS.normalize(child));
            }
        } else {
            this.path = FS.normalize(child);
        }
        this.prefixLength = FS.prefixLength(this.path);
    }

    /**
     * Creates a new {@code File} instance from a parent abstract
     * pathname and a child pathname string.
     *
     * <p> If {@code parent} is {@code null} then the new
     * {@code File} instance is created as if by invoking the
     * single-argument {@code File} constructor on the given
     * {@code child} pathname string.
     *
     * <p> Otherwise the {@code parent} abstract pathname is taken to
     * denote a directory, and the {@code child} pathname string is taken
     * to denote either a directory or a file.  If the {@code child}
     * pathname string is absolute then it is converted into a relative
     * pathname in a system-dependent way.  If {@code parent} is the empty
     * abstract pathname then the new {@code File} instance is created by
     * converting {@code child} into an abstract pathname and resolving
     * the result against a system-dependent default directory.  Otherwise each
     * pathname string is converted into an abstract pathname and the child
     * abstract pathname is resolved against the parent.
     *
     * @param   parent  The parent abstract pathname
     * @param   child   The child pathname string
     * @throws  NullPointerException
     *          If {@code child} is {@code null}
     */
    public File(File parent, String child) {
        if (child == null) {
            throw new NullPointerException();
        }
        if (parent != null) {
            if (parent.path.Length == 0) {
                this.path = FS.resolve(FS.getDefaultParent(),
                                       FS.normalize(child));
            } else {
                this.path = FS.resolve(parent.path,
                                       FS.normalize(child));
            }
        } else {
            this.path = FS.normalize(child);
        }
        this.prefixLength = FS.prefixLength(this.path);
    }

    /**
     * Creates a new {@code File} instance by converting the given
     * {@code file:} URI into an abstract pathname.
     *
     * <p> The exact form of a {@code file:} URI is system-dependent, hence
     * the transformation performed by this constructor is also
     * system-dependent.
     *
     * <p> For a given abstract pathname <i>f</i> it is guaranteed that
     *
     * <blockquote><code>
     * new File(</code><i>&nbsp;f</i><code>.{@link #toURI()
     * toURI}()).equals(</code><i>&nbsp;f</i><code>.{@link #getAbsoluteFile() getAbsoluteFile}())
     * </code></blockquote>
     *
     * so long as the original abstract pathname, the URI, and the new abstract
     * pathname are all created in (possibly different invocations of) the same
     * Java virtual machine.  This relationship typically does not hold,
     * however, when a {@code file:} URI that is created in a virtual machine
     * on one operating system is converted into an abstract pathname in a
     * virtual machine on a different operating system.
     *
     * @param  uri
     *         An absolute, hierarchical URI with a scheme equal to
     *         {@code "file"}, a non-empty path component, and undefined
     *         authority, query, and fragment components
     *
     * @throws  NullPointerException
     *          If {@code uri} is {@code null}
     *
     * @throws  IllegalArgumentException
     *          If the preconditions on the parameter do not hold
     *
     * @see #toURI()
     * @see java.net.URI
     * @since 1.4
     */
    // TODO: public File(URI uri) {

    //    // Check our many preconditions
    //    if (!uri.isAbsolute())
    //        throw new IllegalArgumentException("URI is not absolute");
    //    if (uri.isOpaque())
    //        throw new IllegalArgumentException("URI is not hierarchical");
    //    String scheme = uri.getScheme();
    //    if ((scheme == null) || !scheme.equalsIgnoreCase("file"))
    //        throw new IllegalArgumentException("URI scheme is not \"file\"");
    //    if (uri.getRawAuthority() != null)
    //        throw new IllegalArgumentException("URI has an authority component");
    //    if (uri.getRawFragment() != null)
    //        throw new IllegalArgumentException("URI has a fragment component");
    //    if (uri.getRawQuery() != null)
    //        throw new IllegalArgumentException("URI has a query component");
    //    String p = uri.getPath();
    //    if (p.isEmpty())
    //        throw new IllegalArgumentException("URI path component is empty");

    //    // Okay, now initialize
    //    p = FS.fromURIPath(p);
    //    if (File.separatorChar != '/')
    //        p = p.replace('/', File.separatorChar);
    //    this.path = FS.normalize(p);
    //    this.prefixLength = FS.prefixLength(this.path);
    //}


    /* -- Path-component accessors -- */

    /**
     * Returns the name of the file or directory denoted by this abstract
     * pathname.  This is just the last name in the pathname's name
     * sequence.  If the pathname's name sequence is empty, then the empty
     * string is returned.
     *
     * @return  The name of the file or directory denoted by this abstract
     *          pathname, or the empty string if this pathname's name sequence
     *          is empty
     */
    public String getName() {
        int index = path.LastIndexOf(separatorChar);
        if (index < prefixLength) return path.Substring(prefixLength);
        return path.Substring(index + 1);
    }

    /**
     * Returns the pathname string of this abstract pathname's parent, or
     * {@code null} if this pathname does not name a parent directory.
     *
     * <p> The <em>parent</em> of an abstract pathname consists of the
     * pathname's prefix, if any, and each name in the pathname's name
     * sequence except for the last.  If the name sequence is empty then
     * the pathname does not name a parent directory.
     *
     * @return  The pathname string of the parent directory named by this
     *          abstract pathname, or {@code null} if this pathname
     *          does not name a parent
     */
    public String getParent() {
        int index = path.LastIndexOf(separatorChar);
        if (index < prefixLength) {
            if ((prefixLength > 0) && (path.Length > prefixLength))
                return path.Substring(0, prefixLength);
            return null;
        }
        return path.Substring(0, index);
    }

    /**
     * Returns the abstract pathname of this abstract pathname's parent,
     * or {@code null} if this pathname does not name a parent
     * directory.
     *
     * <p> The <em>parent</em> of an abstract pathname consists of the
     * pathname's prefix, if any, and each name in the pathname's name
     * sequence except for the last.  If the name sequence is empty then
     * the pathname does not name a parent directory.
     *
     * @return  The abstract pathname of the parent directory named by this
     *          abstract pathname, or {@code null} if this pathname
     *          does not name a parent
     *
     * @since 1.2
     */
    public File getParentFile() {
        String p = this.getParent();
        if (p == null) return null;
        if (GetType() != typeof(File)) {
            p = FS.normalize(p);
        }
        return new File(p, this.prefixLength);
    }

    /**
     * Converts this abstract pathname into a pathname string.  The resulting
     * string uses the {@link #separator default name-separator character} to
     * separate the names in the name sequence.
     *
     * @return  The string form of this abstract pathname
     */
    public String getPath() {
        return path;
    }


    /* -- Path operations -- */

    /**
     * Tests whether this abstract pathname is absolute.  The definition of
     * absolute pathname is system dependent.  On UNIX systems, a pathname is
     * absolute if its prefix is {@code "/"}.  On Microsoft Windows systems, a
     * pathname is absolute if its prefix is a drive specifier followed by
     * {@code "\\"}, or if its prefix is {@code "\\\\"}.
     *
     * @return  {@code true} if this abstract pathname is absolute,
     *          {@code false} otherwise
     */
    public bool isAbsolute() {
        return FS.isAbsolute(this);
    }

    /**
     * Returns the absolute pathname string of this abstract pathname.
     *
     * <p> If this abstract pathname is already absolute, then the pathname
     * string is simply returned as if by the {@link #getPath}
     * method.  If this abstract pathname is the empty abstract pathname then
     * the pathname string of the current user directory, which is named by the
     * system property {@code user.dir}, is returned.  Otherwise this
     * pathname is resolved in a system-dependent way.  On UNIX systems, a
     * relative pathname is made absolute by resolving it against the current
     * user directory.  On Microsoft Windows systems, a relative pathname is made absolute
     * by resolving it against the current directory of the drive named by the
     * pathname, if any; if not, it is resolved against the current user
     * directory.
     *
     * @return  The absolute pathname string denoting the same file or
     *          directory as this abstract pathname
     *
     * @throws  SecurityException
     *          If a required system property value cannot be accessed.
     *
     * @see     java.io.File#isAbsolute()
     */
    public String getAbsolutePath() {
        return FS.resolve(this);
    }

    /**
     * Returns the absolute form of this abstract pathname.  Equivalent to
     * <code>new&nbsp;File(this.{@link #getAbsolutePath})</code>.
     *
     * @return  The absolute abstract pathname denoting the same file or
     *          directory as this abstract pathname
     *
     * @throws  SecurityException
     *          If a required system property value cannot be accessed.
     *
     * @since 1.2
     */
    public File getAbsoluteFile() {
        String absPath = getAbsolutePath();
        if (GetType() != typeof(File)) {
            absPath = FS.normalize(absPath);
        }
        return new File(absPath, FS.prefixLength(absPath));
    }

    /**
     * Returns the canonical pathname string of this abstract pathname.
     *
     * <p> A canonical pathname is both absolute and unique.  The precise
     * definition of canonical form is system-dependent.  This method first
     * converts this pathname to absolute form if necessary, as if by invoking the
     * {@link #getAbsolutePath} method, and then maps it to its unique form in a
     * system-dependent way.  This typically involves removing redundant names
     * such as {@code "."} and {@code ".."} from the pathname, resolving
     * symbolic links (on UNIX platforms), and converting drive letters to a
     * standard case (on Microsoft Windows platforms).
     *
     * <p> Every pathname that denotes an existing file or directory has a
     * unique canonical form.  Every pathname that denotes a nonexistent file
     * or directory also has a unique canonical form.  The canonical form of
     * the pathname of a nonexistent file or directory may be different from
     * the canonical form of the same pathname after the file or directory is
     * created.  Similarly, the canonical form of the pathname of an existing
     * file or directory may be different from the canonical form of the same
     * pathname after the file or directory is deleted.
     *
     * @return  The canonical pathname string denoting the same file or
     *          directory as this abstract pathname
     *
     * @throws  IOException
     *          If an I/O error occurs, which is possible because the
     *          construction of the canonical pathname may require
     *          filesystem queries
     *
     * @throws  SecurityException
     *          If a required system property value cannot be accessed, or
     *          if a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead} method denies
     *          read access to the file
     *
     * @since   1.1
     * @see     Path#toRealPath
     */
    public String getCanonicalPath()// throws IOException 
    {
        if (isInvalid()) {
            throw new IOException("Invalid file path");
        }
        return FS.canonicalize(FS.resolve(this));
    }

    /**
     * Returns the canonical form of this abstract pathname.  Equivalent to
     * <code>new&nbsp;File(this.{@link #getCanonicalPath})</code>.
     *
     * @return  The canonical pathname string denoting the same file or
     *          directory as this abstract pathname
     *
     * @throws  IOException
     *          If an I/O error occurs, which is possible because the
     *          construction of the canonical pathname may require
     *          filesystem queries
     *
     * @throws  SecurityException
     *          If a required system property value cannot be accessed, or
     *          if a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead} method denies
     *          read access to the file
     *
     * @since 1.2
     * @see     Path#toRealPath
     */
    public File getCanonicalFile()// throws IOException 
    {
        String canonPath = getCanonicalPath();
        if (GetType() != typeof(File)) {
            canonPath = FS.normalize(canonPath);
        }
        return new File(canonPath, FS.prefixLength(canonPath));
    }

    private static String slashify(String path, bool isDirectory) {
        String p = path;
        if (File.separatorChar != '/')
            p = p.Replace(File.separatorChar, '/');
        if (!p.StartsWith("/"))
            p = "/" + p;
        if (!p.EndsWith("/") && isDirectory)
            p = p + "/";
        return p;
    }

    /**
     * Converts this abstract pathname into a {@code file:} URL.  The
     * exact form of the URL is system-dependent.  If it can be determined that
     * the file denoted by this abstract pathname is a directory, then the
     * resulting URL will end with a slash.
     *
     * @return  A URL object representing the equivalent file URL
     *
     * @throws  MalformedURLException
     *          If the path cannot be parsed as a URL
     *
     * @see     #toURI()
     * @see     java.net.URI
     * @see     java.net.URI#toURL()
     * @see     java.net.URL
     * @since   1.2
     *
     * @deprecated This method does not automatically escape characters that
     * are illegal in URLs.  It is recommended that new code convert an
     * abstract pathname into a URL by first converting it into a URI, via the
     * {@link #toURI() toURI} method, and then converting the URI into a URL
     * via the {@link java.net.URI#toURL() URI.toURL} method.
     */
    //[Obsolete]
    // TODO: public URL toURL() //throws MalformedURLException 
    //{
    //    if (isInvalid()) {
    //        throw new MalformedURLException("Invalid file path");
    //    }
    //    @SuppressWarnings("deprecation")
    //    var result = new URL("file", "", slashify(getAbsolutePath(), isDirectory()));
    //    return result;
    //}

    /**
     * Constructs a {@code file:} URI that represents this abstract pathname.
     *
     * <p> The exact form of the URI is system-dependent.  If it can be
     * determined that the file denoted by this abstract pathname is a
     * directory, then the resulting URI will end with a slash.
     *
     * <p> For a given abstract pathname <i>f</i>, it is guaranteed that
     *
     * <blockquote><code>
     * new {@link #File(java.net.URI) File}(</code><i>&nbsp;f</i><code>.toURI()).equals(
     * </code><i>&nbsp;f</i><code>.{@link #getAbsoluteFile() getAbsoluteFile}())
     * </code></blockquote>
     *
     * so long as the original abstract pathname, the URI, and the new abstract
     * pathname are all created in (possibly different invocations of) the same
     * Java virtual machine.  Due to the system-dependent nature of abstract
     * pathnames, however, this relationship typically does not hold when a
     * {@code file:} URI that is created in a virtual machine on one operating
     * system is converted into an abstract pathname in a virtual machine on a
     * different operating system.
     *
     * <p> Note that when this abstract pathname represents a UNC pathname then
     * all components of the UNC (including the server name component) are encoded
     * in the {@code URI} path. The authority component is undefined, meaning
     * that it is represented as {@code null}. The {@link Path} class defines the
     * {@link Path#toUri toUri} method to encode the server name in the authority
     * component of the resulting {@code URI}. The {@link #toPath toPath} method
     * may be used to obtain a {@code Path} representing this abstract pathname.
     *
     * @return  An absolute, hierarchical URI with a scheme equal to
     *          {@code "file"}, a path representing this abstract pathname,
     *          and undefined authority, query, and fragment components
     * @throws SecurityException If a required system property value cannot
     * be accessed.
     *
     * @see #File(java.net.URI)
     * @see java.net.URI
     * @see java.net.URI#toURL()
     * @since 1.4
     */
    // TODO: public URI toURI() {
    //    try {
    //        File f = getAbsoluteFile();
    //        String sp = slashify(f.getPath(), f.isDirectory());
    //        if (sp.startsWith("//"))
    //            sp = "//" + sp;
    //        return new URI("file", null, sp, null);
    //    } catch (URISyntaxException x) {
    //        throw new Error(x);         // Can't happen
    //    }
    //}


    /* -- Attribute accessors -- */

    /**
     * Tests whether the application can read the file denoted by this
     * abstract pathname. On some platforms it may be possible to start the
     * Java virtual machine with special privileges that allow it to read
     * files that are marked as unreadable. Consequently, this method may return
     * {@code true} even though the file does not have read permissions.
     *
     * @return  {@code true} if and only if the file specified by this
     *          abstract pathname exists <em>and</em> can be read by the
     *          application; {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     */
    public bool canRead() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.checkAccess(this, FileSystem.ACCESS_READ);
    }

    /**
     * Tests whether the application can modify the file denoted by this
     * abstract pathname. On some platforms it may be possible to start the
     * Java virtual machine with special privileges that allow it to modify
     * files that are marked read-only. Consequently, this method may return
     * {@code true} even though the file is marked read-only.
     *
     * @return  {@code true} if and only if the file system actually
     *          contains a file denoted by this abstract pathname <em>and</em>
     *          the application is allowed to write to the file;
     *          {@code false} otherwise.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     */
    public bool canWrite() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.checkAccess(this, FileSystem.ACCESS_WRITE);
    }

    /**
     * Tests whether the file or directory denoted by this abstract pathname
     * exists.
     *
     * @return  {@code true} if and only if the file or directory denoted
     *          by this abstract pathname exists; {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file or directory
     */
    public bool exists() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return false;
        }
        // TODO: return FS.hasBooleanAttributes(this, FileSystem.BA_EXISTS);
        // TODO: Canonical and Absolute Path are wrong.
        return System.IO.File.Exists(this.path);
    }

    /**
     * Tests whether the file denoted by this abstract pathname is a
     * directory.
     *
     * <p> Where it is required to distinguish an I/O exception from the case
     * that the file is not a directory, or where several attributes of the
     * same file are required at the same time, then the {@link
     * java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
     * Files.readAttributes} method may be used.
     *
     * @return {@code true} if and only if the file denoted by this
     *          abstract pathname exists <em>and</em> is a directory;
     *          {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     */
    public bool isDirectory() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.hasBooleanAttributes(this, FileSystem.BA_DIRECTORY);
    }

    /**
     * Tests whether the file denoted by this abstract pathname is a normal
     * file.  A file is <em>normal</em> if it is not a directory and, in
     * addition, satisfies other system-dependent criteria.  Any non-directory
     * file created by a Java application is guaranteed to be a normal file.
     *
     * <p> Where it is required to distinguish an I/O exception from the case
     * that the file is not a normal file, or where several attributes of the
     * same file are required at the same time, then the {@link
     * java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
     * Files.readAttributes} method may be used.
     *
     * @return  {@code true} if and only if the file denoted by this
     *          abstract pathname exists <em>and</em> is a normal file;
     *          {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     */
    public bool isFile() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.hasBooleanAttributes(this, FileSystem.BA_REGULAR);
    }

    /**
     * Tests whether the file named by this abstract pathname is a hidden
     * file.  The exact definition of <em>hidden</em> is system-dependent.  On
     * UNIX systems, a file is considered to be hidden if its name begins with
     * a period character ({@code '.'}).  On Microsoft Windows systems, a file is
     * considered to be hidden if it has been marked as such in the filesystem.
     *
     * @return  {@code true} if and only if the file denoted by this
     *          abstract pathname is hidden according to the conventions of the
     *          underlying platform
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     *
     * @since 1.2
     */
    public bool isHidden() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.hasBooleanAttributes(this, FileSystem.BA_HIDDEN);
    }

    /**
     * Returns the time that the file denoted by this abstract pathname was
     * last modified.
     *
     * @apiNote
     * While the unit of time of the return value is milliseconds, the
     * granularity of the value depends on the underlying file system and may
     * be larger.  For example, some file systems use time stamps in units of
     * seconds.
     *
     * <p> Where it is required to distinguish an I/O exception from the case
     * where {@code 0L} is returned, or where several attributes of the
     * same file are required at the same time, or where the time of last
     * access or the creation time are required, then the {@link
     * java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
     * Files.readAttributes} method may be used.  If however only the
     * time of last modification is required, then the
     * {@link java.nio.file.Files#getLastModifiedTime(Path,LinkOption[])
     * Files.getLastModifiedTime} method may be used instead.
     *
     * @return  A {@code long} value representing the time the file was
     *          last modified, measured in milliseconds since the epoch
     *          (00:00:00 GMT, January 1, 1970), or {@code 0L} if the
     *          file does not exist or if an I/O error occurs.  The value may
     *          be negative indicating the number of milliseconds before the
     *          epoch
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     */
    public long lastModified() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return 0L;
        }
        return FS.getLastModifiedTime(this);
    }

    /**
     * Returns the length of the file denoted by this abstract pathname.
     * The return value is unspecified if this pathname denotes a directory.
     *
     * <p> Where it is required to distinguish an I/O exception from the case
     * that {@code 0L} is returned, or where several attributes of the same file
     * are required at the same time, then the {@link
     * java.nio.file.Files#readAttributes(Path,Class,LinkOption[])
     * Files.readAttributes} method may be used.
     *
     * @return  The length, in bytes, of the file denoted by this abstract
     *          pathname, or {@code 0L} if the file does not exist.  Some
     *          operating systems may return {@code 0L} for pathnames
     *          denoting system-dependent entities such as devices or pipes.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method denies read access to the file
     */
    public long length() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return 0L;
        }
        return FS.getLength(this);
    }


    /* -- File operations -- */

    /**
     * Atomically creates a new, empty file named by this abstract pathname if
     * and only if a file with this name does not yet exist.  The check for the
     * existence of the file and the creation of the file if it does not exist
     * are a single operation that is atomic with respect to all other
     * filesystem activities that might affect the file.
     * <P>
     * Note: this method should <i>not</i> be used for file-locking, as
     * the resulting protocol cannot be made to work reliably. The
     * {@link java.nio.channels.FileLock FileLock}
     * facility should be used instead.
     *
     * @return  {@code true} if the named file does not exist and was
     *          successfully created; {@code false} if the named file
     *          already exists
     *
     * @throws  IOException
     *          If an I/O error occurred
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.2
     */
    public bool createNewFile() //throws IOException 
    {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) security.checkWrite(path);
        if (isInvalid()) {
            throw new IOException("Invalid file path");
        }
        return FS.createFileExclusively(path);
    }

    /**
     * Deletes the file or directory denoted by this abstract pathname.  If
     * this pathname denotes a directory, then the directory must be empty in
     * order to be deleted.
     *
     * <p> Note that the {@link java.nio.file.Files} class defines the {@link
     * java.nio.file.Files#delete(Path) delete} method to throw an {@link IOException}
     * when a file cannot be deleted. This is useful for error reporting and to
     * diagnose why a file cannot be deleted.
     *
     * @return  {@code true} if and only if the file or directory is
     *          successfully deleted; {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkDelete} method denies
     *          delete access to the file
     */
    public bool delete() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkDelete(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.delete(this);
    }

    /**
     * Requests that the file or directory denoted by this abstract
     * pathname be deleted when the virtual machine terminates.
     * Files (or directories) are deleted in the reverse order that
     * they are registered. Invoking this method to delete a file or
     * directory that is already registered for deletion has no effect.
     * Deletion will be attempted only for normal termination of the
     * virtual machine, as defined by the Java Language Specification.
     *
     * <p> Once deletion has been requested, it is not possible to cancel the
     * request.  This method should therefore be used with care.
     *
     * <P>
     * Note: this method should <i>not</i> be used for file-locking, as
     * the resulting protocol cannot be made to work reliably. The
     * {@link java.nio.channels.FileLock FileLock}
     * facility should be used instead.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkDelete} method denies
     *          delete access to the file
     *
     * @see #delete
     *
     * @since 1.2
     */
    //public void deleteOnExit() {
    //    // TODO: @SuppressWarnings("removal")
    //    // TODO: SecurityManager security = System.getSecurityManager();
    //    //if (security != null) {
    //    //    security.checkDelete(path);
    //    //}
    //    if (isInvalid()) {
    //        return;
    //    }
    //    DeleteOnExitHook.add(path);
    //}

    /**
     * Returns an array of strings naming the files and directories in the
     * directory denoted by this abstract pathname.
     *
     * <p> If this abstract pathname does not denote a directory, then this
     * method returns {@code null}.  Otherwise an array of strings is
     * returned, one for each file or directory in the directory.  Names
     * denoting the directory itself and the directory's parent directory are
     * not included in the result.  Each string is a file name rather than a
     * complete path.
     *
     * <p> There is no guarantee that the name strings in the resulting array
     * will appear in any specific order; they are not, in particular,
     * guaranteed to appear in alphabetical order.
     *
     * <p> Note that the {@link java.nio.file.Files} class defines the {@link
     * java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method to
     * open a directory and iterate over the names of the files in the directory.
     * This may use less resources when working with very large directories, and
     * may be more responsive when working with remote directories.
     *
     * @return  An array of strings naming the files and directories in the
     *          directory denoted by this abstract pathname.  The array will be
     *          empty if the directory is empty.  Returns {@code null} if
     *          this abstract pathname does not denote a directory, or if an
     *          I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     */
    public String[] list() {
        return normalizedList();
    }

    /**
     * Returns an array of strings naming the files and directories in the
     * directory denoted by this abstract pathname.  The strings are
     * ensured to represent normalized paths.
     *
     * @return  An array of strings naming the files and directories in the
     *          directory denoted by this abstract pathname.  The array will be
     *          empty if the directory is empty.  Returns {@code null} if
     *          this abstract pathname does not denote a directory, or if an
     *          I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     */
    private String[] normalizedList() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkRead(path);
        //}
        if (isInvalid()) {
            return null;
        }
        String[] s = FS.list(this);
        if (s != null && GetType() != typeof(File)) {
            String[] normalized = new String[s.Length];
            for (int i = 0; i < s.Length; i++) {
                normalized[i] = FS.normalize(s[i]);
            }
            s = normalized;
        }
        return s;
    }

    /**
     * Returns an array of strings naming the files and directories in the
     * directory denoted by this abstract pathname that satisfy the specified
     * filter.  The behavior of this method is the same as that of the
     * {@link #list()} method, except that the strings in the returned array
     * must satisfy the filter.  If the given {@code filter} is {@code null}
     * then all names are accepted.  Otherwise, a name satisfies the filter if
     * and only if the value {@code true} results when the {@link
     * FilenameFilter#accept FilenameFilter.accept(File,&nbsp;String)} method
     * of the filter is invoked on this abstract pathname and the name of a
     * file or directory in the directory that it denotes.
     *
     * @param  filter
     *         A filename filter
     *
     * @return  An array of strings naming the files and directories in the
     *          directory denoted by this abstract pathname that were accepted
     *          by the given {@code filter}.  The array will be empty if the
     *          directory is empty or if no names were accepted by the filter.
     *          Returns {@code null} if this abstract pathname does not denote
     *          a directory, or if an I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     *
     * @see java.nio.file.Files#newDirectoryStream(Path,String)
     */
    public String[] list(FilenameFilter filter) {
        String[] names = normalizedList();
        if ((names == null) || (filter == null)) {
            return names;
        }
        List<String> v = new ();
        for (int i = 0 ; i < names.Length ; i++) {
            if (filter.accept(this, names[i])) {
                v.Add(names[i]);
            }
        }
        return v.ToArray();
    }

    /**
     * Returns an array of abstract pathnames denoting the files in the
     * directory denoted by this abstract pathname.
     *
     * <p> If this abstract pathname does not denote a directory, then this
     * method returns {@code null}.  Otherwise an array of {@code File} objects
     * is returned, one for each file or directory in the directory.  Pathnames
     * denoting the directory itself and the directory's parent directory are
     * not included in the result.  Each resulting abstract pathname is
     * constructed from this abstract pathname using the {@link #File(File,
     * String) File(File,&nbsp;String)} constructor.  Therefore if this
     * pathname is absolute then each resulting pathname is absolute; if this
     * pathname is relative then each resulting pathname will be relative to
     * the same directory.
     *
     * <p> There is no guarantee that the name strings in the resulting array
     * will appear in any specific order; they are not, in particular,
     * guaranteed to appear in alphabetical order.
     *
     * <p> Note that the {@link java.nio.file.Files} class defines the {@link
     * java.nio.file.Files#newDirectoryStream(Path) newDirectoryStream} method
     * to open a directory and iterate over the names of the files in the
     * directory. This may use less resources when working with very large
     * directories.
     *
     * @return  An array of abstract pathnames denoting the files and
     *          directories in the directory denoted by this abstract pathname.
     *          The array will be empty if the directory is empty.  Returns
     *          {@code null} if this abstract pathname does not denote a
     *          directory, or if an I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     *
     * @since  1.2
     */
    public File[] listFiles() {
        String[] ss = normalizedList();
        if (ss == null) return null;
        int n = ss.Length;
        File[] fs = new File[n];
        for (int i = 0; i < n; i++) {
            fs[i] = new File(ss[i], this);
        }
        return fs;
    }

    /**
     * Returns an array of abstract pathnames denoting the files and
     * directories in the directory denoted by this abstract pathname that
     * satisfy the specified filter.  The behavior of this method is the same
     * as that of the {@link #listFiles()} method, except that the pathnames in
     * the returned array must satisfy the filter.  If the given {@code filter}
     * is {@code null} then all pathnames are accepted.  Otherwise, a pathname
     * satisfies the filter if and only if the value {@code true} results when
     * the {@link FilenameFilter#accept
     * FilenameFilter.accept(File,&nbsp;String)} method of the filter is
     * invoked on this abstract pathname and the name of a file or directory in
     * the directory that it denotes.
     *
     * @param  filter
     *         A filename filter
     *
     * @return  An array of abstract pathnames denoting the files and
     *          directories in the directory denoted by this abstract pathname.
     *          The array will be empty if the directory is empty.  Returns
     *          {@code null} if this abstract pathname does not denote a
     *          directory, or if an I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     *
     * @since  1.2
     * @see java.nio.file.Files#newDirectoryStream(Path,String)
     */
    public File[] listFiles(FilenameFilter filter) {
        String[] ss = normalizedList();
        if (ss == null) return null;
        List<File> files = new ();
        foreach (String s in ss)
            if ((filter == null) || filter.accept(this, s))
                files.Add(new File(s, this));
        return files.ToArray();
    }

    /**
     * Returns an array of abstract pathnames denoting the files and
     * directories in the directory denoted by this abstract pathname that
     * satisfy the specified filter.  The behavior of this method is the same
     * as that of the {@link #listFiles()} method, except that the pathnames in
     * the returned array must satisfy the filter.  If the given {@code filter}
     * is {@code null} then all pathnames are accepted.  Otherwise, a pathname
     * satisfies the filter if and only if the value {@code true} results when
     * the {@link FileFilter#accept FileFilter.accept(File)} method of the
     * filter is invoked on the pathname.
     *
     * @param  filter
     *         A file filter
     *
     * @return  An array of abstract pathnames denoting the files and
     *          directories in the directory denoted by this abstract pathname.
     *          The array will be empty if the directory is empty.  Returns
     *          {@code null} if this abstract pathname does not denote a
     *          directory, or if an I/O error occurs.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          SecurityManager#checkRead(String)} method denies read access to
     *          the directory
     *
     * @since  1.2
     * @see java.nio.file.Files#newDirectoryStream(Path,java.nio.file.DirectoryStream.Filter)
     */
    public File[] listFiles(FileFilter filter) {
        String[] ss = normalizedList();
        if (ss == null) return null;
        List<File> files = new();
        foreach (String s in ss) {
            File f = new File(s, this);
            if ((filter == null) || filter.accept(f))
                files.Add(f);
        }
        return files.ToArray();
    }

    /**
     * Creates the directory named by this abstract pathname.
     *
     * @return  {@code true} if and only if the directory was
     *          created; {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method does not permit the named directory to be created
     */
    public bool mkdir() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.createDirectory(this);
    }

    /**
     * Creates the directory named by this abstract pathname, including any
     * necessary but nonexistent parent directories.  Note that if this
     * operation fails it may have succeeded in creating some of the necessary
     * parent directories.
     *
     * @return  {@code true} if and only if the directory was created,
     *          along with all necessary parent directories; {@code false}
     *          otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkRead(java.lang.String)}
     *          method does not permit verification of the existence of the
     *          named directory and all necessary parent directories; or if
     *          the {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method does not permit the named directory and all necessary
     *          parent directories to be created
     */
    public bool mkdirs() {
        if (exists()) {
            return false;
        }
        if (mkdir()) {
            return true;
        }
        File canonFile = null;
        try {
            canonFile = getCanonicalFile();
        } catch (IOException e) {
            return false;
        }

        File parent = canonFile.getParentFile();
        return (parent != null && (parent.mkdirs() || parent.exists()) &&
                canonFile.mkdir());
    }

    /**
     * Renames the file denoted by this abstract pathname.
     *
     * <p> Many aspects of the behavior of this method are inherently
     * platform-dependent: The rename operation might not be able to move a
     * file from one filesystem to another, it might not be atomic, and it
     * might not succeed if a file with the destination abstract pathname
     * already exists.  The return value should always be checked to make sure
     * that the rename operation was successful.  As instances of {@code File}
     * are immutable, this File object is not changed to name the destination
     * file or directory.
     *
     * <p> Note that the {@link java.nio.file.Files} class defines the {@link
     * java.nio.file.Files#move move} method to move or rename a file in a
     * platform independent manner.
     *
     * @param  dest  The new abstract pathname for the named file
     *
     * @return  {@code true} if and only if the renaming succeeded;
     *          {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to either the old or new pathnames
     *
     * @throws  NullPointerException
     *          If parameter {@code dest} is {@code null}
     */
    public bool renameTo(File dest) {
        if (dest == null) {
            throw new NullPointerException();
        }
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //    security.checkWrite(dest.path);
        //}
        if (this.isInvalid() || dest.isInvalid()) {
            return false;
        }
        return FS.rename(this, dest);
    }

    /**
     * Sets the last-modified time of the file or directory named by this
     * abstract pathname.
     *
     * <p> All platforms support file-modification times to the nearest second,
     * but some provide more precision.  The argument will be truncated to fit
     * the supported precision.  If the operation succeeds and no intervening
     * operations on the file take place, then the next invocation of the
     * {@link #lastModified} method will return the (possibly
     * truncated) {@code time} argument that was passed to this method.
     *
     * @param  time  The new last-modified time, measured in milliseconds since
     *               the epoch (00:00:00 GMT, January 1, 1970)
     *
     * @return {@code true} if and only if the operation succeeded;
     *          {@code false} otherwise
     *
     * @throws  IllegalArgumentException  If the argument is negative
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the named file
     *
     * @since 1.2
     */
    public bool setLastModified(long time) {
        if (time < 0) throw new IllegalArgumentException("Negative time");
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.setLastModifiedTime(this, time);
    }

    /**
     * Marks the file or directory named by this abstract pathname so that
     * only read operations are allowed. After invoking this method the file
     * or directory will not change until it is either deleted or marked
     * to allow write access. On some platforms it may be possible to start the
     * Java virtual machine with special privileges that allow it to modify
     * files that are marked read-only. Whether or not a read-only file or
     * directory may be deleted depends upon the underlying system.
     *
     * @return {@code true} if and only if the operation succeeded;
     *          {@code false} otherwise
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the named file
     *
     * @since 1.2
     */
    public bool setReadOnly() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.setReadOnly(this);
    }

    /**
     * Sets the owner's or everybody's write permission for this abstract
     * pathname. On some platforms it may be possible to start the Java virtual
     * machine with special privileges that allow it to modify files that
     * disallow write operations.
     *
     * <p> The {@link java.nio.file.Files} class defines methods that operate on
     * file attributes including file permissions. This may be used when finer
     * manipulation of file permissions is required.
     *
     * @param   writable
     *          If {@code true}, sets the access permission to allow write
     *          operations; if {@code false} to disallow write operations
     *
     * @param   ownerOnly
     *          If {@code true}, the write permission applies only to the
     *          owner's write permission; otherwise, it applies to everybody.  If
     *          the underlying file system can not distinguish the owner's write
     *          permission from that of others, then the permission will apply to
     *          everybody, regardless of this value.
     *
     * @return  {@code true} if and only if the operation succeeded. The
     *          operation will fail if the user does not have permission to change
     *          the access permissions of this abstract pathname.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the named file
     *
     * @since 1.6
     */
    public bool setWritable(bool writable, bool ownerOnly) {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.setPermission(this, FileSystem.ACCESS_WRITE, writable, ownerOnly);
    }

    /**
     * A convenience method to set the owner's write permission for this abstract
     * pathname. On some platforms it may be possible to start the Java virtual
     * machine with special privileges that allow it to modify files that
     * disallow write operations.
     *
     * <p> An invocation of this method of the form {@code file.setWritable(arg)}
     * behaves in exactly the same way as the invocation
     *
     * {@snippet lang=java :
     *     file.setWritable(arg, true)
     * }
     *
     * @param   writable
     *          If {@code true}, sets the access permission to allow write
     *          operations; if {@code false} to disallow write operations
     *
     * @return  {@code true} if and only if the operation succeeded.  The
     *          operation will fail if the user does not have permission to
     *          change the access permissions of this abstract pathname.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.6
     */
    public bool setWritable(bool writable) {
        return setWritable(writable, true);
    }

    /**
     * Sets the owner's or everybody's read permission for this abstract
     * pathname. On some platforms it may be possible to start the Java virtual
     * machine with special privileges that allow it to read files that are
     * marked as unreadable.
     *
     * <p> The {@link java.nio.file.Files} class defines methods that operate on
     * file attributes including file permissions. This may be used when finer
     * manipulation of file permissions is required.
     *
     * <p> If the platform supports setting a file's read permission, but
     * the user does not have permission to change the access permissions of
     * this abstract pathname, then the operation will fail. If the platform
     * does not support setting a file's read permission, this method does
     * nothing and returns the value of the {@code readable} parameter.
     *
     * @param   readable
     *          If {@code true}, sets the access permission to allow read
     *          operations; if {@code false} to disallow read operations
     *
     * @param   ownerOnly
     *          If {@code true}, the read permission applies only to the
     *          owner's read permission; otherwise, it applies to everybody.  If
     *          the underlying file system can not distinguish the owner's read
     *          permission from that of others, then the permission will apply to
     *          everybody, regardless of this value.
     *
     * @return  {@code true} if the operation succeeds, {@code false} if it
     *          fails, or the value of the {@code readable} parameter if
     *          setting the read permission is not supported.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.6
     */
    public bool setReadable(bool readable, bool ownerOnly) {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.setPermission(this, FileSystem.ACCESS_READ, readable, ownerOnly);
    }

    /**
     * A convenience method to set the owner's read permission for this abstract
     * pathname. On some platforms it may be possible to start the Java virtual
     * machine with special privileges that allow it to read files that are
     * marked as unreadable.
     *
     * <p>An invocation of this method of the form {@code file.setReadable(arg)}
     * behaves in exactly the same way as the invocation
     *
     * {@snippet lang=java :
     *     file.setReadable(arg, true)
     * }
     *
     * <p> If the platform supports setting a file's read permission, but
     * the user does not have permission to change the access permissions of
     * this abstract pathname, then the operation will fail. If the platform
     * does not support setting a file's read permission, this method does
     * nothing and returns the value of the {@code readable} parameter.
     *
     * @param  readable
     *          If {@code true}, sets the access permission to allow read
     *          operations; if {@code false} to disallow read operations
     *
     * @return  {@code true} if the operation succeeds, {@code false} if it
     *          fails, or the value of the {@code readable} parameter if
     *          setting the read permission is not supported.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.6
     */
    public bool setReadable(bool readable) {
        return setReadable(readable, true);
    }

    /**
     * Sets the owner's or everybody's execute permission for this abstract
     * pathname. On some platforms it may be possible to start the Java virtual
     * machine with special privileges that allow it to execute files that are
     * not marked executable.
     *
     * <p> The {@link java.nio.file.Files} class defines methods that operate on
     * file attributes including file permissions. This may be used when finer
     * manipulation of file permissions is required.
     *
     * <p> If the platform supports setting a file's execute permission, but
     * the user does not have permission to change the access permissions of
     * this abstract pathname, then the operation will fail. If the platform
     * does not support setting a file's execute permission, this method does
     * nothing and returns the value of the {@code executable} parameter.
     *
     * @param   executable
     *          If {@code true}, sets the access permission to allow execute
     *          operations; if {@code false} to disallow execute operations
     *
     * @param   ownerOnly
     *          If {@code true}, the execute permission applies only to the
     *          owner's execute permission; otherwise, it applies to everybody.
     *          If the underlying file system can not distinguish the owner's
     *          execute permission from that of others, then the permission will
     *          apply to everybody, regardless of this value.
     *
     * @return  {@code true} if the operation succeeds, {@code false} if it
     *          fails, or the value of the {@code executable} parameter if
     *          setting the execute permission is not supported.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.6
     */
    public bool setExecutable(bool executable, bool ownerOnly) {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkWrite(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.setPermission(this, FileSystem.ACCESS_EXECUTE, executable, ownerOnly);
    }

    /**
     * A convenience method to set the owner's execute permission for this
     * abstract pathname. On some platforms it may be possible to start the Java
     * virtual machine with special privileges that allow it to execute files
     * that are not marked executable.
     *
     * <p>An invocation of this method of the form {@code file.setExcutable(arg)}
     * behaves in exactly the same way as the invocation
     *
     * {@snippet lang=java :
     *     file.setExecutable(arg, true)
     * }
     *
     * <p> If the platform supports setting a file's execute permission, but
     * the user does not have permission to change the access permissions of
     * this abstract pathname, then the operation will fail. If the platform
     * does not support setting a file's execute permission, this method does
     * nothing and returns the value of the {@code executable} parameter.
     *
     * @param   executable
     *          If {@code true}, sets the access permission to allow execute
     *          operations; if {@code false} to disallow execute operations
     *
     * @return  {@code true} if the operation succeeds, {@code false} if it
     *          fails, or the value of the {@code executable} parameter if
     *          setting the execute permission is not supported.
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method denies write access to the file
     *
     * @since 1.6
     */
    public bool setExecutable(bool executable) {
        return setExecutable(executable, true);
    }

    /**
     * Tests whether the application can execute the file denoted by this
     * abstract pathname. On some platforms it may be possible to start the
     * Java virtual machine with special privileges that allow it to execute
     * files that are not marked executable. Consequently, this method may return
     * {@code true} even though the file does not have execute permissions.
     *
     * @return  {@code true} if and only if the abstract pathname exists
     *          <em>and</em> the application is allowed to execute the file
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkExec(java.lang.String)}
     *          method denies execute access to the file
     *
     * @since 1.6
     */
    public bool canExecute() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager security = System.getSecurityManager();
        //if (security != null) {
        //    security.checkExec(path);
        //}
        if (isInvalid()) {
            return false;
        }
        return FS.checkAccess(this, FileSystem.ACCESS_EXECUTE);
    }


    /* -- Filesystem interface -- */

    /**
     * List the available filesystem roots.
     *
     * <p> A particular Java platform may support zero or more
     * hierarchically-organized file systems.  Each file system has a
     * {@code root} directory from which all other files in that file system
     * can be reached.
     *
     * <p> This method returns an array of {@code File} objects that denote the
     * root directories of the available filesystem roots.  It is guaranteed
     * that the canonical pathname of any file physically present on the local
     * machine will begin with one of the roots returned by this method.
     * There is no guarantee that a root directory can be accessed.
     *
     * <p> Unlike most methods in this class, this method does not throw
     * security exceptions.  If a security manager exists and its {@link
     * SecurityManager#checkRead(String)} method denies read access to a
     * particular root directory, then that directory will not appear in the
     * result.
     *
     * @implNote
     * Windows platforms, for example, have a root directory
     * for each active drive; UNIX platforms have a single root directory,
     * namely {@code "/"}.  The set of filesystem roots is affected
     * by various system-level operations such as the disconnecting or
     * unmounting of physical or virtual disk drives.
     *
     * <p> The canonical pathname of a file that resides on some other machine
     * and is accessed via a remote-filesystem protocol such as SMB or NFS may
     * or may not begin with one of the roots returned by this method.  If the
     * pathname of a remote file is syntactically indistinguishable from the
     * pathname of a local file then it will begin with one of the roots
     * returned by this method.  Thus, for example, {@code File} objects
     * denoting the root directories of the mapped network drives of a Windows
     * platform will be returned by this method, while {@code File} objects
     * containing UNC pathnames will not be returned by this method.
     *
     * @return  An array of {@code File} objects denoting the available
     *          filesystem roots, or {@code null} if the set of roots could not
     *          be determined.  The array will be empty if there are no
     *          filesystem roots.
     *
     * @since  1.2
     * @see java.nio.file.FileStore
     */
    public static File[] listRoots() {
        return FS.listRoots();
    }


    /* -- Disk usage -- */

    /**
     * Returns the size of the partition <a href="#partName">named</a> by this
     * abstract pathname. If the total number of bytes in the partition is
     * greater than {@link Long#MAX_VALUE}, then {@code long.MaxValue} will be
     * returned.
     *
     * @return  The size, in bytes, of the partition or {@code 0L} if this
     *          abstract pathname does not name a partition or if the size
     *          cannot be obtained
     *
     * @throws  SecurityException
     *          If a security manager has been installed and it denies
     *          {@link RuntimePermission}{@code ("getFileSystemAttributes")}
     *          or its {@link SecurityManager#checkRead(String)} method denies
     *          read access to the file named by this abstract pathname
     *
     * @since  1.6
     * @see FileStore#getTotalSpace
     */
    public long getTotalSpace() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager sm = System.getSecurityManager();
        //if (sm != null) {
        //    sm.checkPermission(new RuntimePermission("getFileSystemAttributes"));
        //    sm.checkRead(path);
        //}
        if (isInvalid()) {
            return 0L;
        }
        long space = FS.getSpace(this, FileSystem.SPACE_TOTAL);
        return space >= 0L ? space : long.MaxValue;
    }

    /**
     * Returns the number of unallocated bytes in the partition <a
     * href="#partName">named</a> by this abstract path name.  If the
     * number of unallocated bytes in the partition is greater than
     * {@link Long#MAX_VALUE}, then {@code long.MaxValue} will be returned.
     *
     * <p> The returned number of unallocated bytes is a hint, but not
     * a guarantee, that it is possible to use most or any of these
     * bytes.  The number of unallocated bytes is most likely to be
     * accurate immediately after this call.  It is likely to be made
     * inaccurate by any external I/O operations including those made
     * on the system outside of this virtual machine.  This method
     * makes no guarantee that write operations to this file system
     * will succeed.
     *
     * @return  The number of unallocated bytes on the partition or {@code 0L}
     *          if the abstract pathname does not name a partition or if this
     *          number cannot be obtained.  This value will be less than or
     *          equal to the total file system size returned by
     *          {@link #getTotalSpace}.
     *
     * @throws  SecurityException
     *          If a security manager has been installed and it denies
     *          {@link RuntimePermission}{@code ("getFileSystemAttributes")}
     *          or its {@link SecurityManager#checkRead(String)} method denies
     *          read access to the file named by this abstract pathname
     *
     * @since  1.6
     * @see FileStore#getUnallocatedSpace
     */
    public long getFreeSpace() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager sm = System.getSecurityManager();
        //if (sm != null) {
        //    sm.checkPermission(new RuntimePermission("getFileSystemAttributes"));
        //    sm.checkRead(path);
        //}
        if (isInvalid()) {
            return 0L;
        }
        long space = FS.getSpace(this, FileSystem.SPACE_FREE);
        return space >= 0L ? space : long.MaxValue;
    }

    /**
     * Returns the number of bytes available to this virtual machine on the
     * partition <a href="#partName">named</a> by this abstract pathname.  If
     * the number of available bytes in the partition is greater than
     * {@link Long#MAX_VALUE}, then {@code long.MaxValue} will be returned.
     * When possible, this method checks for write permissions and other
     * operating system restrictions and will therefore usually provide a more
     * accurate estimate of how much new data can actually be written than
     * {@link #getFreeSpace}.
     *
     * <p> The returned number of available bytes is a hint, but not a
     * guarantee, that it is possible to use most or any of these bytes.  The
     * number of available bytes is most likely to be accurate immediately
     * after this call.  It is likely to be made inaccurate by any external
     * I/O operations including those made on the system outside of this
     * virtual machine.  This method makes no guarantee that write operations
     * to this file system will succeed.
     *
     * @return  The number of available bytes on the partition or {@code 0L}
     *          if the abstract pathname does not name a partition or if this
     *          number cannot be obtained.  On systems where this information
     *          is not available, this method will be equivalent to a call to
     *          {@link #getFreeSpace}.
     *
     * @throws  SecurityException
     *          If a security manager has been installed and it denies
     *          {@link RuntimePermission}{@code ("getFileSystemAttributes")}
     *          or its {@link SecurityManager#checkRead(String)} method denies
     *          read access to the file named by this abstract pathname
     *
     * @since  1.6
     * @see FileStore#getUsableSpace
     */
    public long getUsableSpace() {
        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager sm = System.getSecurityManager();
        //if (sm != null) {
        //    sm.checkPermission(new RuntimePermission("getFileSystemAttributes"));
        //    sm.checkRead(path);
        //}
        if (isInvalid()) {
            return 0L;
        }
        long space = FS.getSpace(this, FileSystem.SPACE_USABLE);
        return space >= 0L ? space : long.MaxValue;
    }

    /* -- Temporary files -- */

    private  class TempDirectory {
        private TempDirectory() { }

        // temporary directory location
        private static readonly File TMPDIR = new File(System.IO.Path.GetTempPath());

        internal static File location() {
            return TMPDIR;
        }

        // file name generation
        // TODO: Is this good enough? Really don't want to port SecureRandom
        //private static readonly SecureRandom RANDOM = new SecureRandom();
        private static readonly Random RANDOM = new Random();
        private static int shortenSubName(int subNameLength, int excess,
            int nameMin) {
            int newLength = Math.Max(nameMin, subNameLength - excess);
            if (newLength < subNameLength) {
                return newLength;
            }
            return subNameLength;
        }
        // TODO: @SuppressWarnings("removal")
        internal static File generateFile(String prefix, String suffix, File dir)
            // TODO: throws IOException
        {
            long n = RANDOM.NextInt64();
            string nus = Convert.ToString(n, 10);

            // Use only the file name from the supplied prefix
            prefix = (new File(prefix)).getName();

            int prefixLength = prefix.Length;
            int nusLength = nus.Length;
            int suffixLength = suffix.Length;

            String name;
            int nameMax = FS.getNameMax(dir.getPath());
            int excess = prefixLength + nusLength + suffixLength - nameMax;
            if (excess <= 0) {
                name = prefix + nus + suffix;
            } else {
                // Name exceeds the maximum path component length: shorten it

                // Attempt to shorten the prefix length to no less than 3
                prefixLength = shortenSubName(prefixLength, excess, 3);
                excess = prefixLength + nusLength + suffixLength - nameMax;

                if (excess > 0) {
                    // Attempt to shorten the suffix length to no less than
                    // 0 or 4 depending on whether it begins with a dot ('.')
                    suffixLength = shortenSubName(suffixLength, excess,
                        suffix.IndexOf(".") == 0 ? 4 : 0);
                    suffixLength = shortenSubName(suffixLength, excess, 3);
                    excess = prefixLength + nusLength + suffixLength - nameMax;
                }

                if (excess > 0 && excess <= nusLength - 5) {
                    // Attempt to shorten the random character string length
                    // to no less than 5
                    nusLength = shortenSubName(nusLength, excess, 5);
                }

                StringBuilder sb =
                    new StringBuilder(prefixLength + nusLength + suffixLength);
                sb.Append(prefixLength < prefix.Length ?
                    prefix.Substring(0, prefixLength) : prefix);
                sb.Append(nusLength < nus.Length ?
                    nus.Substring(0, nusLength) : nus);
                sb.Append(suffixLength < suffix.Length ?
                    suffix.Substring(0, suffixLength) : suffix);
                name = sb.ToString();
            }

            // Normalize the path component
            name = FS.normalize(name);

            File f = new File(dir, name);
            if (!name.Equals(f.getName()) || f.isInvalid()) {
                // TODO: 
                //if (System.getSecurityManager() != null)
                //    throw new IOException("Unable to create temporary file");
                //else
                {
                    throw new IOException("Unable to create temporary file, " + name);
                }
            }
            return f;
        }
    }

    /**
     * <p> Creates a new empty file in the specified directory, using the
     * given prefix and suffix strings to generate its name.  If this method
     * returns successfully then it is guaranteed that:
     *
     * <ol>
     * <li> The file denoted by the returned abstract pathname did not exist
     *      before this method was invoked, and
     * <li> Neither this method nor any of its variants will return the same
     *      abstract pathname again in the current invocation of the virtual
     *      machine.
     * </ol>
     *
     * This method provides only part of a temporary-file facility.  To arrange
     * for a file created by this method to be deleted automatically, use the
     * {@link #deleteOnExit} method.
     *
     * <p> The {@code prefix} argument must be at least three characters
     * long.  It is recommended that the prefix be a short, meaningful string
     * such as {@code "hjb"} or {@code "mail"}.  The
     * {@code suffix} argument may be {@code null}, in which case the
     * suffix {@code ".tmp"} will be used.
     *
     * <p> To create the new file, the prefix and the suffix may first be
     * adjusted to fit the limitations of the underlying platform.  If the
     * prefix is too long then it will be truncated, but its first three
     * characters will always be preserved.  If the suffix is too long then it
     * too will be truncated, but if it begins with a period character
     * ({@code '.'}) then the period and the first three characters
     * following it will always be preserved.  Once these adjustments have been
     * made the name of the new file will be generated by concatenating the
     * prefix, five or more internally-generated characters, and the suffix.
     *
     * <p> If the {@code directory} argument is {@code null} then the
     * system-dependent default temporary-file directory will be used.  The
     * default temporary-file directory is specified by the system property
     * {@code java.io.tmpdir}.  On UNIX systems the default value of this
     * property is typically {@code "/tmp"} or {@code "/var/tmp"}; on
     * Microsoft Windows systems it is typically {@code "C:\\WINNT\\TEMP"}.  A different
     * value may be given to this system property when the Java virtual machine
     * is invoked, but programmatic changes to this property are not guaranteed
     * to have any effect upon the temporary directory used by this method.
     *
     * <p> If the {@code directory} argument is not {@code null} and its
     * abstract pathname is valid and denotes an existing, writable directory,
     * then the file will be created in that directory. Otherwise the file will
     * not be created and an {@code IOException} will be thrown.  Under no
     * circumstances will a directory be created at the location specified by
     * the {@code directory} argument.
     *
     * @param  prefix     The prefix string to be used in generating the file's
     *                    name; must be at least three characters long
     *
     * @param  suffix     The suffix string to be used in generating the file's
     *                    name; may be {@code null}, in which case the
     *                    suffix {@code ".tmp"} will be used
     *
     * @param  directory  The directory in which the file is to be created, or
     *                    {@code null} if the default temporary-file
     *                    directory is to be used
     *
     * @return  An abstract pathname denoting a newly-created empty file
     *
     * @throws  IllegalArgumentException
     *          If the {@code prefix} argument contains fewer than three
     *          characters
     *
     * @throws  IOException
     *          If a file could not be created
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method does not allow a file to be created
     *
     * @since 1.2
     */
    public static File createTempFile(String prefix, String suffix,
                                      File directory)
        //throws IOException
    {
        if (prefix.Length < 3) {
            throw new IllegalArgumentException("Prefix string \"" + prefix +
                "\" too short: length must be at least 3");
        }
        if (suffix == null)
            suffix = ".tmp";

        File tmpdir = (directory != null) ? directory
                                          : TempDirectory.location();

        // TODO: @SuppressWarnings("removal")
        // TODO: SecurityManager sm = System.getSecurityManager();
        File f;
        do {
            f = TempDirectory.generateFile(prefix, suffix, tmpdir);
            // TODO: 
            //if (sm != null) {
            //    try {
            //        sm.checkWrite(f.getPath());
            //    } catch (SecurityException se) {
            //        // don't reveal temporary directory location
            //        if (directory == null)
            //            throw new SecurityException("Unable to create temporary file");
            //        throw se;
            //    }
            //}
        } while (FS.hasBooleanAttributes(f, FileSystem.BA_EXISTS));

        if (!FS.createFileExclusively(f.getPath()))
            throw new IOException("Unable to create temporary file");

        return f;
    }

    /**
     * Creates an empty file in the default temporary-file directory, using
     * the given prefix and suffix to generate its name. Invoking this method
     * is equivalent to invoking {@link #createTempFile(java.lang.String,
     * java.lang.String, java.io.File)
     * createTempFile(prefix,&nbsp;suffix,&nbsp;null)}.
     *
     * <p> The {@link
     * java.nio.file.Files#createTempFile(String,String,java.nio.file.attribute.FileAttribute[])
     * Files.createTempFile} method provides an alternative method to create an
     * empty file in the temporary-file directory. Files created by that method
     * may have more restrictive access permissions to files created by this
     * method and so may be more suited to security-sensitive applications.
     *
     * @param  prefix     The prefix string to be used in generating the file's
     *                    name; must be at least three characters long
     *
     * @param  suffix     The suffix string to be used in generating the file's
     *                    name; may be {@code null}, in which case the
     *                    suffix {@code ".tmp"} will be used
     *
     * @return  An abstract pathname denoting a newly-created empty file
     *
     * @throws  IllegalArgumentException
     *          If the {@code prefix} argument contains fewer than three
     *          characters
     *
     * @throws  IOException  If a file could not be created
     *
     * @throws  SecurityException
     *          If a security manager exists and its {@link
     *          java.lang.SecurityManager#checkWrite(java.lang.String)}
     *          method does not allow a file to be created
     *
     * @since 1.2
     * @see java.nio.file.Files#createTempDirectory(String,FileAttribute[])
     */
    public static File createTempFile(String prefix, String suffix)
        //throws IOException
    {
        return createTempFile(prefix, suffix, null);
    }

    /* -- Basic infrastructure -- */

    /**
     * Compares two abstract pathnames lexicographically.  The ordering
     * defined by this method depends upon the underlying system.  On UNIX
     * systems, alphabetic case is significant in comparing pathnames; on
     * Microsoft Windows systems it is not.
     *
     * @param   pathname  The abstract pathname to be compared to this abstract
     *                    pathname
     *
     * @return  Zero if the argument is equal to this abstract pathname, a
     *          value less than zero if this abstract pathname is
     *          lexicographically less than the argument, or a value greater
     *          than zero if this abstract pathname is lexicographically
     *          greater than the argument
     *
     * @since   1.2
     */
    public int CompareTo(File pathname) {
        return FS.compare(this, pathname);
    }

    /**
     * Tests this abstract pathname for equality with the given object.
     * Returns {@code true} if and only if the argument is not
     * {@code null} and is an abstract pathname that is the same as this
     * abstract pathname.  Whether or not two abstract
     * pathnames are equal depends upon the underlying operating system.
     * On UNIX systems, alphabetic case is significant in comparing pathnames;
     * on Microsoft Windows systems it is not.
     *
     * @apiNote This method only tests whether the abstract pathnames are equal;
     *          it does not access the file system and the file is not required
     *          to exist.
     *
     * @param   obj   The object to be compared with this abstract pathname
     *
     * @return  {@code true} if and only if the objects are the same;
     *          {@code false} otherwise
     *
     * @see #compareTo(File)
     * @see java.nio.file.Files#isSameFile(Path,Path)
     */
    public override bool Equals(Object? obj) {
        if (obj is File file) {
            return CompareTo(file) == 0;
        }
        return false;
    }

    /**
     * Computes a hash code for this abstract pathname.  Because equality of
     * abstract pathnames is inherently system-dependent, so is the computation
     * of their hash codes.  On UNIX systems, the hash code of an abstract
     * pathname is equal to the exclusive <em>or</em> of the hash code
     * of its pathname string and the decimal value
     * {@code 1234321}.  On Microsoft Windows systems, the hash
     * code is equal to the exclusive <em>or</em> of the hash code of
     * its pathname string converted to lower case and the decimal
     * value {@code 1234321}.  Locale is not taken into account on
     * lowercasing the pathname string.
     *
     * @return  A hash code for this abstract pathname
     */
    public override int GetHashCode() {
        return FS.hashCode(this);
    }

    /**
     * Returns the pathname string of this abstract pathname.  This is just the
     * string returned by the {@link #getPath} method.
     *
     * @return  The string form of this abstract pathname
     */
    public String toString() {
        return getPath();
    }

    /**
     * WriteObject is called to save this filename.
     * The separator character is saved also so it can be replaced
     * in case the path is reconstituted on a different host type.
     *
     * @serialData  Default fields followed by separator character.
     *
     * @param  s the {@code ObjectOutputStream} to which data is written
     * @throws IOException if an I/O error occurs
     */
    // TODO: @java.io.Serial
    //private void writeObject(ObjectOutputStream s)
    //    // TODO: throws IOException
    //{
    //    lock (this)
    //    {
    //        s.defaultWriteObject();
    //        s.writeChar(separatorChar); // Add the separator character
    //    }
    //}

    /**
     * readObject is called to restore this filename.
     * The original separator character is read.  If it is different
     * from the separator character on this system, then the old separator
     * is replaced by the local separator.
     *
     * @param  s the {@code ObjectInputStream} from which data is read
     * @throws IOException if an I/O error occurs
     * @throws ClassNotFoundException if a serialized class cannot be loaded
     */
    // TODO: @java.io.Serial
    //private void readObject(ObjectInputStream s)
    //    // TODO: throws IOException, ClassNotFoundException
    //{
    //    lock (this)
    //    {
    //        ObjectInputStream.GetField fields = s.readFields();
    //        String pathField = (String)fields.get("path", null);
    //        char sep = s.readChar(); // read the previous separator char
    //        if (sep != separatorChar)
    //            pathField = pathField.Replace(sep, separatorChar);
    //        String path = FS.normalize(pathField);
    //        UNSAFE.putReference(this, PATH_OFFSET, path);
    //        UNSAFE.putIntVolatile(this, PREFIX_LENGTH_OFFSET, FS.prefixLength(path));
    //    }
    //}

    //private static readonly jdk.internal.misc.Unsafe UNSAFE
    //        = jdk.internal.misc.Unsafe.getUnsafe();
    //private static readonly long PATH_OFFSET
    //        = UNSAFE.objectFieldOffset(File.class, "path");
    //private static readonly long PREFIX_LENGTH_OFFSET
    //        = UNSAFE.objectFieldOffset(File.class, "prefixLength");

    /** use serialVersionUID from JDK 1.0.2 for interoperability */
    // TODO: @java.io.Serial
    private static readonly long serialVersionUID = 301077366599181567L;

    // -- Integration with java.nio.file --
    [NonSerialized]
        private volatile Path filePath;

    /**
     * Returns a {@link Path java.nio.file.Path} object constructed from
     * this abstract path. The resulting {@code Path} is associated with the
     * {@link java.nio.file.FileSystems#getDefault default-filesystem}.
     *
     * <p> The first invocation of this method works as if invoking it were
     * equivalent to evaluating the expression:
     * {@snippet lang=java :
     *         // @link regex="getPath(?=\(t)" target="java.nio.file.FileSystem#getPath" :
     *         FileSystems.getDefault().getPath(this.getPath());
     * }
     * Subsequent invocations of this method return the same {@code Path}.
     *
     * <p> If this abstract pathname is the empty abstract pathname then this
     * method returns a {@code Path} that may be used to access the current
     * user directory.
     *
     * @return  a {@code Path} constructed from this abstract path
     *
     * @throws  java.nio.file.InvalidPathException
     *          if a {@code Path} object cannot be constructed from the abstract
     *          path (see {@link java.nio.file.FileSystem#getPath FileSystem.getPath})
     *
     * @since   1.7
     * @see Path#toFile
     */
    public Path toPath()
    {
        Path result = filePath;
        if (result == null)
        {
            lock(this) {
                result = filePath;
                if (result == null)
                {
                    result = DefaultFileSystem.getFileSystem().getPath(path);
                    filePath = result;
                }
            }
        }
        return result;
    }
}
