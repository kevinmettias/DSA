using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumMedianSumOfSubsequencesOfSizeThree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumMedianSumOfSubsequencesOfSizeThreeSolution's,
// the same methods MaximumMedianSumOfSubsequencesOfSizeThreeTests proves
// correct (NumberOfIntegersWithPopcountDepthEqualToKIBenchmarks precedent - no
// [GlobalSetup] beyond the one random array both arms share, since nums is
// the LeetCode input itself). N stays small for the brute-force partition
// search - its search tree grows combinatorially, into the hundreds of
// thousands of partial groupings by a dozen elements - while the
// sorted-greedy arm scales to the real problem's n up to 5*10^5 trivially.
[MemoryDiagnoser]
public class MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarks
{
    private const int Seed = 3627; // LC problem number
    private const int MaxValue = 1_000_000_000;

    private int[] _nums = [];

    [Params(6, 12)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var nums = new int[N];

        for (var i = 0; i < N; i++)
        {
            nums[i] = random.Next(1, MaxValue);
        }

        _nums = nums;
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() =>
        MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumByBruteForce(_nums);

    [Benchmark]
    public long SortedGreedy() =>
        MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumBySortedGreedy(_nums);
}
