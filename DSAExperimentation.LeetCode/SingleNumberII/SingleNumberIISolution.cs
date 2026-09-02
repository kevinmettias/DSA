namespace DSAExperimentation.LeetCode.SingleNumberII;

// LeetCode 137. Single Number II: every element in the array appears exactly
// three times except for one, which appears exactly once - find it in linear
// time using only constant extra space.
//
// Every triple contributes a multiple of three to the population count of each
// bit position; whatever is left over after taking each position's count modulo
// three is exactly the unique element's bit pattern.
internal static class SingleNumberIISolution
{
    private const int BitWidth = 32;
    private const int TripleModulus = 3;

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
