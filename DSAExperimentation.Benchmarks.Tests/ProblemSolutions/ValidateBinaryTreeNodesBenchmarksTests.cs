using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateBinaryTreeNodesBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidateBinaryTreeNodesSolution's competing strategies for the same question - the re-validate-
// from-every-candidate-root scan against one DisjointSet pass - so a harness whose arms disagree is
// validating two different graphs.
//
// Setup's leftChild chain n-1 -> n-2 -> ... -> 0 is a genuine binary tree rooted at the last index:
// every node has at most one parent, exactly one node has none, and the chain reaches all of them.
// LC 1361's answer is therefore true, and that decisive literal is asserted alongside the arms'
// agreement so agreement cannot hold on a shared wrong verdict. The chain is what forces the naive
// arm through its full O(n^2) rather than an O(1) rejection; Setup builds it from the node count
// alone, so the same NodeCount must rebuild the same pair of arrays.
public sealed partial class ValidateBinaryTreeNodesBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] node counts.
    private const int SmallestNodeCount = 200;

    // Setup's documented outcome: one rooted left-leaning chain covering every node.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsValidByRootScan(), BuildHarness().IsValidByRootScan());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByRootScan());
    }

    [Fact]
    public void IsValidByRootScan_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidByDisjointSet(), harness.IsValidByRootScan());
        Assert.Equal(ExpectedIsValid, harness.IsValidByRootScan());
    }

    [Fact]
    public void IsValidByDisjointSet_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidByRootScan(), harness.IsValidByDisjointSet());
        Assert.Equal(ExpectedIsValid, harness.IsValidByDisjointSet());
    }

    private static ValidateBinaryTreeNodesBenchmarks BuildHarness()
    {
        var harness = new ValidateBinaryTreeNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
