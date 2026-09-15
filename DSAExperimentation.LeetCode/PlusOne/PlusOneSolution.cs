using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.PlusOne;

// LeetCode 66. Plus One: add one to the number represented by a big-endian digit
// array, propagating the carry as far left as it needs to go.
//
// The two strategies differ in how they walk and rebuild the digits: an in-place
// array edit that stops the instant a digit absorbs the carry without overflowing,
// or this repo's own Stack<int>, which always walks every digit and lets the
// stack's LIFO order rebuild the array in the original order.
internal static class PlusOneSolution
{
    private const int DecimalBase = 10;
    private const int MaxDigitValue = DecimalBase - 1;

    // Walk from the least-significant digit; the moment one absorbs the carry
    // without overflowing, every digit to its left is unaffected and the walk can
    // stop immediately.
    public static int[] IncrementByArrayWalk(int[] digits)
    {
        var result = (int[])digits.Clone();

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i] < MaxDigitValue)
            {
                result[i]++;
                return result;
            }

            result[i] = 0;
        }

        // Every digit was 9 and rolled over to 0: the number gained a leading 1.
        var withCarry = new int[result.Length + 1];
        withCarry[0] = 1;
        Array.Copy(result, 0, withCarry, 1, result.Length);
        return withCarry;
    }

    // Push the carry-adjusted digits onto Stack<int> least-significant first;
    // popping yields them most-significant first, so no reversal pass is needed.
    public static int[] IncrementByDigitStack(int[] digits)
    {
        var stack = new DigitStack();
        PushCarryAdjustedDigits(digits, stack);

        var result = new List<int>();

        while (stack.TryPop(out var digit))
        {
            result.Add(digit);
        }

        return result.ToArray();
    }

    // Least-significant digit first, adding one and carrying left; whatever carry is
    // still unabsorbed past the most-significant digit is pushed as the new leading
    // digit. Popping the stack therefore yields the number most-significant first.
    private static void PushCarryAdjustedDigits(int[] digits, DigitStack stack)
    {
        var carry = 1;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var sum = digits[i] + carry;
            stack.Push(sum % DecimalBase);
            carry = sum / DecimalBase;
        }

        if (carry > 0)
        {
            stack.Push(carry);
        }
    }
}
