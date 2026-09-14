using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SequentiallyOrdinalRankTracker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SequentiallyOrdinalRankTrackerSolution's, the same
// factories SequentiallyOrdinalRankTrackerTests proves correct. [GlobalSetup]
// builds the location names and their scores, so workload construction is charged
// to setup and only the replay is measured. Add and Get alternate every step, the
// shape the judge's own interleaved calls take - which is also what makes the
// re-sort baseline O(n^2 log n) over a full run against the two-heap tracker's
// O(n log n).
[MemoryDiagnoser]
public class SequentiallyOrdinalRankTrackerBenchmarks
{
    private const int ScoreExclusiveUpperBound = 1_000_000;
    private const int RandomSeed = 1;
    private const string NamePrefix = "loc";

    [Params(100, 1_000)]
    public int OperationCount;

    private string[] _names = null!;
    private int[] _scores = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _names = Enumerable.Range(0, OperationCount).Select(i => NamePrefix + i).ToArray();
        _scores = Enumerable.Range(0, OperationCount).Select(_ => random.Next(0, ScoreExclusiveUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string ResortEveryGet() =>
        Replay(SequentiallyOrdinalRankTrackerSolution.CreateByResortEveryGet());

    [Benchmark]
    public string TwoHeapTracker() =>
        Replay(SequentiallyOrdinalRankTrackerSolution.CreateByTwoHeaps());

    private string Replay(SequentiallyOrdinalRankTrackerSolution.IRankTracker tracker)
    {
        var lastRank = string.Empty;

        for (var i = 0; i < OperationCount; i++)
        {
            tracker.Add(_names[i], _scores[i]);
            lastRank = tracker.Get();
        }

        return lastRank;
    }
}
