using DSAExperimentation.LeetCode.Solutions.ClosestNodesQueriesInABinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ClosestNodesQueriesInABinarySearchTree;

public sealed partial class ClosestNodesQueriesInABinarySearchTreePromiseTests
{
    public static TheoryData<int?[], int[], bool> Inputs =>
        new()
        {
            { [6, 2, 13, 1, 4, 9, 15, null, null, null, null, null, null, 14], [2, 5, 16], true },
            { [50, 25, 75, 10, 30, 60, 90, 5, 15, 28, 35, 55, 65, 85, 95, 1, 9, 20, 27], [20], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheTreeListsItsValuesInOrder(int?[] levelOrder, int[] queries, bool expected)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);

        Assert.Equal(expected, ClosestNodesQueriesInABinarySearchTreePromise.IsKeptBy(root, queries));
    }
}
