using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.MaximumStrongPairXORII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumStrongPairXORIISolution's, the same
// methods MaximumStrongPairXORIITests proves correct. Length and value range
// match LC 2935's own bound (nums.Length <= 5*10^4, nums[i] < 2^20), where the
// O(n^2) pairwise scan stops being competitive. BitTrieBuckets is handed the
// pre-sorted ArrayIndexedSequence<int> its hoisted overload takes, so sorting
// is charged to [GlobalSetup] rather than to the bucket sweep being measured.
[MemoryDiagnoser]
public class MaximumStrongPairXORIIBenchmarks
{
    private const int RandomSeed = 2935;
    private const int MaxValueExclusive = 1 << 20;

    private int[] _nums = [];

    private ArrayIndexedSequence<int> _sortedNums;
    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();

        _sortedNums = new ArrayIndexedSequence<int>((int[])_nums.Clone());
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(_sortedNums);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumStrongPairXORIISolution.MaximumStrongPairXorByBruteForce(_nums);

    [Benchmark]
    public int BitTrieBuckets() => MaximumStrongPairXORIISolution.MaximumStrongPairXorByBitTrieBuckets(_sortedNums);
}
