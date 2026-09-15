using DecimalStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MultiplyStrings;

// LeetCode 43. Multiply Strings: multiply two arbitrarily large non-negative
// integers given as decimal strings, without converting either operand to a
// built-in numeric type - LeetCode itself bans that shortcut once the
// product would overflow a 64-bit integer.
//
// The two strategies are the shortcut most people reach for first anyway
// (parse both operands as a machine integer, multiply, convert back), which
// stays correct only while the product fits in a long; and elementary-school
// digit-by-digit multiplication built the same way AddBinarySolution composes
// this repo's Stack<char> for base-b string arithmetic - multiply num1 by one
// digit of num2 at a time, shift each partial product by that digit's place,
// then fold it into the running total with stack-based decimal-string
// addition (AddBinary's carry walk, base 10 instead of base 2).
internal static class MultiplyStringsSolution
{
    private const int DecimalBase = 10;
    private const string ZeroDigitString = "0";

    // The shortcut most people reach for first: parse both operands as a
    // machine integer and multiply directly. Deliberately written without
    // this repo's primitives - it is the arm the digit-by-digit strategy
    // below has to justify itself against, and it is also the one LeetCode's
    // own constraints rule out for large inputs.
    public static string MultiplyByLongConversion(string num1, string num2) =>
        (long.Parse(num1) * long.Parse(num2)).ToString();

    // Elementary-school long multiplication: multiply num1 by each digit of
    // num2 in turn, shift that partial product into place, and accumulate
    // with stack-based decimal addition. Stays correct at any length, unlike
    // the long-conversion baseline above.
    public static string MultiplyByDigitStack(string num1, string num2)
    {
        var result = ZeroDigitString;

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
            return ZeroDigitString;
        }

        var stack = new DecimalStack();
        var carry = 0;

        for (var i = num.Length - 1; i >= 0; i--)
        {
            var product = (num[i] - '0') * digit + carry;
            stack.Push((char)('0' + product % DecimalBase));
            carry = product / DecimalBase;
        }

        while (carry > 0)
        {
            stack.Push((char)('0' + carry % DecimalBase));
            carry /= DecimalBase;
        }

        return PopAllIntoString(stack);
    }

    private static string AddDecimalStrings(string a, string b)
    {
        var stack = new DecimalStack();
        var i = a.Length - 1;
        var j = b.Length - 1;
        var carry = 0;

        while (StillHasDigitsToAdd(i, j, carry))
        {
            var digitA = NextDigit(a, ref i);
            var digitB = NextDigit(b, ref j);
            carry = AccumulateDigit(stack, digitA, digitB, carry);
        }

        return TrimLeadingZeros(PopAllIntoString(stack));
    }

    // More to add while either operand still has an unread digit, or a carry is
    // still waiting to be placed.
    private static bool StillHasDigitsToAdd(int i, int j, int carry)
        => i >= 0 || j >= 0 || carry > 0;

    private static int NextDigit(string s, ref int index)
    {
        if (index < 0)
        {
            return 0;
        }

        return s[index--] - '0';
    }

    private static int AccumulateDigit(DecimalStack stack, int digitA, int digitB, int carry)
    {
        var sum = carry + digitA + digitB;
        stack.Push((char)('0' + sum % DecimalBase));
        return sum / DecimalBase;
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

    private static string PopAllIntoString(DecimalStack stack)
    {
        var chars = new List<char>();
        while (stack.TryPop(out var digit))
        {
            chars.Add(digit);
        }

        return new string(chars.ToArray());
    }
}
