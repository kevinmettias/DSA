using DSAExperimentation.LeetCode.Solutions.SearchInABinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchInABinarySearchTree;

public sealed partial class SearchInABinarySearchTreePromiseTests
{
    public static TheoryData<int?[], int, bool> Inputs =>
        new()
        {
            { [4, 2, 7, 1, 3], 2, true },
            { [1, 2, 3, 4, 5, 6, 7], 4, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASearchTreeOfUniqueValues(int?[] levelOrder, int val, bool expected)
        => Assert.Equal(expected, SearchInABinarySearchTreePromise.IsKeptBy(LeetCodeWireFormat.ToBinaryTree(levelOrder), val));
}
