using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateBinaryTreeNodesBenchmarks (ARCHITECTURE 17.9): the generic check
// holds the arms to agreeing with each other, which a shared wrong verdict would pass, so this pins
// the baseline's verdict to the one the workload's construction fixes.
//
// Setup's leftChild chain n-1 -> n-2 -> ... -> 0 is a genuine binary tree rooted at the last index:
// every node has at most one parent, exactly one node has none, and the chain reaches all of them.
// LC 1361's answer is therefore true. The chain is what forces the naive arm through its full
// O(n^2) rather than an O(1) rejection.
public sealed partial class ValidateBinaryTreeNodesBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] node counts.
    private const int SmallestNodeCount = 200;

    // Setup's documented outcome: one rooted left-leaning chain covering every node.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void IsValidByRootScan_LeftLeaningChain_AcceptsTheTree() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByRootScan());

    private static ValidateBinaryTreeNodesBenchmarks BuildHarness()
    {
        var harness = new ValidateBinaryTreeNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
