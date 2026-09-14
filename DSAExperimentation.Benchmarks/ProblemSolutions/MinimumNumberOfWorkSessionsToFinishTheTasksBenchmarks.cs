using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfWorkSessionsToFinishTheTasks;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfWorkSessionsToFinishTheTasksSolution's,
// the same methods MinimumNumberOfWorkSessionsToFinishTheTasksTests proves correct.
// Task durations are all 1 with sessionTime 2, so both single tasks and pairs are
// feasible sessions - real combinatorial choice, not a forced single grouping -
// which is what makes the unmemoized call tree blow up: TaskCount stays small (<= 9)
// because that blowup is factorial-ish and would otherwise make the baseline
// impractically slow, unlike the DP side's ~3^n submask-enumeration cost. Both arms
// are handed a pre-built FeasibleSessionMasks via their hoisted overload, so the
// sum-over-subsets table is charged to [GlobalSetup] rather than to the recurrence
// being measured.
[MemoryDiagnoser]
public class MinimumNumberOfWorkSessionsToFinishTheTasksBenchmarks
{
    private const int SessionTime = 2;
    private const int TaskDuration = 1;

    [Params(6, 9)]
    public int TaskCount;

    private FeasibleSessionMasks _sessions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var tasks = Enumerable.Repeat(TaskDuration, TaskCount).ToArray();
        _sessions = FeasibleSessionMasks.Build(tasks, SessionTime);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByUnmemoizedRecursion(_sessions);

    [Benchmark]
    public int MemoizedBitmaskDp() =>
        MinimumNumberOfWorkSessionsToFinishTheTasksSolution.MinSessionsByMemoizedBitmaskDp(_sessions);
}
