using DSAExperimentation.LeetCode.Solutions.FindModeInBinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindModeInBinarySearchTree;

public sealed partial class FindModeInBinarySearchTreePromiseTests
{
    public static TheoryData<int?[], bool> Inputs =>
        new()
        {
            { [1, null, 2, 2], true },
            { [0], true },
            { [1, 1, 2, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASearchTree(int?[] levelOrder, bool expected)
        => Assert.Equal(expected, FindModeInBinarySearchTreePromise.IsKeptBy(LeetCodeWireFormat.ToBinaryTree(levelOrder)));
}
