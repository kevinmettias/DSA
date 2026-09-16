using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfRobotsWithinBudgetBenchmarks (ARCHITECTURE 17.9): both arms
// are MaximumNumberOfRobotsWithinBudgetSolution's competing strategies for one question - the
// per-left-edge rescan against one monotonic-deque sliding window - so a harness whose arms
// disagree is timing two different problems. Both answer with a single robot count, compared
// directly.
public sealed partial class MaximumNumberOfRobotsWithinBudgetBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameChargeTimesAndRunningCosts()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Both arrays are private, so the rebuild is pinned through the count they produce: the
        // same Length must draw the same seeded charge times and running costs and window them
        // identically.
        Assert.Equal(first.RescanEveryLeftEdge(), second.RescanEveryLeftEdge());
        Assert.Equal(first.MonotonicDequeSlidingWindow(), second.MonotonicDequeSlidingWindow());
    }

    [Fact]
    public void RescanEveryLeftEdge_SeededRobotCosts_AgreesWithMonotonicDequeSlidingWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequeSlidingWindow(), harness.RescanEveryLeftEdge());
    }

    [Fact]
    public void MonotonicDequeSlidingWindow_SeededRobotCosts_AgreesWithRescanEveryLeftEdge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanEveryLeftEdge(), harness.MonotonicDequeSlidingWindow());
    }

    private static MaximumNumberOfRobotsWithinBudgetBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfRobotsWithinBudgetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
