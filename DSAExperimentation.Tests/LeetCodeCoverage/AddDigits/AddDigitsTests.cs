using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddDigits;

// LeetCode 258. Add Digits: repeatedly push a number's digits onto this repo's own
// LIFO Stack<T>, popping them all back off to sum, until only one digit remains -
// the same "explicit digit-extraction primitive" shape ReverseIntegerTests.cs and
// PlusOneTests.cs already use their DigitStack for.
public sealed partial class AddDigitsTests
{
    [Theory]
    [InlineData(38, 2)]
    [InlineData(0, 0)]
    [InlineData(9, 9)]
    [InlineData(9999, 9)]
    public void AddDigits_LeetCodeExamples_ReturnsDigitalRoot(int num, int expected)
        => Assert.Equal(expected, AddDigits(num));

    private static int AddDigits(int num)
    {
        while (num >= 10)
        {
            var digits = new DigitStack();
            var remaining = num;
            while (remaining > 0)
            {
                digits.Push(remaining % 10);
                remaining /= 10;
            }

            num = 0;
            while (digits.TryPop(out var digit))
            {
                num += digit;
            }
        }

        return num;
    }
}
