using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.LongestIncreasingSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestIncreasingSubsequenceSolution's, the same
// methods LongestIncreasingSubsequenceSolutionTests proves correct.
public class LongestIncreasingSubsequenceBenchmarks
{
    private int[] _values = [];

    [Params(2_000, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = SeededDraws.Values(Length, 0, Length, random);
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming() =>
        LongestIncreasingSubsequenceSolution.LengthOfLisByDynamicProgramming(_values);

    [Benchmark]
    public int PatienceSortingBinarySearch() =>
        LongestIncreasingSubsequenceSolution.LengthOfLisByPatienceSortingBinarySearch(_values);
}
