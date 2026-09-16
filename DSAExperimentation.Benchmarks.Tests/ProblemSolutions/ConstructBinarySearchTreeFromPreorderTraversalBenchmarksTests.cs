using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinarySearchTreeFromPreorderTraversalBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - a fresh compare-and-descend Insert
// walk per value against the single-pass upper-bound recursion - so a harness whose arms disagree is
// timing two different problems. Both arms report the built tree's root value, which for a BST built
// from preorder is the array's first value, so the workload Setup builds is observable through it.
//
// Setup builds an ascending run from zero, which is both a valid BST preorder and the adversarial
// shape the comment names: a fully right-skewed chain, where each Insert walks the whole height built
// so far. The same Length must rebuild the same ascending run, and its first value is the root both
// arms must report.
//
// That root value is a proxy, not the answer: the class returns it only so the built tree cannot be
// optimized away, so agreement witnesses that both arms rooted the tree at the same value and that
// neither threw, not that they built the same tree underneath.
public sealed partial class ConstructBinarySearchTreeFromPreorderTraversalBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int AscendingRunFirstValue = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameAscendingPreorder()
    {
        Assert.Equal(AscendingRunFirstValue, BuildHarness().BoundedRecursion());
        Assert.Equal(BuildHarness().BoundedRecursion(), BuildHarness().BoundedRecursion());
    }

    [Fact]
    public void RepeatedTreeInsert_RightSkewedAscendingPreorder_AgreesWithBoundedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BoundedRecursion(), harness.RepeatedTreeInsert());
    }

    [Fact]
    public void BoundedRecursion_RightSkewedAscendingPreorder_AgreesWithRepeatedTreeInsert()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedTreeInsert(), harness.BoundedRecursion());
    }

    private static ConstructBinarySearchTreeFromPreorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinarySearchTreeFromPreorderTraversalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
