using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ArrayPartition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Array Partition (LC 561): repeatedly scanning for the two smallest remaining
// elements (O(n^2), no sort) vs. this repo's own MergeSort over
// ArrayIndexedSequence (O(n log n)) followed by summing every even-indexed
// element - both compute the same maximized sum of pair-minimums, proved by
// ArrayPartitionTests.
[MemoryDiagnoser]
public class ArrayPartitionBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 561;

    // Symmetric bound for the generated values' range: [-ValueBound, ValueBound).
    private const int ValueBound = 10_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedSmallestPairScan() => ArrayPartitionSolution.MaxSumByRepeatedSmallestPairScan(_values);

    [Benchmark]
    public int MergeSortPairSum() => ArrayPartitionSolution.MaxSumByMergeSort(_values);
}
