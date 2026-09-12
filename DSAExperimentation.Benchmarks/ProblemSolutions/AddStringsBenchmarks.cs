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
    public string CharArrayReverse() => AddStringsSolution.AddByCharArrayReverse(_a, _b);

    [Benchmark]
    public string StackDigits() => AddStringsSolution.AddByBitStack(_a, _b);
}
