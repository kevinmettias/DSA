using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MissingNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MissingNumberSolution's, the same methods
// MissingNumberTests proves correct.
[MemoryDiagnoser]
public class MissingNumberBenchmarks
{
    // LC problem number, reused as the deterministic value seed.
    private const int Seed = 268;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = MissingNumberWorkloads.BuildValues(Length, seed: Seed);

    [Benchmark(Baseline = true)]
    public int BruteForce() => MissingNumberSolution.FindMissingNumberByBruteForce(_values);

    [Benchmark]
    public int SetMembership() => MissingNumberSolution.FindMissingNumberBySetMembership(_values);
}
