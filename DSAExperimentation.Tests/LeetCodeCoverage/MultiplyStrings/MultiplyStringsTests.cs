using DecimalStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MultiplyStrings;

// LeetCode 43. Multiply Strings: elementary-school digit-by-digit
// multiplication built the same way AddBinaryTests already composes this
// repo's Stack<char> for base-b string arithmetic (least-significant digit
// first, reversed back to normal order via the stack). Multiply num1 by one
// digit of num2 at a time, shift each partial product by that digit's place,
// then fold it into the running total with the same stack-based decimal-
// string addition AddBinary uses for base 2 - two compositions of the same
// primitive, not a new one.
public sealed partial class MultiplyStringsTests
{
    [Theory]
    [InlineData("2", "3", "6")]
    [InlineData("123", "456", "56088")]
    [InlineData("0", "12345", "0")]
    public void Multiply_LeetCodeExamples_ReturnsDecimalProduct(string num1, string num2, string expected) =>
        Assert.Equal(expected, Multiply(num1, num2));

    private static string Multiply(string num1, string num2)
    {
        var result = "0";

        for (var i = num2.Length - 1; i >= 0; i--)
        {
            var partial = MultiplyBySingleDigit(num1, num2[i] - '0');
            var shifted = partial + new string('0', num2.Length - 1 - i);
            result = AddDecimalStrings(result, shifted);
        }

        return result;
    }

    private static string MultiplyBySingleDigit(string num, int digit)
    {
        if (digit == 0)
        {
            return "0";
        }

        var stack = new DecimalStack();
        var carry = 0;

        for (var i = num.Length - 1; i >= 0; i--)
        {
            var product = (num[i] - '0') * digit + carry;
            stack.Push((char)('0' + product % 10));
            carry = product / 10;
        }

        while (carry > 0)
        {
            stack.Push((char)('0' + carry % 10));
            carry /= 10;
        }

        return PopAllIntoString(stack);
    }

    private static string AddDecimalStrings(string a, string b)
    {
        var stack = new DecimalStack();
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

            stack.Push((char)('0' + sum % 10));
            carry = sum / 10;
        }

        return TrimLeadingZeros(PopAllIntoString(stack));
    }

    private static string PopAllIntoString(DecimalStack stack)
    {
        var chars = new List<char>();
        while (stack.TryPop(out var digit))
        {
            chars.Add(digit);
        }

        return new string(chars.ToArray());
    }

    private static string TrimLeadingZeros(string digits)
    {
        var start = 0;
        while (start < digits.Length - 1 && digits[start] == '0')
        {
            start++;
        }

        return digits[start..];
    }
}
