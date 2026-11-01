using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfDfsStringsArePalindromesBenchmarks (ARCHITECTURE 17.9): the generic
// check holds the arms to each other, and this pins the shape of the baseline's answer to the
// workload: one flag per node of the chain Setup builds, which no comparison between arms can
// establish.
public sealed partial class CheckIfDfsStringsArePalindromesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // One flag per node is the shape the answer promises.
    [Fact]
    public void BruteForce_SeededChainTree_ReturnsOneFlagPerNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().BruteForce().Length);

    private static CheckIfDfsStringsArePalindromesBenchmarks BuildHarness()
    {
        var harness = new CheckIfDfsStringsArePalindromesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
