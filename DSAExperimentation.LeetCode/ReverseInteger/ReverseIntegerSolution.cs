using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.ReverseInteger;

// LeetCode 7. Reverse Integer: reverse the base-10 digits of a signed 32-bit
// integer, reporting 0 if the reversed value would overflow int's range.
//
// The statement also says to assume the environment cannot store 64-bit integers,
// so neither strategy builds the answer in a long and range-checks it afterwards:
// every value lives in an int, and CanAppendDigit refuses each multiply-and-add
// before it runs. A negative input is reversed in the negative range rather than
// through its absolute value, because int.MinValue has no positive int counterpart.
//
// The two strategies differ only in how the digits are reversed - direct
// arithmetic via mod/div, or this repo's own Stack<char> used as an explicit
// LIFO digit-reversal primitive - and agree on the same overflow guard.
internal static class ReverseIntegerSolution
{
    private const int DecimalBase = 10;

    // The largest and smallest values an int can still be multiplied by ten from
    // without overflowing; at exactly these, the appended digit decides.
    private const int PositiveHeadroom = int.MaxValue / DecimalBase;
    private const int NegativeHeadroom = int.MinValue / DecimalBase;
    private const int LargestFinalPositiveDigit = int.MaxValue % DecimalBase;
    private const int SmallestFinalNegativeDigit = int.MinValue % DecimalBase;

    // Baseline: peel digits off with mod/div and fold them into the reversed
    // value directly, entirely BCL arithmetic. C#'s % keeps the dividend's sign,
    // so a negative value yields negative digits and the reversal stays negative
    // without ever negating anything.
    public static int ReverseByArithmetic(int value)
    {
        var remaining = value;
        var reversed = 0;

        while (remaining != 0)
        {
            var digit = remaining % DecimalBase;

            if (!CanAppendDigit(reversed, digit))
            {
                return 0;
            }

            reversed = (reversed * DecimalBase) + digit;
            remaining /= DecimalBase;
        }

        return reversed;
    }

    // Push the decimal digits most-significant first onto this repo's
    // Stack<char>; popping then yields them least-significant first, which is the
    // reversed order to fold back in. The sign is not a digit and is not pushed;
    // it is reapplied to each popped digit instead.
    public static int ReverseByDigitStack(int value)
    {
        var digits = PushDigits(value);
        var sign = Math.Sign(value);
        var reversed = 0;

        while (digits.TryPop(out var symbol))
        {
            var digit = sign * (symbol - '0');

            if (!CanAppendDigit(reversed, digit))
            {
                return 0;
            }

            reversed = (reversed * DecimalBase) + digit;
        }

        return reversed;
    }

    // The value's decimal digits, the most significant pushed first, its sign left out.
    private static DigitStack PushDigits(int value)
    {
        var digits = new DigitStack();

        foreach (var symbol in value.ToString())
        {
            if (char.IsAsciiDigit(symbol))
            {
                digits.Push(symbol);
            }
        }

        return digits;
    }

    // Whether reversed * 10 + digit stays inside int's range, decided before the
    // multiply-and-add so nothing wider than an int ever holds the value: beyond
    // the headroom the multiply alone overflows, and at exactly the headroom the
    // digit must not pass int's own last digit (7 upward, -8 downward).
    private static bool CanAppendDigit(int reversed, int digit) =>
        reversed is >= NegativeHeadroom and <= PositiveHeadroom
        && (reversed != PositiveHeadroom || digit <= LargestFinalPositiveDigit)
        && (reversed != NegativeHeadroom || digit >= SmallestFinalNegativeDigit);
}
