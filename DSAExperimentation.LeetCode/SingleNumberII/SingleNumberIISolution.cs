namespace DSAExperimentation.LeetCode.SingleNumberII;

// LeetCode 137. Single Number II: every element in the array appears exactly
// three times except for one, which appears exactly once - find it in linear
// time using only constant extra space.
//
// Two strategies: count each bit position modulo three, or carry that per-bit
// count on the fly in two bitmasks (the ones/twos state machine). Both are O(n)
// and constant space; the first scans the array once per bit, the second once in
// total.
internal static class SingleNumberIISolution
{
    private const int BitWidth = 32;
    private const int TripleModulus = 3;

    // The ones/twos state machine: two bitmasks hold, for every position, whether
    // that bit has been seen an odd or a doubly-even number of times; a bit
    // reaching three is cleared from both, so each triple cancels in place. One
    // pass over the array against the mod-three sweep's 32, with no per-bit
    // counting - at the cost of a state machine far harder to read than a nested
    // count.
    public static int FindSingleByTwoBitCounters(int[] nums)
    {
        var ones = 0;
        var twos = 0;

        foreach (var n in nums)
        {
            twos |= ones & n;
            ones ^= n;

            var both = ones & twos;
            ones &= ~both;
            twos &= ~both;
        }

        return ones;
    }

    // Every triple contributes a multiple of three to the population count of each
    // bit position; whatever is left over after taking each position's count modulo
    // three is exactly the unique element's bit pattern.
    public static int FindSingleByBitCountModThree(int[] nums)
    {
        var result = 0;

        for (var bit = 0; bit < BitWidth; bit++)
        {
            var count = nums.Count(n => ((n >> bit) & 1) == 1);

            if (count % TripleModulus != 0)
            {
                result |= 1 << bit;
            }
        }

        return result;
    }
}
