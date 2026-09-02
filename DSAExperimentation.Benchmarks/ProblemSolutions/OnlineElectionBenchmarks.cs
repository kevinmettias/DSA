using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Online Election (LC 911): PerQueryRescan answers every query by re-counting votes
// from scratch up to that query's own time - O(n) per query, O(n^2) overall since
// query times only ever grow. PrecomputedBinarySearch instead builds a parallel
// "leader at index i" array once via this repo's own HashMap<int,int> for running
// vote counts, then answers each query with this repo's own BinarySearch.UpperBound
// over an ArraySequence<int> of vote times - O(n) preprocessing plus O(log n) per
// query. _queries sits just after each vote's own timestamp, so PerQueryRescan's
// scan for query i always covers the full [0, i] prefix rather than short-circuiting
// early.
[MemoryDiagnoser]
public class OnlineElectionBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 911;
    private const int CandidateCount = 10;
    private const int TimeStep = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _persons = null!;
    private int[] _times = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _persons = Enumerable.Range(0, Length).Select(_ => random.Next(0, CandidateCount)).ToArray();
        _times = Enumerable.Range(0, Length).Select(i => i * TimeStep).ToArray();
        _queries = _times.Select(t => t + 1).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerQueryRescan()
    {
        var leaderSum = 0;

        foreach (var t in _queries)
        {
            leaderSum += LeaderAtOrBefore(t);
        }

        return leaderSum;
    }

    [Benchmark]
    public int PrecomputedBinarySearch()
    {
        var leaders = new int[_persons.Length];
        var votes = new HashMap<int, int>();
        var leader = -1;
        var leaderVotes = 0;

        for (var i = 0; i < _persons.Length; i++)
        {
            TallyVote(votes, _persons[i], ref leader, ref leaderVotes);
            leaders[i] = leader;
        }

        var sequence = new ArraySequence<int>(_times);
        var leaderSum = 0;

        foreach (var t in _queries)
        {
            var index = BinarySearch.UpperBound(sequence, t) - 1;
            leaderSum += leaders[index];
        }

        return leaderSum;
    }

    // Re-counts votes from scratch over [0, queryTime] and returns the resulting leader.
    private int LeaderAtOrBefore(int queryTime)
    {
        var votes = new HashMap<int, int>();
        var leader = -1;
        var leaderVotes = 0;

        for (var i = 0; i < _times.Length && _times[i] <= queryTime; i++)
        {
            TallyVote(votes, _persons[i], ref leader, ref leaderVotes);
        }

        return leader;
    }

    // Records one more vote for `person` and updates the running leader if it changes.
    private static void TallyVote(HashMap<int, int> votes, int person, ref int leader, ref int leaderVotes)
    {
        votes.TryGetValue(person, out var count);
        count++;
        votes.Set(person, count);

        if (count >= leaderVotes)
        {
            leader = person;
            leaderVotes = count;
        }
    }
}
