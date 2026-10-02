namespace DSAExperimentation.LeetCode.ReverseBits;

// LeetCode 190. Reverse Bits: reverse the 32 bits of an unsigned integer.
//
// Two strategies: walk all 32 positions shifting and folding one bit at a time,
// or slice the word into four bytes and reverse each through a 256-entry lookup
// table. The table trades 256 bytes of static state for a fixed four-lookup body
// that does no per-bit looping.
internal static class ReverseBitsSolution
{
    private const int BitWidth = 32;
    private const int BitsPerByte = 8;
    private const int ByteCount = 256;
    private const uint ByteMask = 0xFF;

    private static readonly byte[] ReversedByteTable = BuildReversedByteTable();

    // Reverse each of the four bytes through the lookup table and place them back
    // most-significant first. Four table reads and three shifts, with no loop,
    // against the bit-shift arm's 32 iterations - at the cost of the 256-byte
    // static table and the byte-slicing arithmetic.
    public static uint ReverseByByteLookup(uint value) =>
        ((uint)ReversedByteTable[(int)(value & ByteMask)] << 24)
        | ((uint)ReversedByteTable[(int)((value >> 8) & ByteMask)] << 16)
        | ((uint)ReversedByteTable[(int)((value >> 16) & ByteMask)] << 8)
        | ReversedByteTable[(int)((value >> 24) & ByteMask)];

    // Walk all 32 positions once: shift the accumulator left and fold in the
    // input's current lowest bit, then shift the input right to retire that bit.
    // The accumulator fills in from its low end at the same rate the input
    // empties from its low end, so after 32 iterations the two ends have traded
    // places.
    public static uint ReverseByBitShift(uint value)
    {
        var reversed = 0u;

        for (var i = 0; i < BitWidth; i++)
        {
            reversed = (reversed << 1) | (value & 1);
            value >>= 1;
        }

        return reversed;
    }

    // Reverses one byte's bits once so each of the 256 possible bytes can be
    // answered by a single table read during the scan above.
    private static byte[] BuildReversedByteTable()
    {
        var table = new byte[ByteCount];

        for (var i = 0; i < ByteCount; i++)
        {
            var source = i;

            for (var bit = 0; bit < BitsPerByte; bit++)
            {
                table[i] = (byte)((table[i] << 1) | (source & 1));
                source >>= 1;
            }
        }

        return table;
    }
}
