using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.ReverseInteger;

// LeetCode 7. Reverse Integer: reverse the base-10 digits of a signed 32-bit
// integer, reporting 0 if the reversed value would overflow int's range.
//
// The two strategies differ only in how the digits are reversed - direct
// arithmetic via mod/div, or this repo's own Stack<char> used as an explicit
// LIFO digit-reversal primitive - and agree afterward on the same overflow check.
internal static class ReverseIntegerSolution
{
    private const int DecimalBase = 10;

    // Baseline: peel digits off with mod/div and fold them into the reversed
    // value directly, entirely BCL arithmetic.
    public static int ReverseByArithmetic(int value)
    {
        var remaining = Math.Abs((long)value);
        long reversed = 0;

        while (remaining > 0)
        {
            reversed = (reversed * DecimalBase) + (remaining % DecimalBase);
            remaining /= DecimalBase;
        }

        if (value < 0)
        {
            reversed = -reversed;
        }

        return ToIntOrZero(reversed);
    }

    // Push the decimal digits least-significant first onto this repo's
    // Stack<char>; popping then yields them most-significant first.
    public static int ReverseByDigitStack(int value)
    {
        var digits = new DigitStack();

        foreach (var digit in Math.Abs((long)value).ToString())
        {
            digits.Push(digit);
        }

        long reversed = 0;

        while (digits.TryPop(out var digit))
        {
            reversed = (reversed * DecimalBase) + (digit - '0');
        }

        if (value < 0)
        {
            reversed = -reversed;
        }

        return ToIntOrZero(reversed);
    }

    private static int ToIntOrZero(long reversed) =>
        reversed is < int.MinValue or > int.MaxValue ? 0 : (int)reversed;
}
