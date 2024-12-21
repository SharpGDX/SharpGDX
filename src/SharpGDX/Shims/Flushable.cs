using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    public interface Flushable
    {

        /**
         * Flushes this stream by writing any buffered output to the underlying
         * stream.
         *
         * @throws IOException If an I/O error occurs
         */
        void flush();
    }
}
