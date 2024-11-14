namespace SharpGDX.Utils.Compression.RangeCoder;

public class BitTreeDecoder(int numBitLevels)
{
    private readonly short[] _models = new short[1 << numBitLevels];

    public static int ReverseDecode(short[] models, int startIndex, Decoder rangeDecoder, int numBitLevels)
    {
        var m = 1;
        var symbol = 0;

        for (var bitIndex = 0; bitIndex < numBitLevels; bitIndex++)
        {
            var bit = rangeDecoder.DecodeBit(models, startIndex + m);
            m <<= 1;
            m += bit;
            symbol |= bit << bitIndex;
        }

        return symbol;
    }

    public int Decode(Decoder rangeDecoder)
    {
        var m = 1;

        for (var bitIndex = numBitLevels; bitIndex != 0; bitIndex--)
        {
            m = (m << 1) + rangeDecoder.DecodeBit(_models, m);
        }

        return m - (1 << numBitLevels);
    }

    public void Init()
    {
        Decoder.InitBitModels(_models);
    }

    public int ReverseDecode(Decoder rangeDecoder)
    {
        var m = 1;
        var symbol = 0;

        for (var bitIndex = 0; bitIndex < numBitLevels; bitIndex++)
        {
            var bit = rangeDecoder.DecodeBit(_models, m);
            m <<= 1;
            m += bit;
            symbol |= bit << bitIndex;
        }

        return symbol;
    }
}