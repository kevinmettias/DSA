namespace DSAExperimentation.LeetCode.PowerOfTwo;

// LeetCode 231. Power of Two: is n exactly 2^k for some non-negative integer k?
//
// The naive strategy divides out factors of two one at a time and checks what
// survives; the bit trick recognizes that a power of two has exactly one set bit,
// so clearing it with n & (n - 1) reduces the whole question to one comparison.
internal static class PowerOfTwoSolution
{
    // The textbook approach: repeatedly halve n while it is even, then check
    // whether the survivor is 1. Written without this repo's primitives - the
    // arm IsPowerOfTwoByBitTrick below has to justify itself against.
    public static bool IsPowerOfTwoByRepeatedDivision(int n)
    {
        if (n <= 0)
        {
            return false;
        }

        while (n % 2 == 0)
        {
            n /= 2;
        }

        return n == 1;
    }

    // A power of two has exactly one set bit, so n & (n - 1) clears it and the
    // result is zero; n must still be positive since (n - 1) underflows for n = 0.
    public static bool IsPowerOfTwoByBitTrick(int n) => n > 0 && (n & (n - 1)) == 0;
}
