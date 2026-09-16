using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.RemoveKDigits;

// LeetCode 402. Remove K Digits: drop removalCount of the digits from a non-negative
// integer string so the digits left behind, in their original order, form the smallest
// possible number.
internal static class RemoveKDigitsSolution
{
    private const string ZeroResult = "0";

    // The naive round-by-round baseline: removalCount separate O(n) scans, each finding
    // and removing the string's first strictly-descending digit. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy below has
    // to justify itself against. Removing the first descent one digit at a time is
    // the textbook equivalent of the monotonic-stack greedy, so both strategies
    // produce the same final digit sequence.
    public static string RemoveByRepeatedFirstDescentRemoval(string num, int removalCount)
    {
        var current = num;

        for (var round = 0; round < removalCount; round++)
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
    public static string RemoveByMonotonicStackSweep(string num, int removalCount)
    {
        var stack = new DigitStack();
        var remaining = PopGreaterDigits(stack, num, removalCount);
        PopRemainingBudget(stack, remaining);

        return ReadSurvivingNumber(stack);
    }

    private static int PopGreaterDigits(DigitStack stack, string num, int removalCount)
    {
        foreach (var digit in num)
        {
            while (ShouldDropStackTop(stack, digit, removalCount))
            {
                stack.TryPop(out _);
                removalCount--;
            }

            stack.Push(digit);
        }

        return removalCount;
    }

    // The digit on top is dropped while removals are still available and it is strictly
    // greater than the one arriving - giving it up leaves the smaller number behind.
    private static bool ShouldDropStackTop(DigitStack stack, char digit, int removalCount) =>
        removalCount > 0 && stack.TryPeek(out var top) && top > digit;

    private static void PopRemainingBudget(DigitStack stack, int removalCount)
    {
        while (removalCount > 0 && stack.TryPop(out _))
        {
            removalCount--;
        }
    }

    // The digits still on the stack, read back top-first so they come out in the order
    // they were pushed, spelled as the number left once the leading zeros a shortened
    // result can expose are dropped.
    private static string ReadSurvivingNumber(DigitStack stack)
    {
        var digits = PopAllIntoArray(stack);

        return TrimLeadingZeros(new string(digits));
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
