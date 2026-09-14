using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.StepByStepDirectionsFromABinaryTreeNodeToAnother;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StepByStepDirectionsFromABinaryTreeNodeToAnother;

// Harness only. Both strategies are
// StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution's - this file states
// LeetCode's published examples once and pins each strategy to them.
// BinaryTreeNode<int> is internal, so it cannot appear in a public TheoryData
// member; the trees travel as LeetCode's own level-order arrays and BuildTree
// reconstructs one inside each test method.
public sealed class StepByStepDirectionsFromABinaryTreeNodeToAnotherTests
{
    public static TheoryData<int?[], int, int, string> Examples =>
        new()
        {
            // LeetCode's example 1: up out of the left subtree, back down the right.
            { [5, 1, 2, 3, null, 6, 4], 3, 6, "UURL" },
            // LeetCode's example 2: the destination is the start's own child.
            { [2, 1], 2, 1, "L" },
            // Same tree as example 1, descending into the other child of the turnaround.
            { [5, 1, 2, 3, null, 6, 4], 3, 4, "UURR" },
            // The destination is a descendant of the start, so the route never climbs.
            { [1, 2, null, null, 3], 1, 3, "LR" },
            // The reverse of it: the start is a descendant, so the route only climbs.
            { [1, 2, null, null, 3], 3, 1, "UU" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetDirectionsByPathSearch_LeetCodeExamples_ReturnsTheClimbThenDescent(
        int?[] levelOrder, int startValue, int destValue, string expected)
    {
        var root = BuildTree(levelOrder);

        var directions = StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution.GetDirectionsByPathSearch(
            root, startValue, destValue);

        Assert.Equal(expected, directions);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetDirectionsByLowestCommonAncestorPaths_LeetCodeExamples_ReturnsTheClimbThenDescent(
        int?[] levelOrder, int startValue, int destValue, string expected)
    {
        var root = BuildTree(levelOrder);

        var directions = StepByStepDirectionsFromABinaryTreeNodeToAnotherSolution
            .GetDirectionsByLowestCommonAncestorPaths(root, startValue, destValue);

        Assert.Equal(expected, directions);
    }

    // LeetCode's own level-order input shape: a BFS-ordered array with null
    // standing in for a missing child.
    private static BinaryTreeNode<int> BuildTree(int?[] levelOrder)
    {
        var root = new BinaryTreeNode<int>(levelOrder[0]!.Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (i < levelOrder.Length)
        {
            var current = queue.Dequeue();

            if (i < levelOrder.Length && levelOrder[i] is { } leftValue)
            {
                current.Left = new BinaryTreeNode<int>(leftValue);
                queue.Enqueue(current.Left);
            }

            i++;

            if (i < levelOrder.Length && levelOrder[i] is { } rightValue)
            {
                current.Right = new BinaryTreeNode<int>(rightValue);
                queue.Enqueue(current.Right);
            }

            i++;
        }

        return root;
    }
}
