using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniqueBinarySearchTreesIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniqueBinarySearchTreesIISolution's competing strategies for the same question - the unmemoized
// range recursion against the memoized one - so a harness whose arms disagree is building two
// different sets of trees.
//
// Each arm returns the list of trees it built. Their number is decisive - it is Catalan(8) = 1430
// for the smaller parameter, the same constant UniqueBinarySearchTreesBenchmarks' counting sibling
// is anchored to - and so is that no two of them share a shape, so both are asserted for each arm
// rather than left to a shared wrong answer.
public sealed partial class UniqueBinarySearchTreesIIBenchmarksTests
{
    // The smaller of the class's [Params(8, 12)] node counts.
    private const int SmallestNodeCount = 8;

    // Catalan(8) = 1430: the number of distinct BST shapes over 8 ordered keys.
    private const int ExpectedTreeCount = 1_430;

    [Fact]
    public void PlainRecursion_SmallestNodeCount_BuildsEveryDistinctTree() =>
        AssertEveryDistinctTree(BuildHarness().PlainRecursion());

    [Fact]
    public void MemoizedRange_SmallestNodeCount_BuildsEveryDistinctTree() =>
        AssertEveryDistinctTree(BuildHarness().MemoizedRange());

    // Each tree is rendered on its own, so a subtree the memoized arm shares between trees renders
    // in full in every tree that holds it.
    private static void AssertEveryDistinctTree(object? answer)
    {
        var trees = Assert.IsType<List<BinaryTreeNode<int>?>>(answer);

        Assert.Equal(ExpectedTreeCount, trees.Count);
        Assert.Equal(ExpectedTreeCount, trees.Select(AnswerGraphText.Of).Distinct().Count());
    }

    private static UniqueBinarySearchTreesIIBenchmarks BuildHarness() => new() { Nodes = SmallestNodeCount };
}
