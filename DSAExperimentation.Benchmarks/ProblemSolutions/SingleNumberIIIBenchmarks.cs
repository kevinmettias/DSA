using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SingleNumberIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SingleNumberIIISolution's, the same methods
// SingleNumberIIITests proves correct.
[MemoryDiagnoser]
public class SingleNumberIIIBenchmarks
{
    // LC problem number, reused as the deterministic value seed.
    private const int Seed = 260;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = SingleNumberIIIWorkloads.BuildValues(Length, seed: Seed);

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => SingleNumberIIISolution.FindSingleNumbersByBruteForce(_values);

    [Benchmark]
    public int[] HashMapFrequencyCount() => SingleNumberIIISolution.FindSingleNumbersByHashMapFrequencyCount(_values);
}
