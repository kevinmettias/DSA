using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructBinarySearchTreeFromPreorderTraversalBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this pins the
// baseline's root to the value the workload's shape decides. Arms report the built tree's root
// value, which for a BST built from preorder is the array's first value.
//
// Setup builds an ascending run from one, which is both a valid BST preorder and the adversarial
// shape the comment names: a fully right-skewed chain, where each Insert walks the whole height built
// so far. Its first value is the root the baseline must report.
//
// That root value is a proxy, not the answer: the class returns it only so the built tree cannot be
// optimized away, so this pins the root, not the tree underneath.
public sealed partial class ConstructBinarySearchTreeFromPreorderTraversalBenchmarksTests
{
    private const int SmallestLength = 10;
    private const int AscendingRunFirstValue = 1;

    [Fact]
    public void BoundedRecursion_RightSkewedAscendingPreorder_RootsTheTreeAtTheFirstValue() =>
        Assert.Equal(AscendingRunFirstValue, BuildHarness().BoundedRecursion());

    private static ConstructBinarySearchTreeFromPreorderTraversalBenchmarks BuildHarness()
    {
        var harness = new ConstructBinarySearchTreeFromPreorderTraversalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
