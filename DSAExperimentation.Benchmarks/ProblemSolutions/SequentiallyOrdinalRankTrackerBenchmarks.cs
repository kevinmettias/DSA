using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SequentiallyOrdinalRankTracker;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SequentiallyOrdinalRankTrackerSolution's, the same
// factories SequentiallyOrdinalRankTrackerSolutionTests proves correct. [GlobalSetup]
// builds the location names and their scores, so workload construction is charged
// to setup and only the replay is measured. Add and Get alternate every step, the
// shape the judge's own interleaved calls take - which is also what makes the
// re-sort baseline O(n^2 log n) over a full run against the two-heap tracker's
// O(n log n). Each name is "loc" plus LetterNames' name for its step, so it stays
// inside LC 2102's lowercase-letters-only names of at most 10 letters, and scores
// are drawn from its [1, 10^5].
public class SequentiallyOrdinalRankTrackerBenchmarks
{
    private const int MinScore = 1;
    private const int MaxScore = 100_000;
    private const int RandomSeed = 1;
    private const string NamePrefix = "loc";

    private string[] _names = [];

    private int[] _scores = [];

    // What every Get returned, in call order - what each arm returns; an Add returns nothing.
    private string[] _ranked = [];
    [Params(100, 1_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _names = Enumerable.Range(0, OperationCount).Select(i => NamePrefix + LetterNames.Of(i)).ToArray();
        _scores = SeededDraws.Values(OperationCount, MinScore, MaxScore + 1, random);
        _ranked = new string[OperationCount];
    }

    [Benchmark(Baseline = true)]
    public string[] ResortEveryGet() =>
        Replay(SequentiallyOrdinalRankTrackerSolution.CreateByResortEveryGet());

    [Benchmark]
    public string[] TwoHeapTracker() =>
        Replay(SequentiallyOrdinalRankTrackerSolution.CreateByTwoHeaps());

    private string[] Replay(SequentiallyOrdinalRankTrackerSolution.IRankTracker tracker)
    {
        for (var i = 0; i < OperationCount; i++)
        {
            tracker.Add(_names[i], _scores[i]);
            _ranked[i] = tracker.Get();
        }

        return _ranked;
    }
}
