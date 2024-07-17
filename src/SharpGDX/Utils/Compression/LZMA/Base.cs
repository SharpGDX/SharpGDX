using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Utils.Compression.LZMA
{
	public class Base {
	public static readonly int kNumRepDistances = 4;
	public static readonly int kNumStates = 12;

	public static  int StateInit () {
		return 0;
	}

	public static  int StateUpdateChar (int index) {
		if (index < 4) return 0;
		if (index < 10) return index - 3;
		return index - 6;
	}

	public static  int StateUpdateMatch (int index) {
		return (index < 7 ? 7 : 10);
	}

	public static  int StateUpdateRep (int index) {
		return (index < 7 ? 8 : 11);
	}

	public static  int StateUpdateShortRep (int index) {
		return (index < 7 ? 9 : 11);
	}

	public static  bool StateIsCharState (int index) {
		return index < 7;
	}

	public static readonly int kNumPosSlotBits = 6;
	public static readonly int kDicLogSizeMin = 0;
		// public static readonly int kDicLogSizeMax = 28;
		// public static readonly int kDistTableSizeMax = kDicLogSizeMax * 2;

		public static readonly int kNumLenToPosStatesBits = 2; // it's for speed optimization
	public static readonly int kNumLenToPosStates = 1 << kNumLenToPosStatesBits;

	public static readonly int kMatchMinLen = 2;

	public static int GetLenToPosState (int len) {
		len -= kMatchMinLen;
		if (len < kNumLenToPosStates) return len;
		return (int)(kNumLenToPosStates - 1);
	}

	public static readonly int kNumAlignBits = 4;
	public static readonly int kAlignTableSize = 1 << kNumAlignBits;
	public static readonly int kAlignMask = (kAlignTableSize - 1);

	public static readonly int kStartPosModelIndex = 4;
	public static readonly int kEndPosModelIndex = 14;
	public static readonly int kNumPosModels = kEndPosModelIndex - kStartPosModelIndex;

	public static readonly int kNumFullDistances = 1 << (kEndPosModelIndex / 2);

	public static readonly int kNumLitPosStatesBitsEncodingMax = 4;
	public static readonly int kNumLitContextBitsMax = 8;

	public static readonly int kNumPosStatesBitsMax = 4;
	public static readonly int kNumPosStatesMax = (1 << kNumPosStatesBitsMax);
	public static readonly int kNumPosStatesBitsEncodingMax = 4;
	public static readonly int kNumPosStatesEncodingMax = (1 << kNumPosStatesBitsEncodingMax);

	public static readonly int kNumLowLenBits = 3;
	public static readonly int kNumMidLenBits = 3;
	public static readonly int kNumHighLenBits = 8;
	public static readonly int kNumLowLenSymbols = 1 << kNumLowLenBits;
	public static readonly int kNumMidLenSymbols = 1 << kNumMidLenBits;
	public static readonly int kNumLenSymbols = kNumLowLenSymbols + kNumMidLenSymbols + (1 << kNumHighLenBits);
	public static readonly int kMatchMaxLen = kMatchMinLen + kNumLenSymbols - 1;
}
}
