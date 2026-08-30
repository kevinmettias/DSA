using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Relative Sort Array (LC 1122): LinearScanComparerSort re-derives each element's
// rank by scanning arr2 (Array.IndexOf) on every comparison Array.Sort makes -
// O(n log n * m). HashMapMergeSort instead precomputes every rank once into this
// repo's own HashMap<int,int> - O(n + m) - then sorts by that O(1) lookup via
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence, the same
// custom-comparer shape TwoCitySchedulingBenchmarks already uses. _arr1 is half
// values drawn from _arr2 (exercises the ranked branch) and half values guaranteed
// outside _arr2's range (exercises the unranked, sort-by-value-ascending branch and
// forces every LinearScanComparerSort miss through a full m-length scan). With
// ReferenceLength (m) fixed at 2,000, a BenchmarkDotNet --job Dry run shows the
// expected crossover: LinearScanComparerSort ahead 2.63x at Length=200 (n log n * m
// hasn't yet outgrown Array.Sort's much lower per-comparison constant factor), then
// HashMapMergeSort ahead ~2.5x at Length=5,000, as the O(n log n * m) scan cost
// overtakes HashMap's O(1)-lookup advantage - the same small-n-overhead-then-
// crossover shape several other composed-vs-naive benchmarks in this repo show.
[MemoryDiagnoser]
public class RelativeSortArrayBenchmarks
{
    private const int ReferenceLength = 2_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr1 = null!;
    private int[] _arr2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1122);
        _arr2 = Enumerable.Range(0, ReferenceLength).Select(i => i * 2).ToArray();
        _arr1 = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, 2) == 0
                ? _arr2[random.Next(_arr2.Length)]
                : random.Next(100_000, 200_000))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] LinearScanComparerSort()
    {
        var arr1 = (int[])_arr1.Clone();

        Array.Sort(arr1, (a, b) =>
        {
            var aRank = Array.IndexOf(_arr2, a);
            var bRank = Array.IndexOf(_arr2, b);
            var aKey = aRank >= 0 ? aRank : int.MaxValue;
            var bKey = bRank >= 0 ? bRank : int.MaxValue;
            return aKey != bKey ? aKey.CompareTo(bKey) : a.CompareTo(b);
        });

        return arr1;
    }

    [Benchmark]
    public int[] HashMapMergeSort()
    {
        var arr1 = (int[])_arr1.Clone();
        var rank = new HashMap<int, int>();
        for (var i = 0; i < _arr2.Length; i++)
        {
            rank.Set(_arr2[i], i);
        }

        var byRelativeOrder = Comparer<int>.Create((a, b) =>
        {
            var aKey = rank.TryGetValue(a, out var aIndex) ? aIndex : int.MaxValue;
            var bKey = rank.TryGetValue(b, out var bIndex) ? bIndex : int.MaxValue;
            return aKey != bKey ? aKey.CompareTo(bKey) : a.CompareTo(b);
        });

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(arr1), byRelativeOrder);
        return arr1;
    }
}
