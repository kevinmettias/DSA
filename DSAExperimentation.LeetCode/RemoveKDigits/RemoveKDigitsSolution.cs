using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.RemoveKDigits;

// LeetCode 402. Remove K Digits: remove k digits from a non-negative integer string so
// the digits left behind, in their original order, form the smallest possible number.
internal static class RemoveKDigitsSolution
{
    private const string ZeroResult = "0";

    // The naive round-by-round baseline: k separate O(n) scans, each finding and
    // removing the string's first strictly-descending digit. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy below has
    // to justify itself against. Removing the first descent one digit at a time is
    // the textbook equivalent of the monotonic-stack greedy, so both strategies
    // produce the same final digit sequence.
    public static string RemoveByRepeatedFirstDescentRemoval(string num, int k)
    {
        var current = num;

        for (var round = 0; round < k; round++)
        {
            var removeIndex = FindFirstDescentOrLast(current);
            current = current.Remove(removeIndex, 1);
        }

        return TrimLeadingZeros(current);
    }

    private static int FindFirstDescentOrLast(string current)
    {
        for (var i = 0; i < current.Length - 1; i++)
        {
            if (current[i] > current[i + 1])
            {
                return i;
            }
        }

        return current.Length - 1;
    }

    // The classic monotonic-stack greedy over this repo's own Stack<char> - the
    // LargestRectangleInHistogram/RemoveDuplicateLetters precedent, applied to
    // picking the smallest possible remaining number. Each new digit pops every
    // still-removable, strictly-greater digit off the top before being pushed
    // itself; any removal budget left once the scan ends comes off the (already
    // non-decreasing) tail.
    public static string RemoveByMonotonicStackSweep(string num, int k)
    {
        var stack = new DigitStack();
        var remaining = PopGreaterDigits(stack, num, k);
        PopRemainingBudget(stack, remaining);

        var digits = PopAllIntoArray(stack);

        return TrimLeadingZeros(new string(digits));
    }

    private static int PopGreaterDigits(DigitStack stack, string num, int k)
    {
        foreach (var digit in num)
        {
            while (k > 0 && stack.TryPeek(out var top) && top > digit)
            {
                stack.TryPop(out _);
                k--;
            }

            stack.Push(digit);
        }

        return k;
    }

    private static void PopRemainingBudget(DigitStack stack, int k)
    {
        while (k > 0 && stack.TryPop(out _))
        {
            k--;
        }
    }

    private static char[] PopAllIntoArray(DigitStack stack)
    {
        var digits = new char[stack.Count];
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out digits[i]);
        }

        return digits;
    }

    private static string TrimLeadingZeros(string value)
    {
        var trimmed = value.TrimStart('0');
        return trimmed.Length == 0 ? ZeroResult : trimmed;
    }
}
