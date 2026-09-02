using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Skiplist (LC 1206): a linear-scan List<int> multiset (Contains/IndexOf +
// RemoveAt, O(n) per call) vs. this repo's own FenwickTree<int,SumOperation<int>>
// used as a point-update/point-query frequency array over num's bounded domain,
// O(log 2*10^4) per call. Both run the same fixed batch of add-then-search-then-erase
// calls a real Skiplist caller would make.
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
    public int LinearScanList()
    {
        var list = new List<int>();
        AddAllList(list);
        var trueCount = CountContainsList(list);
        trueCount += RemoveFoundList(list);
        return trueCount;
    }

    [Benchmark]
    public int FenwickTreeFrequencyMultiset()
    {
        var frequencies = new FenwickTree<int, SumOperation<int>>(MaxValue + 1);
        AddAllFenwick(frequencies);
        var trueCount = CountContainsFenwick(frequencies);
        trueCount += RemoveFoundFenwick(frequencies);
        return trueCount;
    }

    private void AddAllList(List<int> list)
    {
        foreach (var value in _values)
        {
            list.Add(value);
        }
    }

    private int CountContainsList(List<int> list)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            if (list.Contains(value))
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private int RemoveFoundList(List<int> list)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            var index = list.IndexOf(value);
            if (index >= 0)
            {
                list.RemoveAt(index);
                trueCount++;
            }
        }

        return trueCount;
    }

    private void AddAllFenwick(FenwickTree<int, SumOperation<int>> frequencies)
    {
        foreach (var value in _values)
        {
            frequencies.Add(value, 1);
        }
    }

    private int CountContainsFenwick(FenwickTree<int, SumOperation<int>> frequencies)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            if (frequencies.Query(value, value) > 0)
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private int RemoveFoundFenwick(FenwickTree<int, SumOperation<int>> frequencies)
    {
        var trueCount = 0;

        foreach (var value in _values)
        {
            if (frequencies.Query(value, value) > 0)
            {
                frequencies.Add(value, -1);
                trueCount++;
            }
        }

        return trueCount;
    }
}
