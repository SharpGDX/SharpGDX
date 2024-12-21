using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * The JavaLayerException is the base class for all API-level exceptions thrown by JavaLayer. To facilitate conversion and common
 * handling of exceptions from other domains, the class can delegate some functionality to a contained Throwable instance.
 * <p>
 *
 * @author MDM
 */
public class JavaLayerException : Exception
{

    private Exception? exception;

    private PrintStream errorStream = new PrintStream();

    public JavaLayerException()
    {
    }

    public JavaLayerException(String msg)
        : base(msg)
    {

    }

    public JavaLayerException(String msg, Exception t)
        : base(msg)
    {

        exception = t;
    }

    public Exception getException()
    {
        return exception;
    }

    public void printStackTrace()
    {
        // TODO: Originally passed System.err, which from my understanding prints to a file? -RP
        printStackTrace(errorStream);
    }

    public void printStackTrace(PrintStream ps)
    {
        if (exception == null)
        {
            // TODO: base.printStackTrace(ps);
            Console.WriteLine(ps.ToString());
        }
        else
            Console.WriteLine(exception.ToString());
    }

}
