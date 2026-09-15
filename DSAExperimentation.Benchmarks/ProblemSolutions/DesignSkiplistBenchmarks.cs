using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignSkiplist;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignSkiplistSolution's, the same classes
// DesignSkiplistTests proves correct. A linear-scan List<int> multiset
// (Contains/IndexOf + RemoveAt, O(n) per call) against this repo's own
// FenwickTree<int,SumOperation<int>> used as a point-update/point-query frequency
// array over num's bounded domain, O(log 2*10^4) per call. Both replay the same
// fixed batch of add-then-search-then-erase calls a real Skiplist caller would make.
[MemoryDiagnoser]
public class DesignSkiplistBenchmarks
{
    private const int MaxValue = 20_000;

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1206;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, OperationCount).Select(_ => random.Next(0, MaxValue + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new DesignSkiplistSolution.SkiplistByLinearScanList());

    [Benchmark]
    public int FenwickTreeFrequencyMultiset() => Replay(new DesignSkiplistSolution.SkiplistByFenwickFrequencies());

    private int Replay(DesignSkiplistSolution.ISkiplist skiplist)
    {
        AddEveryValue(skiplist);

        var searchHits = CountSearchHits(skiplist);
        var eraseHits = CountEraseHits(skiplist);

        return searchHits + eraseHits;
    }

    private void AddEveryValue(DesignSkiplistSolution.ISkiplist skiplist)
    {
        foreach (var value in _values)
        {
            skiplist.Add(value);
        }
    }

    private int CountSearchHits(DesignSkiplistSolution.ISkiplist skiplist)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            if (skiplist.Search(value))
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private int CountEraseHits(DesignSkiplistSolution.ISkiplist skiplist)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            if (skiplist.Erase(value))
            {
                trueCount++;
            }
        }

        return trueCount;
    }
}
