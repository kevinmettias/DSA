using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ContainsDuplicate;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainsDuplicateSolution's, exercised on an array
// with no duplicate at all so neither strategy gets to stop early - the worst case
// for a "does any value repeat" query.
[MemoryDiagnoser]
public class ContainsDuplicateBenchmarks
{
    private const int Seed = 217;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = ContainsDuplicateWorkloads.BuildDistinctValues(Length, seed: Seed);

    [Benchmark(Baseline = true)]
    public bool HasDuplicateByBruteForce() => ContainsDuplicateSolution.HasDuplicateByBruteForce(_nums);

    [Benchmark]
    public bool HasDuplicateBySetProbe() => ContainsDuplicateSolution.HasDuplicateBySetProbe(_nums);
}
