using DSAExperimentation.LeetCode.FindTheNumberOfPossibleWaysForAnEvent;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNumberOfPossibleWaysForAnEventSolution's.
// Stages and score range stay fixed while performer count grows, so the brute
// force's x^n enumeration is what the memoized DP has to beat.
//
// Sizes are per arm. That enumeration stops at 10 performers; the memoized DP is
// O(n * stages) and runs on to LC 3317's own bound of 1,000, and the two are compared
// at the counts both run.
public class FindTheNumberOfPossibleWaysForAnEventBenchmarks
{
    private const int Stages = 3;
    private const int MaxScore = 5;

    public static IEnumerable<int> BruteForceSizes => [6, 10];

    public static IEnumerable<int> StagePartitionMemoSizes => [.. BruteForceSizes, 100, 1_000];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForceEnumeration(int performers) =>
        FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByBruteForceEnumeration(performers, Stages, MaxScore);

    [Benchmark]
    [ArgumentsSource(nameof(StagePartitionMemoSizes))]
    public int StagePartitionMemo(int performers) =>
        FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByStagePartitionMemo(performers, Stages, MaxScore);
}
