using DSAExperimentation.LeetCode.MaximumMedianSumOfSubsequencesOfSizeThree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumMedianSumOfSubsequencesOfSizeThreeSolution's,
// the same methods MaximumMedianSumOfSubsequencesOfSizeThreeSolutionTests proves
// correct (NumberOfIntegersWithPopcountDepthEqualToKIBenchmarks precedent - no
// [GlobalSetup] beyond the one random array per size both arms share, since nums is
// the LeetCode input itself).
//
// Sizes are per arm, each a multiple of three as LC 3627 requires. The brute-force
// partition search grows combinatorially, into the hundreds of thousands of partial
// groupings by a dozen elements, so it stops at 12; the sorted-greedy arm is
// O(n log n) and runs on to 300,000, inside the real problem's bound of 5 * 10^5. The
// two are compared at the counts both run.
public class MaximumMedianSumOfSubsequencesOfSizeThreeBenchmarks
{
    private const int Seed = 3627; // LC problem number
    private const int MaxValue = 1_000_000_000;

    private Dictionary<int, int[]> _numsByCount = [];

    public static IEnumerable<int> BruteForceSizes => [6, 12];

    public static IEnumerable<int> SortedGreedySizes => [.. BruteForceSizes, 3_000, 300_000];

    // Every element count any arm runs is drawn here, outside the timed region, each from
    // its own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _numsByCount = SortedGreedySizes.ToDictionary(count => count, BuildNums);

    private static int[] BuildNums(int elementCount)
    {
        var random = new Random(Seed);
        var nums = new int[elementCount];

        for (var i = 0; i < elementCount; i++)
        {
            nums[i] = random.Next(1, MaxValue);
        }

        return nums;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int elementCount) =>
        MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumByBruteForce(_numsByCount[elementCount]);

    [Benchmark]
    [ArgumentsSource(nameof(SortedGreedySizes))]
    public long SortedGreedy(int elementCount) =>
        MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumBySortedGreedy(_numsByCount[elementCount]);
}
