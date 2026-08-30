using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDigitOne;

// LeetCode 233. Number of Digit One: this repo's own LIFO Stack<int> (same
// digit-extraction primitive ReverseIntegerTests uses) peels n's decimal digits
// off most-significant-first, then a running place-value tally (contribution of
// each position depends only on the digits already seen, the digit itself, and
// the remainder still to come) counts every '1' across 1..n in O(log n) instead
// of materializing and scanning every number.
public sealed class NumberOfDigitOneTests
{
    [Theory]
    [InlineData(13, 6)]
    [InlineData(0, 0)]
    [InlineData(100, 21)]
    public void CountDigitOne_LeetCodeExamples_ReturnsOccurrencesOfDigitOne(int n, long expected)
        => Assert.Equal(expected, CountDigitOne(n));

    private static long CountDigitOne(int n)
    {
        if (n <= 0)
        {
            return 0;
        }

        var digits = new DigitStack();
        for (var remaining = n; remaining > 0; remaining /= 10)
        {
            digits.Push(remaining % 10);
        }

        var placeValue = 1L;
        for (var i = 1; i < CountDigits(n); i++)
        {
            placeValue *= 10;
        }

        long count = 0;
        long higherDigits = 0;

        while (digits.TryPop(out var digit))
        {
            var lowerRemainder = n % placeValue;
            count += digit switch
            {
                0 => higherDigits * placeValue,
                1 => (higherDigits * placeValue) + lowerRemainder + 1,
                _ => (higherDigits + 1) * placeValue,
            };

            higherDigits = (higherDigits * 10) + digit;
            placeValue /= 10;
        }

        return count;
    }

    private static int CountDigits(int n)
    {
        var count = 0;
        for (var remaining = n; remaining > 0; remaining /= 10)
        {
            count++;
        }

        return count;
    }
}
