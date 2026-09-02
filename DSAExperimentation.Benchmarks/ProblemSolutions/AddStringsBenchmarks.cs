using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Add Strings (LC 415): building the reversed digit buffer with a plain List<char>
// then reversing it, vs. this repo's own Stack<char> (AddBinaryBenchmarks
// precedent, base 2 there) - pushing least-significant-first so popping naturally
// yields the digits most-significant-first with no separate reverse pass.
[MemoryDiagnoser]
public class AddStringsBenchmarks
{
    // The LeetCode problem number, reused as the deterministic random seed.
    private const int RandomSeed = 415;

    private const int DecimalBase = 10;

    [Params(200, 5_000)]
    public int Length;

    private string _a = null!;
    private string _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _a = string.Concat(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DecimalBase))));
        _b = string.Concat(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DecimalBase))));
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse()
    {
        var digits = new List<char>();
        var i = _a.Length - 1;
        var j = _b.Length - 1;
        var carry = 0;

        while (i >= 0 || j >= 0 || carry > 0)
        {
            var digit = NextDigit(ref i, ref j, ref carry);
            digits.Add(digit);
        }

        digits.Reverse();
        return new string(digits.ToArray());
    }

    [Benchmark]
    public string StackDigits()
    {
        var stack = new DigitStack();
        var i = _a.Length - 1;
        var j = _b.Length - 1;
        var carry = 0;

        while (i >= 0 || j >= 0 || carry > 0)
        {
            var digit = NextDigit(ref i, ref j, ref carry);
            stack.Push(digit);
        }

        var digits = new List<char>();
        while (stack.TryPop(out var digit))
        {
            digits.Add(digit);
        }

        return new string(digits.ToArray());
    }

    // Consumes at most one trailing digit from each of _a/_b (advancing i/j),
    // folds it into carry, and returns the resulting base-10 digit character.
    private char NextDigit(ref int i, ref int j, ref int carry)
    {
        var sum = carry;
        if (i >= 0)
        {
            sum += _a[i--] - '0';
        }

        if (j >= 0)
        {
            sum += _b[j--] - '0';
        }

        carry = sum / DecimalBase;
        return (char)('0' + (sum % DecimalBase));
    }
}
