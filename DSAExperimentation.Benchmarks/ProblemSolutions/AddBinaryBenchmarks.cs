using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AddBinary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddBinarySolution's. All-ones operands force a carry
// out of every digit position, so the result always grows by one and neither
// strategy gets to stop early.
[MemoryDiagnoser]
public class AddBinaryBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _a = null!;
    private string _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        _a = new string('1', Length);
        _b = new string('1', Length);
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse() => AddBinarySolution.AddByCharArrayReverse(_a, _b);

    [Benchmark]
    public string StackBits() => AddBinarySolution.AddByBitStack(_a, _b);
}
