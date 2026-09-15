using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.GoodSubsequenceQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GoodSubsequenceQueriesSolution's, the same methods
// GoodSubsequenceQueriesTests proves correct. Both strategies mutate the state
// [GlobalSetup] would otherwise hand them once (the composed arm's
// GoodSubsequenceIndex point-updates its SegmentTrees on every query; the
// baseline mutates its own working copy of nums), so [IterationSetup] rebuilds
// both fresh before every measured invocation instead of letting later
// iterations replay queries against an already-updated array - the same reason
// SurroundedRegionsBenchmarks rebuilds its board per iteration rather than
// reusing [GlobalSetup]'s.
[MemoryDiagnoser]
public class GoodSubsequenceQueriesBenchmarks
{
    private const int Seed = 3901;
    private const int QueryCount = 500;

    private int[] _nums = [];

    private int[][] _queries = [];
    private int[] _bruteForceNums = [];
    private GoodSubsequenceIndex _index = null!;
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_nums, _queries) = GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, seed: Seed);

    [IterationSetup]
    public void IterationSetup()
    {
        _bruteForceNums = (int[])_nums.Clone();
        _index = GoodSubsequenceIndex.Build(_nums, GoodSubsequenceQueriesScenario.P);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        GoodSubsequenceQueriesSolution.CountGoodSubseqByBruteForce(_bruteForceNums, GoodSubsequenceQueriesScenario.P, _queries);

    [Benchmark]
    public int SegmentTreeGcd() =>
        GoodSubsequenceQueriesSolution.CountGoodSubseqBySegmentTreeGcd(_index, _queries);
}
