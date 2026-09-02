using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sequentially Ordinal Rank Tracker (LC 2102): re-sorting every location seen so
// far on every Get() call (O(n^2 log n) over a full add/get run) vs this repo's
// own Heap<T,TOrder> driving the two-heap "topK size pinned at the query count"
// trick (O(n log n) overall). Add/Get alternate every step, the shape the judge's
// own interleaved calls take.
[MemoryDiagnoser]
public class SequentiallyOrdinalRankTrackerBenchmarks
{
    private const int ScoreExclusiveUpperBound = 1_000_000;

    [Params(100, 1_000)]
    public int OperationCount;

    private string[] _names = null!;
    private int[] _scores = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _names = Enumerable.Range(0, OperationCount).Select(i => $"loc{i}").ToArray();
        _scores = Enumerable.Range(0, OperationCount).Select(_ => random.Next(0, ScoreExclusiveUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string ResortEveryGet()
    {
        var seen = new List<(int Score, string Name)>(OperationCount);
        var lastRank = string.Empty;

        for (var i = 0; i < OperationCount; i++)
        {
            seen.Add((_scores[i], _names[i]));
            seen.Sort((a, b) => a.Score != b.Score ? b.Score.CompareTo(a.Score) : string.CompareOrdinal(a.Name, b.Name));
            lastRank = seen[i].Name;
        }

        return lastRank;
    }

    [Benchmark]
    public string TwoHeapTracker()
    {
        var topK = new Heap<(int, string), MaxHeapOrder<(int, string)>>();
        var backup = new Heap<(int, string), MinHeapOrder<(int, string)>>();
        var lastRank = string.Empty;

        for (var i = 0; i < OperationCount; i++)
        {
            topK.Push((-_scores[i], _names[i]));
            topK.TryPop(out var demoted);
            backup.Push(demoted);

            backup.TryPop(out var promoted);
            topK.Push(promoted);
            topK.TryPeek(out var worstOfTop);
            lastRank = worstOfTop.Item2;
        }

        return lastRank;
    }
}
