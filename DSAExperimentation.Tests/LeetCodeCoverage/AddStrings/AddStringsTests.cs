using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AddStrings;

// LeetCode 415. Add Strings: base-10 digit-by-digit addition (AddBinaryTests
// precedent, base 2 there), pushing least-significant-first onto this repo's own
// Stack<char> so popping naturally yields the digits most-significant-first.
public sealed partial class AddStringsTests
{
    [Theory]
    [InlineData("11", "123", "134")]
    [InlineData("456", "77", "533")]
    [InlineData("0", "0", "0")]
    public void AddStrings_LeetCodeExamples_ReturnsDecimalSum(string a, string b, string expected)
    {
        var sum = Add(a, b);

        Assert.Equal(expected, sum);
    }

    private static string Add(string a, string b)
    {
        var stack = new DigitStack();
        PushDigitsOntoStack(a, b, stack);

        var digits = new List<char>();
        while (stack.TryPop(out var digit))
        {
            digits.Add(digit);
        }

        return new string(digits.ToArray());
    }

    private static void PushDigitsOntoStack(string a, string b, DigitStack stack)
    {
        var i = a.Length - 1;
        var j = b.Length - 1;
        var carry = 0;

        while (i >= 0 || j >= 0 || carry > 0)
        {
            var sum = carry;
            if (i >= 0)
            {
                sum += a[i--] - '0';
            }

            if (j >= 0)
            {
                sum += b[j--] - '0';
            }

            stack.Push((char)('0' + (sum % 10)));
            carry = sum / 10;
        }
    }
}
