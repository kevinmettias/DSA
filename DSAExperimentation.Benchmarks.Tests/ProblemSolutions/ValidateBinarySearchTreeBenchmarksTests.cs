using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): the class carries a
// single arm - the previous class's second [Benchmark] called the same private helper under a
// different name, so one strategy was removed rather than left as two - and there is therefore no
// agreement to assert, only an oracle.
//
// It carries no [Params] either: [GlobalSetup] fixes one three-node tree, root 2 with left 1 and
// right 3, which is LC 98's own second example and whose published answer is true. That decisive
// literal is the assertion below.
public sealed partial class ValidateBinarySearchTreeBenchmarksTests
{
    // LC 98's own second example: 1 < 2 < 3, so the bounds recursion accepts the tree.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_FixedTree_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsValidByBoundsRecursion(), BuildHarness().IsValidByBoundsRecursion());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBoundsRecursion());
    }

    [Fact]
    public void IsValidByBoundsRecursion_FixedTree_AcceptsTheExampleTree() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBoundsRecursion());

    private static ValidateBinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new ValidateBinarySearchTreeBenchmarks();
        harness.Setup();

        return harness;
    }
}
