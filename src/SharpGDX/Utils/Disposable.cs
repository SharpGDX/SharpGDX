using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils
{
    /// <summary>
    /// Interface for disposable resources.
    /// </summary>
    public interface Disposable
	{
        /// <summary>
        /// Releases all resources of this object.
        /// </summary>
        public void Dispose();
	}
}
