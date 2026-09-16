using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConvertBSTToGreaterTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the hand-rolled reverse in-order walk against this
// repo's ascending-only InOrderTraversal composed into the same transformation - so a harness whose
// arms disagree is timing two different problems. Each arm returns the transformed root's value, an
// int, so they are compared directly. That shared int is a proxy for the whole transformation: it
// pins the one node whose new value depends on every other node's, but says nothing about the other
// four hundred and ninety-nine, so this agreement is honest and weak, and no return type is changed
// here to strengthen it. Setup builds the shared tree from a fixture with no seed of its
// own, so the same NodeCount must rebuild the same tree, and its documented shape is that each arm
// clones before transforming: a second call on the same harness must therefore read the same
// untransformed values rather than re-transforming an already-transformed tree.
public sealed partial class ConvertBSTToGreaterTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameUntransformedTree()
    {
        Assert.Equal(BuildHarness().ManualReverseInOrder(), BuildHarness().ManualReverseInOrder());

        var harness = BuildHarness();

        Assert.Equal(harness.ManualReverseInOrder(), harness.ManualReverseInOrder());
    }

    [Fact]
    public void ManualReverseInOrder_BalancedTree_AgreesWithInOrderTraversalHooks()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InOrderTraversalHooks(), harness.ManualReverseInOrder());
    }

    [Fact]
    public void InOrderTraversalHooks_BalancedTree_AgreesWithManualReverseInOrder()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualReverseInOrder(), harness.InOrderTraversalHooks());
    }

    private static ConvertBSTToGreaterTreeBenchmarks BuildHarness()
    {
        var harness = new ConvertBSTToGreaterTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
