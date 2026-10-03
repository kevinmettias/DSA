using DSAExperimentation.LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfWorkSessionsToFinishTheTasksSolution's,
// the same methods MinimumNumberOfWorkSessionsToFinishTheTasksSolutionTests proves correct.
// Task durations are all 1 with sessionTime 2, so both single tasks and pairs are
// feasible sessions - real combinatorial choice, not a forced single grouping -
// which is what makes the unmemoized call tree blow up. Both arms are handed a
// pre-built FeasibleSessionMasks via their hoisted overload, so the sum-over-subsets
// table is charged to [GlobalSetup] rather than to the recurrence being measured.
//
// Sizes are per arm. The unmemoized blowup is factorial-ish, so that arm stops at 9
// tasks; the memoized DP pays only its ~3^n submask enumeration and runs on to
// LC 1986's own bound of 14. The two are compared at the sizes both run.
public class MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks
{
    private const int SessionTime = 2;
    private const int TaskDuration = 1;

    private Dictionary<int, FeasibleSessionMasks> _sessionsBySize = [];

    public static IEnumerable<int> BaselineSizes => [6, 9];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 12, 14];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _sessionsBySize = MemoizedSizes.ToDictionary(taskCount => taskCount, BuildSessions);

    private static FeasibleSessionMasks BuildSessions(int taskCount)
    {
        var tasks = Enumerable.Repeat(TaskDuration, taskCount).ToArray();

        return FeasibleSessionMasks.Build(tasks, SessionTime);
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int UnmemoizedRecursion(int taskCount) =>
        MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByUnmemoizedRecursion(
            _sessionsBySize[taskCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedBitmaskDp(int taskCount) =>
        MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByMemoizedBitmaskDp(
            _sessionsBySize[taskCount]);
}
