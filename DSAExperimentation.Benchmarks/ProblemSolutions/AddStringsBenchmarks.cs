using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AddStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddStringsSolution's, the same methods
// AddStringsTests proves correct.
[MemoryDiagnoser]
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
        _firstOperand = string.Concat(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DecimalBase))));
        _secondOperand = string.Concat(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DecimalBase))));
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse() => AddStringsSolution.AddByCharArrayReverse(_firstOperand, _secondOperand);

    [Benchmark]
    public string StackDigits() => AddStringsSolution.AddByBitStack(_firstOperand, _secondOperand);
}
