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
    [Params(200, 5_000)]
    public int Length;

    private int[] _persons = null!;
    private int[] _times = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(911);
        _persons = Enumerable.Range(0, Length).Select(_ => random.Next(0, 10)).ToArray();
        _times = Enumerable.Range(0, Length).Select(i => i * 2).ToArray();
        _queries = _times.Select(t => t + 1).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerQueryRescan()
    {
        var leaderSum = 0;

        foreach (var t in _queries)
        {
            var votes = new HashMap<int, int>();
            var leader = -1;
            var leaderVotes = 0;

            for (var i = 0; i < _times.Length && _times[i] <= t; i++)
            {
                votes.TryGetValue(_persons[i], out var count);
                count++;
                votes.Set(_persons[i], count);

                if (count >= leaderVotes)
                {
                    leader = _persons[i];
                    leaderVotes = count;
                }
            }

            leaderSum += leader;
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
            votes.TryGetValue(_persons[i], out var count);
            count++;
            votes.Set(_persons[i], count);

            if (count >= leaderVotes)
            {
                leader = _persons[i];
                leaderVotes = count;
            }

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
}
