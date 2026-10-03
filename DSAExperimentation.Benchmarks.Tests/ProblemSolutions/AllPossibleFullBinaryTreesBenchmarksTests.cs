using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllPossibleFullBinaryTreesBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: how many full binary trees the node count admits, which is the Catalan number of (n-1)/2 - a literal known
// without consulting either arm. Both arms return the list of tree roots as object? (the node type is internal,
// CS0050), and each tree is read back in LeetCode's level-order notation, so the list is held to that many trees
// with no shape repeated. Reading each tree on its own is also what holds the arms to the same answer: the
// memoized arm shares subtrees across trees, so ArmAgreement leaves their object graphs uncompared. The class
// has no [GlobalSetup]: the
// single [Params] node count is the whole input, so the harness is constructed per size and the arms called directly.
public sealed partial class AllPossibleFullBinaryTreesBenchmarksTests
{
    // The smaller of [Params(13, 19)] node counts. A full binary tree has an odd node count,
    // and 13 leaves 6 internal nodes to split.
    private const int SmallestNodes = 13;

    // The 6th Catalan number: the number of full binary trees over 13 nodes.
    private const int ExpectedFullTreeCount = 132;

    [Fact]
    public void Naive_ThirteenNodes_BuildsEveryDistinctFullTreeOnce() =>
        AssertEveryDistinctFullTreeOnce(BuildHarness().Naive());

    [Fact]
    public void Memoized_ThirteenNodes_BuildsEveryDistinctFullTreeOnce() =>
        AssertEveryDistinctFullTreeOnce(BuildHarness().Memoized());

    private static void AssertEveryDistinctFullTreeOnce(object? answer)
    {
        var shapes = Assert.IsType<List<BinaryTreeNode<int>?>>(answer)
            .Select(root => AnswerGraphText.Of(LeetCodeWireFormat.FromBinaryTree(root)))
            .ToList();

        Assert.Equal(ExpectedFullTreeCount, shapes.Count);
        Assert.Equal(ExpectedFullTreeCount, shapes.Distinct(StringComparer.Ordinal).Count());
    }

    private static AllPossibleFullBinaryTreesBenchmarks BuildHarness() =>
        new() { Nodes = SmallestNodes };
}
