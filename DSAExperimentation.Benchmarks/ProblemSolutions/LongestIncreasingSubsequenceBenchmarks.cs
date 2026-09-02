using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestIncreasingSubsequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestIncreasingSubsequenceSolution's, the same
// methods LongestIncreasingSubsequenceTests proves correct.
[MemoryDiagnoser]
public class LongestIncreasingSubsequenceBenchmarks
{
    [Params(2_000, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming() =>
        LongestIncreasingSubsequenceSolution.LengthOfLisByDynamicProgramming(_values);

    [Benchmark]
    public int PatienceSortingBinarySearch() =>
        LongestIncreasingSubsequenceSolution.LengthOfLisByPatienceSortingBinarySearch(_values);
}
