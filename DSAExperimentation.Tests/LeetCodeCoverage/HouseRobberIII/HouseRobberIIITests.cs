using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.HouseRobberIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberIII;

// Harness only. Both strategies are HouseRobberIIISolution's - this file pins them
// to LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature; BuildTree reconstructs it). RobFoldAlgebra (the catamorphism
// this repo's generic TreeFold engine closes over) lives beside the solution and is
// exercised only through RobByTreeFoldAlgebra here, per the same precedent as
// CountWaysToBuildRoomsInAnAntColony's RoomWaysAlgebra.
public sealed class HouseRobberIIITests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [3, 2, 3, null, 3, null, 1], 7 },
            { [3, 4, 5, 1, 3, null, 1], 9 },
            { [5], 5 },
            { [1, 2, 3], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByRecursivePair_LeetCodeExamples_ReturnsBestNonAdjacentSum(int?[] values, int expected) =>
        Assert.Equal(expected, HouseRobberIIISolution.RobByRecursivePair(BuildTree(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByTreeFoldAlgebra_LeetCodeExamples_ReturnsBestNonAdjacentSum(int?[] values, int expected) =>
        Assert.Equal(expected, HouseRobberIIISolution.RobByTreeFoldAlgebra(BuildTree(values)));

    // LeetCode's level-order array shape: each existing node consumes exactly two
    // subsequent slots for its children, null marking a missing one. LC 337
    // guarantees at least one node, so the root is never null.
    private static BinaryTreeNode<int> BuildTree(int?[] values)
    {
        var root = new BinaryTreeNode<int>(values[0]!.Value);
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);
        var i = 1;

        while (queue.Count > 0 && i < values.Length)
        {
            var node = queue.Dequeue();

            if (values[i] is int leftValue)
            {
                node.Left = new BinaryTreeNode<int>(leftValue);
                queue.Enqueue(node.Left);
            }

            i++;

            if (i < values.Length && values[i] is int rightValue)
            {
                node.Right = new BinaryTreeNode<int>(rightValue);
                queue.Enqueue(node.Right);
            }

            i++;
        }

        return root;
    }
}
