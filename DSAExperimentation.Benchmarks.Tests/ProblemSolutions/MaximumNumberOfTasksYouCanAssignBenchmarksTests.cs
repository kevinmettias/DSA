using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfTasksYouCanAssignBenchmarks (ARCHITECTURE 17.9): both arms
// are MaximumNumberOfTasksYouCanAssignSolution's competing strategies for one question - a linear
// walk down "try k = maxK, maxK - 1, ..." against BinarySearch.LowerBound over an on-demand
// feasibility sequence - so a harness whose arms disagree is timing two different problems. Both
// answer with a single assignment count, compared directly.
public sealed partial class MaximumNumberOfTasksYouCanAssignBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Setup draws every task requirement from [500_000, 1_000_000) while the strongest worker
    // reaches only 99 + 5 pills * strength 100 = 599, so no worker can ever take a task and the
    // whole workload is unassignable. This is deliberately the answer the harness forces - it is
    // what makes the linear scan walk its full range before stopping - so it is asserted rather
    // than left implicit, and it is the decisive literal the Setup path is judged against.
    private const int ExpectedAssignableTasks = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUnassignableWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Both arrays and the prepared SortedTaskAssignment are private, so the rebuild is pinned
        // through the count they produce: the same Length must sort the same seeded tasks and
        // workers into the same prepared assignment.
        Assert.Equal(ExpectedAssignableTasks, first.LinearScan());
        Assert.Equal(ExpectedAssignableTasks, second.SequenceLowerBound());
    }

    [Fact]
    public void LinearScan_UnassignableWorkload_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.LinearScan());
    }

    [Fact]
    public void SequenceLowerBound_UnassignableWorkload_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.SequenceLowerBound());
    }

    private static MaximumNumberOfTasksYouCanAssignBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfTasksYouCanAssignBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
