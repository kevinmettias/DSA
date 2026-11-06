using DSAExperimentation.LeetCode.Solutions.InsertIntoABinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.InsertIntoABinarySearchTree;

public sealed partial class InsertIntoABinarySearchTreePromiseTests
{
    public static TheoryData<int?[], int, bool> Inputs =>
        new()
        {
            { [4, 2, 7, 1, 3], 5, true },
            { [], 5, true },
            { [2, 2, null, 2, null, 2], 3, false },
            { [4, 2, 7, 1, 5], 6, false },
            { [4, 2, 7, 1, 3], 3, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASearchTreeOfUniqueValuesWithoutTheNewOne(int?[] tree, int newValue, bool expected)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(tree);

        Assert.Equal(expected, InsertIntoABinarySearchTreePromise.IsKeptBy(root, newValue));
    }
}
