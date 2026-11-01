using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodNodesInBinaryTreeBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only the workload's
// documented shape: the count is neither vacuous nor trivially every node - the root is always good
// against the int.MinValue seed, while the deep nodes below it are not.
public sealed partial class CountGoodNodesInBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void TopDownTraversalCount_TwoHundredNodeTree_CountsSomeButNotEveryNode() =>
        Assert.InRange(BuildHarness().TopDownTraversalCount(), 1, SmallestNodeCount - 1);

    private static CountGoodNodesInBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new CountGoodNodesInBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
