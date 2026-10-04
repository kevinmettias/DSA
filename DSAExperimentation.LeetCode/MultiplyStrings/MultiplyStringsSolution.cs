using DecimalStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.MultiplyStrings;

// LeetCode 43. Multiply Strings: multiply two non-negative integers of up to
// 200 decimal digits each, given as strings, without a BigInteger library and
// without converting either operand to an integer - a product of two 200-digit
// operands has up to 400 digits, far past any machine integer.
//
// Both strategies are grade-school multiplication and differ in how they hold
// the running total. The textbook baseline adds every digit pair's product into
// the column it lands in and carries once at the end. The composed strategy is
// built the way AddBinarySolution composes this repo's Stack<char> for base-b
// string arithmetic: multiply num1 by one digit of num2 at a time, shift each
// partial product by that digit's place, then fold it into the running total
// with stack-based decimal-string addition (AddBinary's carry walk, base 10
// instead of base 2).
internal static class MultiplyStringsSolution
{
    private const int DecimalBase = 10;
    private const string ZeroDigitString = "0";

    // The textbook baseline: num1[i] * num2[j] belongs to column i + j + 1 of
    // an (m + n)-column int[], so every digit pair's product is added straight
    // into its column, and one right-to-left carry pass at the end turns the
    // column sums into digits. A column collects at most min(m, n) products of
    // two digits - 200 * 81 at LeetCode's bound - so an int holds it without a
    // carry per product. Deliberately written without this repo's primitives
    // (ARCHITECTURE.md §17.5).
    public static string MultiplyByColumnSums(string num1, string num2)
    {
        var columns = new int[num1.Length + num2.Length];

        for (var i = 0; i < num1.Length; i++)
        {
            for (var j = 0; j < num2.Length; j++)
            {
                columns[i + j + 1] += (num1[i] - '0') * (num2[j] - '0');
            }
        }

        CarryOnce(columns);

        return TrimLeadingZeros(DigitsOf(columns));
    }

    // Leaves one decimal digit per column. A product of an m-digit and an
    // n-digit number has at most m + n digits, so no carry is left over once
    // the leftmost column is reached.
    private static void CarryOnce(int[] columns)
    {
        var carry = 0;

        for (var column = columns.Length - 1; column >= 0; column--)
        {
            var total = columns[column] + carry;
            columns[column] = total % DecimalBase;
            carry = total / DecimalBase;
        }
    }

    // The carried columns read left to right, leading zeros included: the
    // product can have m + n - 1 digits rather than m + n, and is "0" when
    // either operand is.
    private static string DigitsOf(int[] columns)
    {
        var chars = new char[columns.Length];

        for (var column = 0; column < columns.Length; column++)
        {
            chars[column] = (char)('0' + columns[column]);
        }

        return new string(chars);
    }

    // Elementary-school long multiplication: multiply num1 by each digit of
    // num2 in turn, shift that partial product into place, and accumulate
    // with stack-based decimal addition.
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

    private static string AddDecimalStrings(string firstOperand, string secondOperand)
    {
        var stack = new DecimalStack();
        var i = firstOperand.Length - 1;
        var j = secondOperand.Length - 1;
        var carry = 0;

        while (HasDigitsLeftToAdd(i, j, carry))
        {
            var digitA = NextDigit(firstOperand, ref i);
            var digitB = NextDigit(secondOperand, ref j);
            carry = AccumulateDigit(stack, digitA, digitB, carry);
        }

        return TrimLeadingZeros(PopAllIntoString(stack));
    }

    // More to add while either operand still has an unread digit, or a carry is
    // still waiting to be placed.
    private static bool HasDigitsLeftToAdd(int leftIndex, int rightIndex, int carry)
        => leftIndex >= 0 || rightIndex >= 0 || carry > 0;

    private static int NextDigit(string operand, ref int index)
    {
        if (index < 0)
        {
            return 0;
        }

        return operand[index--] - '0';
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
