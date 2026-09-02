using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design a Number Container System (LC 2349): both strategies replay the same
// Change-then-Find workload. Each index first gets its own distinct number (a
// bijection over [0, Count)), then ~20% of indices are reassigned to a different
// random number - just enough churn to exercise HeapPerNumberFind's lazy discarding
// of now-stale entries. With numbers this sparse (at most a couple of indices ever
// share one), a matching index is roughly uniformly positioned across the whole
// array, so ArrayScanFind's fresh scan for the smallest matching index in a plain
// int[] (unset = -1) genuinely costs O(n) on average per query - unlike a small
// fixed number domain, where scanning from index 0 tends to hit a match within the
// first few slots regardless of Count and never actually exercises the O(n) case.
// HeapPerNumberFind instead composes this repo's own HashMap<TKey,TValue> (current
// number per index) and a min Heap<int,MinHeapOrder<int>> per number, whose size
// stays O(1) here, so Find resolves in amortized O(1) instead of scanning the array.
[MemoryDiagnoser]
public class DesignANumberContainerSystemBenchmarks
{
    private const int RandomSeed = 2349;
    private const int ChurnDivisor = 5;

    [Params(200, 3_000)]
    public int Count;

    private int[] _changeIndices = null!;
    private int[] _changeNumbers = null!;
    private int[] _findQueries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var churnCount = Count / ChurnDivisor;

        _changeIndices = Enumerable.Range(0, Count)
            .Concat(Enumerable.Range(0, churnCount).Select(_ => random.Next(0, Count)))
            .ToArray();
        _changeNumbers = Enumerable.Range(0, Count)
            .Concat(Enumerable.Range(0, churnCount).Select(_ => random.Next(0, Count)))
            .ToArray();
        _findQueries = Enumerable.Range(0, Count).Select(_ => random.Next(0, Count)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ArrayScanFind()
    {
        var numberByIndex = new int[Count];
        Array.Fill(numberByIndex, -1);

        for (var i = 0; i < _changeIndices.Length; i++)
        {
            numberByIndex[_changeIndices[i]] = _changeNumbers[i];
        }

        long total = 0;

        foreach (var number in _findQueries)
        {
            total += ScanForSmallestIndex(numberByIndex, number);
        }

        return total;
    }

    private static int ScanForSmallestIndex(int[] numberByIndex, int number)
    {
        for (var i = 0; i < numberByIndex.Length; i++)
        {
            if (numberByIndex[i] == number)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public long HeapPerNumberFind()
    {
        var numberByIndex = new HashMap<int, int>();
        var indicesByNumber = new HashMap<int, Heap<int, MinHeapOrder<int>>>();

        for (var i = 0; i < _changeIndices.Length; i++)
        {
            Change(numberByIndex, indicesByNumber, _changeIndices[i], _changeNumbers[i]);
        }

        long total = 0;

        foreach (var number in _findQueries)
        {
            total += Find(numberByIndex, indicesByNumber, number);
        }

        return total;
    }

    private static void Change(
        HashMap<int, int> numberByIndex, HashMap<int, Heap<int, MinHeapOrder<int>>> indicesByNumber, int index, int number)
    {
        numberByIndex.Set(index, number);

        if (!indicesByNumber.TryGetValue(number, out var indices))
        {
            indices = new Heap<int, MinHeapOrder<int>>();
            indicesByNumber.Set(number, indices);
        }

        indices.Push(index);
    }

    private static int Find(
        HashMap<int, int> numberByIndex, HashMap<int, Heap<int, MinHeapOrder<int>>> indicesByNumber, int number)
    {
        if (!indicesByNumber.TryGetValue(number, out var indices))
        {
            return -1;
        }

        while (indices.TryPeek(out var index))
        {
            if (numberByIndex.TryGetValue(index, out var current) && current == number)
            {
                return index;
            }

            indices.TryPop(out _);
        }

        return -1;
    }
}
