namespace DSAExperimentation.LeetCode.NumberOf1Bits;

// LeetCode 191. Number of 1 Bits: count the set bits in a 32-bit unsigned integer.
//
// Two ways to count: clear the lowest set bit each step, which runs exactly
// popcount(n) times, or shift the operand right and test the low bit, which
// always runs all 32 positions. Which wins depends on how dense the operand is.
internal static class NumberOf1BitsSolution
{
    private const int BitWidth = 32;

    // Shifts right and tests the low bit one position at a time: always 32
    // iterations, against the bit-clear arm's popcount(n). On a dense operand the
    // two nearly tie; on a sparse one this pays for all 32 positions anyway, in
    // exchange for a loop whose iteration count never depends on the data.
    public static int CountByShiftAndMask(uint value)
    {
        var count = 0;

        for (var bit = 0; bit < BitWidth; bit++)
        {
            count += (int)(value & 1);
            value >>= 1;
        }

        return count;
    }

    // Brian Kernighan's trick: n & (n - 1) clears the lowest set bit, so the loop
    // runs exactly popcount(n) times instead of inspecting all 32 bit positions.
    public static int CountByBitClear(uint value)
    {
        var count = 0;

        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }
}
