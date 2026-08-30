using BenchmarkDotNet.Attributes;
using DecimalStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Multiply Strings (LC 43): the shortcut most people reach for first -
// parsing both operands into a machine integer and multiplying directly -
// vs. this repo's Stack<char>-based digit-by-digit multiply/add. Only the
// latter stays correct once the product would overflow a long, which is
// exactly why LeetCode itself bans the built-in-integer shortcut for this
// problem. [Params] is kept small enough (5, 9 digits) that LongConversion
// never throws, so the two benchmarks stay a fair head-to-head instead of
// one of them faulting.
[MemoryDiagnoser]
public class MultiplyStringsBenchmarks
{
    [Params(5, 9)]
    public int Digits;

    private string _num1 = null!;
    private string _num2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _num1 = GenerateDigits(random, Digits);
        _num2 = GenerateDigits(random, Digits);
    }

    [Benchmark(Baseline = true)]
    public string LongConversion() => (long.Parse(_num1) * long.Parse(_num2)).ToString();

    [Benchmark]
    public string StackDigitByDigit() => Multiply(_num1, _num2);

    private static string GenerateDigits(Random random, int digits)
    {
        var chars = new char[digits];
        chars[0] = (char)('1' + random.Next(9));

        for (var i = 1; i < digits; i++)
        {
            chars[i] = (char)('0' + random.Next(10));
        }

        return new string(chars);
    }

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
