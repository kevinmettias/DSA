using DSAExperimentation.LeetCode.Solutions.ConstructBinarySearchTreeFromPreorderTraversal;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConstructBinarySearchTreeFromPreorderTraversal;

public sealed partial class ConstructBinarySearchTreeFromPreorderTraversalPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [8, 5, 1, 7, 10, 12], true },
            { [1, 3], true },
            { [10, 5, 15, 3, 7, 12, 20], false },
            { [5, 5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASearchTreesPreorder(int[] preorder, bool expected)
        => Assert.Equal(expected, ConstructBinarySearchTreeFromPreorderTraversalPromise.IsKeptBy(preorder));
}
