namespace DSAExperimentation.LeetCode.PowerOfTwo;

// LeetCode 231. Power of Two: is `value` exactly 2^k for some non-negative integer k?
//
// The naive strategy divides out factors of two one at a time and checks what
// survives; the bit trick recognizes that a power of two has exactly one set bit,
// so clearing it with `value & (value - 1)` reduces the whole question to one
// comparison.
internal static class PowerOfTwoSolution
{
    // The textbook approach: repeatedly halve `value` while it is even, then check
    // whether the survivor is 1. Written without this repo's primitives - the
    // arm IsPowerOfTwoByBitTrick below has to justify itself against.
    public static bool IsPowerOfTwoByRepeatedDivision(int value)
    {
        if (value <= 0)
        {
            return false;
        }

        while (value % 2 == 0)
        {
            value /= 2;
        }

        return value == 1;
    }

    // A power of two has exactly one set bit, so `value & (value - 1)` clears it and
    // the result is zero; `value` must still be positive since `value - 1` underflows
    // at zero.
    public static bool IsPowerOfTwoByBitTrick(int value) => value > 0 && (value & (value - 1)) == 0;
}
