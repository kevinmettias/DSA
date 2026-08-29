using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseInteger;

// LeetCode 7. Reverse Integer: use this repo's LIFO Stack<T> as the explicit
// digit-reversal primitive, with the problem's 32-bit overflow rule checked after
// reconstruction.
public sealed partial class ReverseIntegerTests
{
    [Theory]
    [InlineData(123, 321)]
    [InlineData(-123, -321)]
    [InlineData(120, 21)]
    [InlineData(0, 0)]
    public void Reverse_InRangeInput_ReturnsDigitsInReverseOrder(int value, int expected)
        => Assert.Equal(expected, Reverse(value));

    [Theory]
    [InlineData(1534236469)]
    [InlineData(-1563847412)]
    public void Reverse_ReversedValueOverflows_ReturnsZero(int value)
        => Assert.Equal(0, Reverse(value));

    private static int Reverse(int value)
    {
        var digits = new DigitStack();
        foreach (var digit in Math.Abs((long)value).ToString())
        {
            digits.Push(digit);
        }

        long reversed = 0;
        while (digits.TryPop(out var digit))
        {
            reversed = (reversed * 10) + (digit - '0');
        }

        if (value < 0)
        {
            reversed = -reversed;
        }

        return reversed is < int.MinValue or > int.MaxValue ? 0 : (int)reversed;
    }
}

