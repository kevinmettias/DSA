using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumBinaryTree;

// Harness only. Both strategies are MaximumBinaryTreeSolution's - this file just
// pins them to LeetCode's published examples, comparing each built tree against a
// hand-written expected shape (a print, not a second construction algorithm).
public sealed partial class MaximumBinaryTreeTests
{
    public static TheoryData<int[], string> Examples =>
        new()
        {
            // [3,2,1,6,0,5] -> the classic LC 654 example tree.
            { [3, 2, 1, 6, 0, 5], "(6 (3 . (2 . (1 . .))) (5 (0 . .) .))" },
            // Ascending input produces a left-skewed chain: the max is always the
            // last element, so every node hangs off the previous one's Left.
            { [1, 2, 3], "(3 (2 (1 . .) .) .)" },
            { [1], "(1 . .)" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByRescanForMax_LeetCodeExamples_MatchesExpectedShape(int[] nums, string expectedShape) =>
        Assert.Equal(expectedShape, Serialize(MaximumBinaryTreeSolution.ConstructByRescanForMax(nums)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByMonotonicStack_LeetCodeExamples_MatchesExpectedShape(int[] nums, string expectedShape) =>
        Assert.Equal(expectedShape, Serialize(MaximumBinaryTreeSolution.ConstructByMonotonicStack(nums)));

    private static string Serialize(BinaryTreeNode<int>? node) =>
        node is null ? "." : $"({node.Value} {Serialize(node.Left)} {Serialize(node.Right)})";
}
