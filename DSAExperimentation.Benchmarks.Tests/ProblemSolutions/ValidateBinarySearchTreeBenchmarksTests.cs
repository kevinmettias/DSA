using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): the class carries a
// single arm - the previous class's second [Benchmark] called the same private helper under a
// different name, so one strategy was removed rather than left as two - and there is therefore no
// agreement to assert, only an oracle.
//
// [GlobalSetup] inserts distinct values into this repo's own BinarySearchTree, whose ordering
// invariant puts every smaller value in a node's left subtree and every larger one in its right, so
// the tree it hands the arm is a valid search tree by construction and the decisive answer is true.
// The insertion order is seeded, so a rebuilt harness must answer the same.
public sealed partial class ValidateBinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    // Distinct values inserted into a search tree always form a valid one.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsValidByBoundsRecursion(), BuildHarness().IsValidByBoundsRecursion());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBoundsRecursion());
    }

    [Fact]
    public void IsValidByBoundsRecursion_InsertionBuiltTree_AcceptsTheTree() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBoundsRecursion());

    private static ValidateBinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new ValidateBinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
