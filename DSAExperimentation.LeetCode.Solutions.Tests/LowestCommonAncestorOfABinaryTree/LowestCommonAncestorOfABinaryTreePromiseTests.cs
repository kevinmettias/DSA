using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.LowestCommonAncestorOfABinaryTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LowestCommonAncestorOfABinaryTree;

public sealed partial class LowestCommonAncestorOfABinaryTreePromiseTests
{
    public static TheoryData<int?[], int, int, bool> Inputs =>
        new()
        {
            { [3, 5, 1, 6, 2, 0, 8, null, null, 7, 4], 5, 1, true },
            { [1, 2], 1, 2, true },
            { [30, 15, 18, 3, 17, 19, 20, 1, 16, null, null, null, 14, 15], 3, 17, false },
            { [3, 5, 1], 5, 9, false },
            { [3, 5, 1], 5, 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForUniqueValuesAndTwoOfThem(int?[] levelOrder, int firstValue, int secondValue, bool expected)
    {
        var root = Assert.IsType<BinaryTreeNode<int>>(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(expected, LowestCommonAncestorOfABinaryTreePromise.IsKeptBy(root, firstValue, secondValue));
    }
}
