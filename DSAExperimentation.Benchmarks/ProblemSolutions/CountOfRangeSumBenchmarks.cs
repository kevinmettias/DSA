using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count of Range Sum (LC 327): the textbook O(n^2) all-pairs prefix-sum scan vs. a
// left-to-right sweep through this repo's own FenwickTree<int,SumOperation<int>> (a
// Binary Indexed Tree of counts), with each prefix sum's compressed rank - and its
// [prefixSum-upper, prefixSum-lower] query bounds - found via
// BinarySearch.LowerBound/UpperBound over the sorted distinct prefix sums, O(n log n)
// overall. Same coordinate-compression-plus-Fenwick-sweep shape
// CountOfSmallerNumbersAfterSelfBenchmarks already uses for LC 315, generalized from
// a single one-sided PrefixQuery to a two-sided Query range.
[MemoryDiagnoser]
public class CountOfRangeSumBenchmarks
{
    private const int Lower = -1_000;
    private const int Upper = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(327);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-100, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwisePrefixScan()
    {
        var prefix = BuildPrefixSums(_nums);
        var count = 0;

        for (var a = 0; a < prefix.Length; a++)
        {
            for (var b = a + 1; b < prefix.Length; b++)
            {
                var rangeSum = prefix[b] - prefix[a];

                if (rangeSum >= Lower && rangeSum <= Upper)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int FenwickTreeSweep()
    {
        var prefix = BuildPrefixSums(_nums);
        var sortedDistinct = prefix.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var prefixSum in prefix)
        {
            var loRank = BinarySearch.LowerBound(sequence, prefixSum - Upper);
            var hiRank = BinarySearch.UpperBound(sequence, prefixSum - Lower) - 1;

            if (loRank <= hiRank)
            {
                count += tree.Query(loRank, hiRank);
            }

            tree.Add(BinarySearch.LowerBound(sequence, prefixSum), 1);
        }

        return count;
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        return prefix;
    }
}
