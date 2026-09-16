using DSAExperimentation.LeetCode.ValidateBinaryTreeNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinaryTreeNodes;

// Harness only. Both strategies are ValidateBinaryTreeNodesSolution's - the
// DisjointSet one-pass check and the naive per-root re-traversal that used to live
// untested in the benchmark - pinned here to LeetCode's published examples plus the
// two failure shapes the union-find arm exists to catch (a node with two parents,
// and a cycle among non-root nodes).
public sealed class ValidateBinaryTreeNodesTests
{
    public static TheoryData<TreeNodesExample> Examples =>
        new()
        {
            new TreeNodesExample(4, [1, -1, 3, -1], [2, -1, -1, -1], FormsOneTree: true),
            new TreeNodesExample(4, [1, -1, 3, -1], [-1, -1, -1, -1], FormsOneTree: false),
            new TreeNodesExample(2, [1, 0], [-1, -1], FormsOneTree: false),
            new TreeNodesExample(3, [2, 2, -1], [-1, -1, -1], FormsOneTree: false),
            new TreeNodesExample(3, [1, 2, 0], [-1, -1, -1], FormsOneTree: false),
            new TreeNodesExample(1, [-1], [-1], FormsOneTree: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidateByDisjointSet_LeetCodeExamples_ReturnsWhetherNodesFormOneTree(TreeNodesExample example)
    {
        var actual = ValidateBinaryTreeNodesSolution.ValidateByDisjointSet(
            example.N, example.LeftChild, example.RightChild);

        Assert.Equal(example.FormsOneTree, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidateByRootScan_LeetCodeExamples_ReturnsWhetherNodesFormOneTree(TreeNodesExample example)
    {
        var actual = ValidateBinaryTreeNodesSolution.ValidateByRootScan(
            example.N, example.LeftChild, example.RightChild);

        Assert.Equal(example.FormsOneTree, actual);
    }

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TreeNodesExample(
        int N, int[] LeftChild, int[] RightChild, bool FormsOneTree);
}
