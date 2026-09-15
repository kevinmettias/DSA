using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.MaximumStrongPairXORI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumStrongPairXORISolution's, the same methods
// MaximumStrongPairXORITests proves correct. Length and value range match LC
// 2932's own bound (nums.Length <= 50, nums[i] <= 100), where the pairwise scan
// is already fast - see MaximumStrongPairXORIIBenchmarks for the same
// comparison at the scale that actually favors the trie strategy. BitTrieBuckets
// is handed the pre-sorted ArrayIndexedSequence<int> its hoisted overload takes,
// so sorting is charged to [GlobalSetup] rather than to the sweep being
// measured.
[MemoryDiagnoser]
public class MaximumStrongPairXORIBenchmarks
{
    private const int RandomSeed = 2932;
    private const int MaxValueExclusive = 101;

    private int[] _nums = [];

    private ArrayIndexedSequence<int> _sortedNums;
    [Params(10, 50)]
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
    public int BruteForce() => MaximumStrongPairXORISolution.MaximumStrongPairXorByBruteForce(_nums);

    [Benchmark]
    public int BitTrieBuckets() => MaximumStrongPairXORISolution.MaximumStrongPairXorByBitTrieBuckets(_sortedNums);
}
