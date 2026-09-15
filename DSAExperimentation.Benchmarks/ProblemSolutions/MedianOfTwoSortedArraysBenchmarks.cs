using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.MedianOfTwoSortedArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MedianOfTwoSortedArraysSolution's, the same methods
// MedianOfTwoSortedArraysTests proves correct.
[MemoryDiagnoser]
public class MedianOfTwoSortedArraysBenchmarks
{
    private const int RandomSeed = 11;
    private const int MaxRandomValue = 1_000_000;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    [Params(2_000, 40_000)]
    public int TotalLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, TotalLength / AlgorithmConstants.HalvingFactor).Select(_ => random.Next(-MaxRandomValue, MaxRandomValue)).Order().ToArray();
        _nums2 = Enumerable.Range(0, TotalLength - _nums1.Length).Select(_ => random.Next(-MaxRandomValue, MaxRandomValue)).Order().ToArray();
    }

    [Benchmark(Baseline = true)]
    public double MergeAndSort() => MedianOfTwoSortedArraysSolution.FindMedianByMergeAndSort(_nums1, _nums2);

    [Benchmark]
    public double BinarySearchPartition() => MedianOfTwoSortedArraysSolution.FindMedianByBinarySearchPartition(_nums1, _nums2);
}
