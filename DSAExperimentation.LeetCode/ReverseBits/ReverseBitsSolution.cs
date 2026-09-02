namespace DSAExperimentation.LeetCode.ReverseBits;

// LeetCode 190. Reverse Bits: reverse the 32 bits of an unsigned integer.
//
// Walk all 32 positions once: shift the accumulator left and fold in the
// input's current lowest bit, then shift the input right to retire that bit.
// The accumulator fills in from its low end at the same rate the input
// empties from its low end, so after 32 iterations the two ends have traded
// places.
internal static class ReverseBitsSolution
{
    private const int BitWidth = 32;

    public static uint ReverseByBitShift(uint n)
    {
        var reversed = 0u;

        for (var i = 0; i < BitWidth; i++)
        {
            reversed = (reversed << 1) | (n & 1);
            n >>= 1;
        }

        return reversed;
    }
}
