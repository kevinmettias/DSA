using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignSkiplist.DesignSkiplistSolution;

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

    [Params(200, 5_000)]
    public int OperationCount;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, OperationCount).Select(_ => random.Next(0, MaxValue + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Replay(new SkiplistByLinearScanList());

    [Benchmark]
    public int FenwickTreeFrequencyMultiset() => Replay(new SkiplistByFenwickFrequencies());

    private int Replay(ISkiplist skiplist)
    {
        foreach (var value in _values)
        {
            skiplist.Add(value);
        }

        var trueCount = 0;

        foreach (var value in _values)
        {
            if (skiplist.Search(value))
            {
                trueCount++;
            }
        }

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
