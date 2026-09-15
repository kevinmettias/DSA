using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MultiplyStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MultiplyStringsSolution's, the same methods
// MultiplyStringsTests proves correct - the shortcut most people reach for
// first (parse both operands as a machine integer and multiply directly) vs.
// this repo's Stack<char>-based digit-by-digit multiply/add. Only the latter
// stays correct once the product would overflow a long, which is exactly why
// LeetCode itself bans the built-in-integer shortcut for this problem.
// [Params] is kept small enough (5, 9 digits) that LongConversion never
// throws, so the two benchmarks stay a fair head-to-head instead of one of
// them faulting.
[MemoryDiagnoser]
public class MultiplyStringsBenchmarks
{
    private const int DecimalBase = 10;
    private const int LeadingDigitRange = 9; private string _num1 = "";

    private string _num2 = "";
    // random.Next(9) => 1..9, avoiding a leading zero

    [Params(5, 9)]
    public int Digits { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _num1 = GenerateDigits(random, Digits);
        _num2 = GenerateDigits(random, Digits);
    }

    private static string GenerateDigits(Random random, int digits)
    {
        var chars = new char[digits];
        chars[0] = (char)('1' + random.Next(LeadingDigitRange));

        for (var i = 1; i < digits; i++)
        {
            chars[i] = (char)('0' + random.Next(DecimalBase));
        }

        return new string(chars);
    }

    [Benchmark(Baseline = true)]
    public string LongConversion() => MultiplyStringsSolution.MultiplyByLongConversion(_num1, _num2);

    [Benchmark]
    public string StackDigitByDigit() => MultiplyStringsSolution.MultiplyByDigitStack(_num1, _num2);
}
