using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution's, the same methods
// PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolutionTests proves correct - a
// direct O(2^(2n)) scan of every size-n bitmask over the whole array against
// meet-in-the-middle over each half's subset sums grouped by subset size.
//
// Sizes are per arm, counted in elements (always even, since LeetCode hands over 2n).
// The full scan's 2^(2n) masks stop it at 20; meet-in-the-middle only enumerates
// 2^n sums per half and runs on to LC 2035's own bound of 30 elements. The two are
// compared at the sizes both run.
public class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 2035;

    // Symmetric bound for the generated values' range: [-ValueBound, ValueBound).
    private const int ValueBound = 50;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BaselineSizes => [16, 20];

    public static IEnumerable<int> MeetInTheMiddleSizes => [.. BaselineSizes, 24, 30];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsBySize = MeetInTheMiddleSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, -ValueBound, ValueBound, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceAllEqualSplits(int length) =>
        PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByBruteForceEqualSplits(_numsBySize[length]);

    [Benchmark]
    [ArgumentsSource(nameof(MeetInTheMiddleSizes))]
    public int MeetInTheMiddleGroupedBySize(int length) =>
        PartitionArrayIntoTwoArraysToMinimizeSumDifferenceSolution
            .MinimumDifferenceByMeetInTheMiddle(_numsBySize[length]);
}
