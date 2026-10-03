using DSAExperimentation.LeetCode.AddStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddStringsSolution's, the same methods
// AddStringsSolutionTests proves correct.
public class AddStringsBenchmarks
{
    // The LeetCode problem number, reused as the deterministic random seed.
    private const int RandomSeed = 415;

    private const int DecimalBase = 10;

    private string _firstOperand = "";

    private string _secondOperand = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _firstOperand = DrawOperand(random, Length);
        _secondOperand = DrawOperand(random, Length);
    }

    // LC 415's operands carry no leading zero, so the first digit is drawn from 1-9 and every
    // later one from 0-9.
    private static string DrawOperand(Random random, int length)
    {
        var digits = new char[length];
        digits[0] = (char)('1' + random.Next(DecimalBase - 1));
        for (var i = 1; i < length; i++)
        {
            digits[i] = (char)('0' + random.Next(DecimalBase));
        }

        return new string(digits);
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse() => AddStringsSolution.AddByCharArrayReverse(_firstOperand, _secondOperand);

    [Benchmark]
    public string StackDigits() => AddStringsSolution.AddByBitStack(_firstOperand, _secondOperand);
}
