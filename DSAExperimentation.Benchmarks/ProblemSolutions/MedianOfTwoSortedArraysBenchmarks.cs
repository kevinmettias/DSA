using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Median of Two Sorted Arrays (LC 4): the O((m+n) log(m+n)) textbook fallback -
// concatenate both arrays and sort from scratch, ignoring that each half already
// arrived sorted - vs. this repo's own generic BinarySearch.LowerBound bisecting
// the smaller array for the correct partition in O(log(min(m,n))), the same
// approach MedianOfTwoSortedArraysTests uses.
[MemoryDiagnoser]
public class MedianOfTwoSortedArraysBenchmarks
{
    [Params(2_000, 40_000)]
    public int TotalLength;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(11);
        _nums1 = Enumerable.Range(0, TotalLength / 2).Select(_ => random.Next(-1_000_000, 1_000_000)).Order().ToArray();
        _nums2 = Enumerable.Range(0, TotalLength - _nums1.Length).Select(_ => random.Next(-1_000_000, 1_000_000)).Order().ToArray();
    }

    [Benchmark(Baseline = true)]
    public double MergeAndSort()
    {
        var merged = _nums1.Concat(_nums2).OrderBy(value => value).ToArray();
        var mid = merged.Length / 2;
        return merged.Length % 2 == 1 ? merged[mid] : (merged[mid - 1] + merged[mid]) / 2.0;
    }

    [Benchmark]
    public double BinarySearchPartition() => FindMedianSortedArrays(_nums1, _nums2);

    private static double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        if (nums1.Length > nums2.Length)
        {
            return FindMedianSortedArrays(nums2, nums1);
        }

        var m = nums1.Length;
        var n = nums2.Length;
        var half = (m + n + 1) / 2;

        var feasibility = new PartitionFeasibilitySequence(nums1, nums2, half);
        var i = BinarySearch.LowerBound<int, PartitionFeasibilitySequence>(feasibility, 1) - 1;
        var j = half - i;

        var leftOfPartition = Math.Max(
            i == 0 ? int.MinValue : nums1[i - 1],
            j == 0 ? int.MinValue : nums2[j - 1]);

        if ((m + n) % 2 == 1)
        {
            return leftOfPartition;
        }

        var rightOfPartition = Math.Min(
            i == m ? int.MaxValue : nums1[i],
            j == n ? int.MaxValue : nums2[j]);

        return (leftOfPartition + rightOfPartition) / 2.0;
    }

    // See MedianOfTwoSortedArraysTests.PartitionFeasibilitySequence for the full
    // explanation - repeated here rather than shared because TwoSumBenchmarks/
    // KthLargestBenchmarks establish this project keeps its own copy of the
    // solution rather than depending on the Tests project.
    private readonly struct PartitionFeasibilitySequence(int[] nums1, int[] nums2, int half) : IRandomAccessSequence<int>
    {
        public int Length => nums1.Length + 1;

        public int Get(int i)
        {
            var j = half - i;
            var leftOfNums1 = i == 0 ? int.MinValue : nums1[i - 1];
            var rightOfNums2 = j == nums2.Length ? int.MaxValue : nums2[j];
            return leftOfNums1 <= rightOfNums2 ? 0 : 1;
        }
    }
}
