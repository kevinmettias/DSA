using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OnlineElection;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OnlineElectionSolution's, the same methods
// OnlineElectionTests proves correct. _queries sits just after each vote's own
// timestamp, so the rescanning arm's tally for query i always covers the full
// [0, i] prefix rather than short-circuiting early - O(n) per query against the
// precomputed arm's O(log n).
[MemoryDiagnoser]
public class OnlineElectionBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 911;
    private const int CandidateCount = 10;
    private const int TimeStep = 2;

    private int[] _persons = [];

    private int[] _times = [];
    private int[] _queries = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _persons = Enumerable.Range(0, Length).Select(_ => random.Next(0, CandidateCount)).ToArray();
        _times = Enumerable.Range(0, Length).Select(i => i * TimeStep).ToArray();
        _queries = _times.Select(t => t + 1).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryRescan() =>
        OnlineElectionSolution.LeadersByPerQueryRescan(_persons, _times, _queries);

    [Benchmark]
    public int[] PrecomputedBinarySearch() =>
        OnlineElectionSolution.LeadersByPrecomputedBinarySearch(_persons, _times, _queries);
}
