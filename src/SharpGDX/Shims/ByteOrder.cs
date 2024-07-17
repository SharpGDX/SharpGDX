using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
	/** Defines byte order constants.
 *
 * @since Android 1.0 */
	public sealed class ByteOrder
	{

		/** This constant represents big endian.
		 *
		 * @since Android 1.0 */
		public static readonly ByteOrder BIG_ENDIAN = new ByteOrder("BIG_ENDIAN"); //$NON-NLS-1$

		/** This constant represents little endian.
		 *
		 * @since Android 1.0 */
		public static readonly ByteOrder LITTLE_ENDIAN = new ByteOrder("LITTLE_ENDIAN"); //$NON-NLS-1$

		private static readonly ByteOrder NATIVE_ORDER;

		static ByteOrder (){
			// TODO: Important
// if (Platform.getMemorySystem().isLittleEndian()) {
			//NATIVE_ORDER = LITTLE_ENDIAN;
// } else {
// NATIVE_ORDER = BIG_ENDIAN;
// }
		}

		/** Returns the current platform byte order.
		 *
		 * @return the byte order object, which is either LITTLE_ENDIAN or BIG_ENDIAN.
		 * @since Android 1.0 */
		public static ByteOrder nativeOrder()
		{
			return NATIVE_ORDER;
		}

		private readonly String name;

		private ByteOrder(String name)
		{
			this.name = name;
		}

		/** Returns a string that describes this object.
		 *
		 * @return "BIG_ENDIAN" for {@link #BIG_ENDIAN ByteOrder.BIG_ENDIAN} objects, "LITTLE_ENDIAN" for {@link #LITTLE_ENDIAN
		 *         ByteOrder.LITTLE_ENDIAN} objects.
		 * @since Android 1.0 */
		public override String ToString()
		{
			return name;
		}
	}
}
