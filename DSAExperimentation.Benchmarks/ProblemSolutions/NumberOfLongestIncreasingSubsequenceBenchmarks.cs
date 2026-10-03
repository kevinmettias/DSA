using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfLongestIncreasingSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfLongestIncreasingSubsequenceSolution's,
// the same methods NumberOfLongestIncreasingSubsequenceSolutionTests proves correct.
// The textbook O(n^2) DP vs. this repo's own SegmentTree<Element,
// ICombineOperation<Element>> keyed by a BinarySearch.LowerBound-compressed
// rank - O(n log n). Length stops at LC 673's own bound of 2,000, where this seed's
// count of longest subsequences (213,770,240) still fits the 32-bit answer LC 673
// guarantees.
public class NumberOfLongestIncreasingSubsequenceBenchmarks
{
    private const int RandomSeed = 673; private int[] _values = [];

    // LC 673

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, 0, Length, random);
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming() => NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisByBruteForce(_values);

    [Benchmark]
    public int SegmentTreeCoordinateCompression()
        => NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisBySegmentTree(_values);
}
