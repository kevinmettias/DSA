using DSAExperimentation.LeetCode.Solutions.ValidateBinaryTreeNodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidateBinaryTreeNodes;

// Harness only. Both strategies are ValidateBinaryTreeNodesSolution's - the
// DisjointSet one-pass check and the naive per-root re-traversal that used to live
// untested in the benchmark - pinned here to the two failure shapes the union-find
// arm exists to catch (a node with two parents, and a cycle among non-root nodes)
// and to a forest of two trees. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ValidateBinaryTreeNodesSolutionTests
{
    public static TheoryData<TreeNodesCase> Cases =>
        new()
        {
            new TreeNodesCase(4, [1, -1, 3, -1], [-1, -1, -1, -1], FormsOneTree: false),
            new TreeNodesCase(3, [2, 2, -1], [-1, -1, -1], FormsOneTree: false),
            new TreeNodesCase(3, [1, 2, 0], [-1, -1, -1], FormsOneTree: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByDisjointSet_Cases_ReturnsWhetherNodesFormOneTree(TreeNodesCase testCase)
    {
        var actual = ValidateBinaryTreeNodesSolution.IsValidByDisjointSet(
            testCase.N, testCase.LeftChild, testCase.RightChild);

        Assert.Equal(testCase.FormsOneTree, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidByRootScan_Cases_ReturnsWhetherNodesFormOneTree(TreeNodesCase testCase)
    {
        var actual = ValidateBinaryTreeNodesSolution.IsValidByRootScan(
            testCase.N, testCase.LeftChild, testCase.RightChild);

        Assert.Equal(testCase.FormsOneTree, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TreeNodesCase(
        int N, int[] LeftChild, int[] RightChild, bool FormsOneTree);
}
