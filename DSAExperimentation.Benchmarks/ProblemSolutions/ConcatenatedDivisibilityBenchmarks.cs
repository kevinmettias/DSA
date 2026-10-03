using DSAExperimentation.LeetCode.ConcatenatedDivisibility;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConcatenatedDivisibilitySolution's, the same
// methods ConcatenatedDivisibilitySolutionTests proves correct. No hoisted overload is
// needed - nums/divisor are already the cheap, plain-array shape [GlobalSetup] would
// produce either way, the same reasoning MedianOfTwoSortedArraysBenchmarks
// applies to its own nums1/nums2.
//
// Sizes are per arm. Backtracking is O(n!) in the worst case, so it stops at 9
// numbers - exactly the range where the bitmask-DP arm's 2^n * divisor states start
// pulling away from the baseline's factorial ones. The DP runs on to LC 3533's own
// bound of 13, and the two are compared at the counts both run.
public class ConcatenatedDivisibilityBenchmarks
{
    // LC problem number, reused as the deterministic input seed.
    private const int Seed = 3533;

    private Dictionary<int, (int[] Nums, int Divisor)> _inputByCount = [];

    public static IEnumerable<int> BacktrackingSizes => [6, 9];

    public static IEnumerable<int> BitmaskMemoSizes => [.. BacktrackingSizes, 11, 13];

    // Every number count any arm runs is drawn here, outside the timed region, each from
    // the generator seeded by Seed + its own count; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _inputByCount = BitmaskMemoSizes.ToDictionary(count => count, BuildInput);

    private static (int[] Nums, int Divisor) BuildInput(int numberCount)
    {
        var random = new Random(Seed + numberCount);
        int[] nums = [.. Enumerable.Range(0, numberCount).Select(_ => random.Next(1, 100_000))];

        return (nums, random.Next(1, 101));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BacktrackingSizes))]
    public IList<int> Backtracking(int numberCount)
    {
        var (nums, divisor) = _inputByCount[numberCount];

        return ConcatenatedDivisibilitySolution.SmallestPermutationByBacktracking(nums, divisor);
    }

    [Benchmark]
    [ArgumentsSource(nameof(BitmaskMemoSizes))]
    public IList<int> BitmaskMemo(int numberCount)
    {
        var (nums, divisor) = _inputByCount[numberCount];

        return ConcatenatedDivisibilitySolution.SmallestPermutationByBitmaskMemo(nums, divisor);
    }
}
