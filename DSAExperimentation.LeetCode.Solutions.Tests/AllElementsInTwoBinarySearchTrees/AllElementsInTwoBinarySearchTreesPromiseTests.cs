using DSAExperimentation.LeetCode.Solutions.AllElementsInTwoBinarySearchTrees;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AllElementsInTwoBinarySearchTrees;

public sealed partial class AllElementsInTwoBinarySearchTreesPromiseTests
{
    // Two trees in LeetCode's level-order notation, and whether both are binary search trees.
    public static TheoryData<int?[], int?[], bool> Inputs =>
        new()
        {
            { [2, 1, 4], [1, 0, 3], true },
            { [], [5, 1, 7], true },
            { [1, null, 8], [8, 1], true },
            { [5, 3, 6], [2, 2, 3, null, null, 4, 4], false },
            { [8, null, 2], [], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenBothTreesListTheirValuesInOrder(int?[] first, int?[] second, bool expected)
    {
        var root1 = LeetCodeWireFormat.ToBinaryTree(first);
        var root2 = LeetCodeWireFormat.ToBinaryTree(second);

        Assert.Equal(expected, AllElementsInTwoBinarySearchTreesPromise.IsKeptBy(root1, root2));
    }
}
