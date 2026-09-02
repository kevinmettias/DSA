namespace DSAExperimentation.LeetCode.NumberOf1Bits;

// LeetCode 191. Number of 1 Bits: count the set bits in a 32-bit unsigned integer.
//
// Brian Kernighan's trick: n & (n - 1) clears the lowest set bit, so the loop
// runs exactly popcount(n) times instead of inspecting all 32 bit positions.
internal static class NumberOf1BitsSolution
{
    public static int CountByBitClear(uint n)
    {
        var count = 0;

        while (n != 0)
        {
            n &= n - 1;
            count++;
        }

        return count;
    }
}
