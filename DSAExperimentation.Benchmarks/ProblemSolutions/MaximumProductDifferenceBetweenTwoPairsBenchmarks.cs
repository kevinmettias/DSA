using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Product Difference Between Two Pairs (LC 1913): an O(n^2) brute-force
// scan of every pair's product (valid here because every nums[i] is positive per
// LC's own constraint, so the array-wide max-product pair and min-product pair are
// always index-disjoint once n >= 4, with no explicit four-way-distinctness check
// needed) vs. this repo's own MergeSort over ArrayIndexedSequence (O(n log n)),
// reading the answer straight off the two ends of the sorted array - the same
// composition ArrayPartitionBenchmarks already uses for LC 561.
[MemoryDiagnoser]
public class MaximumProductDifferenceBetweenTwoPairsBenchmarks
{
    private const int RandomSeed = 1913;
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 10_000;
    private const int SecondFromEndOffset = 2; // index offset for the second-largest element from the sorted array's end

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairScan()
    {
        var maxProduct = int.MinValue;
        var minProduct = int.MaxValue;

        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                var product = _values[i] * _values[j];
                maxProduct = Math.Max(maxProduct, product);
                minProduct = Math.Min(minProduct, product);
            }
        }

        return maxProduct - minProduct;
    }

    [Benchmark]
    public int MergeSortExtremes()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var n = sorted.Length;
        return (sorted[n - 1] * sorted[n - SecondFromEndOffset]) - (sorted[0] * sorted[1]);
    }
}
