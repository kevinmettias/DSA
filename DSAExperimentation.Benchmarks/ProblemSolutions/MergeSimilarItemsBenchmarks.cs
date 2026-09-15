using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MergeSimilarItems;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeSimilarItemsSolution's, the same methods
// MergeSimilarItemsTests proves correct. The naive arm re-scans its output list
// for a matching value on every insert (O(n) per item, O(n^2) overall) before
// sorting; the composed arm accumulates into this repo's HashMap<int, int> and
// sorts the distinct values with this repo's MergeSort. Values are deliberately
// all-distinct across both input arrays so the naive scan never finds an early
// match and always pays its full-length walk - the same "force the real worst
// case" trick TwoSumBenchmarks' unreachable target uses.
//
// Both arms now return LeetCode's actual answer rather than just its length, the
// same deliberate change ARCHITECTURE.md #17.8 records for WordLadderII: they are
// the asserted methods, so materializing the merged list is part of what is
// measured. The result is a list of distinct values, linear in Length, so this
// does not change which arm the comparison is about.
[MemoryDiagnoser]
public class MergeSimilarItemsBenchmarks
{
    private int[][] _items1 = [];

    private int[][] _items2 = [];
    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var items = MergeSimilarItemsWorkloads.BuildDisjointValues(Length);
        _items1 = items.Items1;
        _items2 = items.Items2;
    }

    [Benchmark(Baseline = true)]
    public List<(int Value, int Weight)> LinearScan() =>
        MergeSimilarItemsSolution.MergeByLinearScan(_items1, _items2);

    [Benchmark]
    public List<(int Value, int Weight)> HashMapAndMergeSort() =>
        MergeSimilarItemsSolution.MergeByHashMapAndMergeSort(_items1, _items2);
}
