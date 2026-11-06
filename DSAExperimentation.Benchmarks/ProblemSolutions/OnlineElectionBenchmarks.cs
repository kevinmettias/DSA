using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Solutions.OnlineElection;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OnlineElectionSolution's, the same factories
// OnlineElectionSolutionTests proves correct, each object built from the vote log
// inside the arm and asked q once per query, in order, its leaders returned. _queries sits just after each vote's own
// timestamp, so the rescanning arm's tally for query i always covers the full
// [0, i] prefix rather than short-circuiting early - O(n) per query against the
// precomputed arm's O(log n).
public class OnlineElectionBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 911;
    private const int CandidateCount = 10;
    private const int TimeStep = 2;

    private int[] _persons = [];

    private int[] _times = [];
    private int[] _queries = [];

    // Every query's leader; sized in setup so the replay allocates nothing.
    private int[] _leaders = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _persons = SeededDraws.Values(Length, 0, CandidateCount, random);
        _times = Enumerable.Range(0, Length).Select(i => i * TimeStep).ToArray();
        _queries = _times.Select(t => t + 1).ToArray();
        _leaders = new int[Length];
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryRescan()
    {
        var candidate = OnlineElectionSolution.CreateByPerQueryRescan(_persons, _times);
        return QueryEach(candidate);
    }

    [Benchmark]
    public int[] PrecomputedBinarySearch()
    {
        var candidate = OnlineElectionSolution.CreateByPrecomputedBinarySearch(_persons, _times);
        return QueryEach(candidate);
    }

    private int[] QueryEach(OnlineElectionSolution.ITopVotedCandidate candidate)
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            _leaders[i] = candidate.Q(_queries[i]);
        }

        return _leaders;
    }
}
