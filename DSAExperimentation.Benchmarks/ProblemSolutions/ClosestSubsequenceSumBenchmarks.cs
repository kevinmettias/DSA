using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ClosestSubsequenceSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestSubsequenceSumSolution's, the same methods
// ClosestSubsequenceSumSolutionTests proves correct. The workload is a random array over a
// small magnitude bound and a goal far outside its reachable sum range, so neither
// arm ever lands on an exact match and both scan to completion - which is what makes
// the 2^n baseline and the 2*2^(n/2) meet-in-the-middle arm comparable.
//
// Sizes are per arm. The 2^n baseline stops at 20 elements; meet-in-the-middle only
// enumerates each half's 2^(n/2) sums and runs on to 32, and the two are compared at
// the lengths both run. LC 1755 allows 40, but its 2^20 sums a half there are past a
// benchmark's budget.
public class ClosestSubsequenceSumBenchmarks
{
    private const int Goal = 1_000_000;
    private const int RandomSeed = 1755; // LC problem number
    private const int ValueMagnitudeBound = 50;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [16, 20];

    public static IEnumerable<int> MeetInTheMiddleSizes => [.. BruteForceSizes, 26, 32];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = MeetInTheMiddleSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, -ValueMagnitudeBound, ValueMagnitudeBound, new Random(RandomSeed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForceAllSubsets(int length) =>
        ClosestSubsequenceSumSolution.MinAbsoluteDifferenceByBruteForceSubsets(_numsByLength[length], Goal);

    [Benchmark]
    [ArgumentsSource(nameof(MeetInTheMiddleSizes))]
    public int MeetInTheMiddle(int length) =>
        ClosestSubsequenceSumSolution.MinAbsoluteDifferenceByMeetInTheMiddle(_numsByLength[length], Goal);
}
