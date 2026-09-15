using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DecodeString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DecodeStringSolution's, the same methods
// DecodeStringTests proves correct.
[MemoryDiagnoser]
public class DecodeStringBenchmarks
{
    private string _encoded = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _encoded = DecodeStringWorkloads.BuildEncoded(Length);

    [Benchmark(Baseline = true)]
    public string RecursiveDescent() => DecodeStringSolution.DecodeByRecursiveDescent(_encoded);

    [Benchmark]
    public string StackScan() => DecodeStringSolution.DecodeByStackScan(_encoded);
}
