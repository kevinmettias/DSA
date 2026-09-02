using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge Similar Items (LC 2363): the naive approach re-scans the output list
// linearly for a matching value on every insert (O(n) per item, O(n^2) overall)
// before sorting it; the repo-primitive approach accumulates into a HashMap<int,int>
// (O(1) average per item) and sorts the distinct values with this repo's own
// MergeSort. Values are deliberately all-distinct across both input arrays so the
// naive linear scan never finds an early match and always pays its full-length scan -
// the same "force the real worst case" trick TwoSumBenchmarks' unreachable target uses.
[MemoryDiagnoser]
public class MergeSimilarItemsBenchmarks
{
    [Params(200, 1_000)]
    public int Length;

    private int[][] _items1 = null!;
    private int[][] _items2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // items1 holds even values, items2 holds odd values - no value ever repeats,
        // so neither approach gets an early-exit shortcut from a shared value.
        _items1 = Enumerable.Range(0, Length).Select(i => new[] { i * 2, 1 }).ToArray();
        _items2 = Enumerable.Range(0, Length).Select(i => new[] { i * 2 + 1, 1 }).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var merged = new List<int[]>();
        MergeInto(merged, _items1);
        MergeInto(merged, _items2);
        merged.Sort((a, b) => a[0].CompareTo(b[0]));

        return merged.Count;
    }

    private static void MergeInto(List<int[]> merged, int[][] items)
    {
        foreach (var item in items)
        {
            var found = false;

            foreach (var entry in merged)
            {
                if (entry[0] == item[0])
                {
                    entry[1] += item[1];
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                merged.Add([item[0], item[1]]);
            }
        }
    }

    [Benchmark]
    public int HashMapAndMergeSort()
    {
        var totals = new HashMap<int, int>();
        Accumulate(totals, _items1);
        Accumulate(totals, _items2);

        var values = totals.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(values));

        return values.Length;
    }

    private static void Accumulate(HashMap<int, int> totals, int[][] items)
    {
        foreach (var item in items)
        {
            totals.TryGetValue(item[0], out var weight);
            totals.Set(item[0], weight + item[1]);
        }
    }
}
