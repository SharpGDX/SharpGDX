using System.Text;
using SharpGDX.Shims;

namespace SharpGDX.MP3;

/**
 * 16-Bit CRC checksum
 */
public sealed class Crc16 {
	private static ushort polynomial = (ushort)0x8005;
	private ushort crc;

	/**
	 * Dummy Constructor
	 */
	public Crc16 () {
		crc = (ushort)0xFFFF;
	}

	/**
	 * Feed a bitstring to the crc calculation (0 < length <= 32).
	 */
	public void add_bits (int bitstring, int length) {
		int bitmask = 1 << length - 1;
		do
			if ((crc & 0x8000) == 0 ^ (bitstring & bitmask) == 0) {
				crc <<= 1;
				crc ^= polynomial;
			} else
				crc <<= 1;
		while ((bitmask >>>= 1) != 0);
	}

	/**
	 * Return the calculated checksum. Erase it for next calls to add_bits().
	 */
	public short checksum () {
		ushort sum = crc;
		crc = (ushort)0xFFFF;
		// TODO: Verify that this checksum is correct.
		return (short)sum;
	}
}
