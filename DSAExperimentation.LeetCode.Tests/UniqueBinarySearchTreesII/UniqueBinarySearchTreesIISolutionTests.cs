using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.UniqueBinarySearchTreesII;

namespace DSAExperimentation.LeetCode.Tests.UniqueBinarySearchTreesII;

// Harness only. Both strategies are UniqueBinarySearchTreesIISolution's - this
// file pins them to LeetCode's two published examples, each tree read back in
// LeetCode's level-order notation and compared with the published output. The
// shapes are what is compared, not the in-order sequence: every BST over [1..n]
// shares that one, so a check on it passes any five trees over [1..3]. LeetCode
// accepts the trees in any order, so both lists are sorted before comparing.
public sealed partial class UniqueBinarySearchTreesIISolutionTests
{
    public static TheoryData<int, int?[][]> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            { 3, [[1, null, 2, null, 3], [1, null, 3, 2], [2, 1, 3], [3, 1, null, null, 2], [3, 2, null, 1]] },
            { 1, [[1]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateTreesByPlainRecursion_LeetCodeExamples_ReturnsEveryPublishedShape(
        int nodeCount, int?[][] expectedShapes) =>
        AssertShapes(expectedShapes, UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(nodeCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateTreesByMemoizedRange_LeetCodeExamples_ReturnsEveryPublishedShape(
        int nodeCount, int?[][] expectedShapes) =>
        AssertShapes(expectedShapes, UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(nodeCount));

    [Fact]
    public void GenerateTreesByPlainRecursion_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByPlainRecursion(1));

    [Fact]
    public void GenerateTreesByMemoizedRange_NOne_ReturnsSingleLeafTree() =>
        AssertSingleLeafTree(UniqueBinarySearchTreesIISolution.GenerateTreesByMemoizedRange(1));

    // Each shape becomes one comma-joined key (an absent child joins as an empty
    // field), and the two key lists are compared in ordinal order, so the published
    // and the returned order need not agree but every shape must appear exactly as
    // often in both.
    private static void AssertShapes(int?[][] expectedShapes, List<BinaryTreeNode<int>?> trees)
    {
        var returnedShapes = trees.Select(LeetCodeWireFormat.FromBinaryTree);

        Assert.Equal(SortedShapeKeys(expectedShapes), SortedShapeKeys(returnedShapes));
    }

    private static string[] SortedShapeKeys(IEnumerable<int?[]> shapes) =>
        [.. shapes.Select(shape => string.Join(',', shape)).Order(StringComparer.Ordinal)];

    private static void AssertSingleLeafTree(List<BinaryTreeNode<int>?> trees)
    {
        // For n = 1 the recurrence's only non-inverted branch is (1,1): the nulls it
        // lists for the inverted (1,0) and (2,1) become that leaf's absent children
        // rather than a list entry, since AppendCombinations fills the list with the
        // node it builds. IsType asks for that node instead of promising it is there.
        var tree = Assert.IsType<BinaryTreeNode<int>>(Assert.Single(trees));

        Assert.Equal(1, tree.Value);
        Assert.Null(tree.Left);
        Assert.Null(tree.Right);
    }
}
