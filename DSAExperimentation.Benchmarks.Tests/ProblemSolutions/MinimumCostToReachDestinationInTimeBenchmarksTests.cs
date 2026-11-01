using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToReachDestinationInTimeBenchmarks (ARCHITECTURE 17.9): an
// unreachable destination is reported as a shared sentinel in both arms, so two arms that both gave
// up would still agree; Setup builds a solvable step chain, and that the baseline reaches the last
// city is a property of the workload the generic agreement check cannot supply.
public sealed partial class MinimumCostToReachDestinationInTimeBenchmarksTests
{
    private const int SmallestLastCity = 10;

    // Both strategies report the same sentinel when the budget cannot pay for any route; the step
    // chain is solvable, so this is only the value a broken workload would collapse onto.
    private const int UnreachableSentinel = -1;

    [Fact]
    public void NaiveDfs_TwoWayStepChain_ReachesTheLastCity() =>
        Assert.NotEqual(UnreachableSentinel, BuildHarness().NaiveDfs(SmallestLastCity));

    private static MinimumCostToReachDestinationInTimeBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToReachDestinationInTimeBenchmarks();
        harness.Setup();

        return harness;
    }
}
