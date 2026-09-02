using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveKDigits;

// LeetCode 402. Remove K Digits: the classic monotonic-stack greedy - the
// LargestRectangleInHistogram/RemoveDuplicateLetters precedent, applied to picking the
// smallest possible remaining number - over this repo's own Stack<char>. Each new digit
// pops every still-removable, strictly-greater digit off the top before being pushed
// itself; any removal budget left once the scan ends comes off the (already
// non-decreasing) tail.
public sealed partial class RemoveKDigitsTests
{
    [Theory]
    [InlineData("1432219", 3, "1219")]
    [InlineData("10200", 1, "200")]
    [InlineData("10", 2, "0")]
    [InlineData("112", 1, "11")]
    public void RemoveKDigits_ClassicExamples_ReturnsSmallestPossibleNumber(string num, int k, string expected)
    {
        var actual = RemoveKDigits(num, k);
        Assert.Equal(expected, actual);
    }

    private static string RemoveKDigits(string num, int k)
    {
        var stack = new DigitStack();
        k = PopGreaterDigits(stack, num, k);
        PopRemainingBudget(stack, k);

        var digits = PopAllIntoArray(stack);

        return TrimLeadingZeros(digits);
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

    private static string TrimLeadingZeros(char[] digits)
    {
        var trimmed = new string(digits).TrimStart('0');
        return trimmed.Length == 0 ? "0" : trimmed;
    }
}
