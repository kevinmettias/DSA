using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfLongestIncreasingSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfLongestIncreasingSubsequenceSolution's,
// the same methods NumberOfLongestIncreasingSubsequenceTests proves correct.
// The textbook O(n^2) DP vs. this repo's own SegmentTree<Element,
// ICombineOperation<Element>> keyed by a BinarySearch.LowerBound-compressed
// rank - O(n log n).
[MemoryDiagnoser]
public class NumberOfLongestIncreasingSubsequenceBenchmarks
{
    private const int RandomSeed = 673; private int[] _values = [];

    // LC 673

    [Params(3_000, 8_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming() => NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisByBruteForce(_values);

    [Benchmark]
    public int SegmentTreeCoordinateCompression()
        => NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisBySegmentTree(_values);
}
