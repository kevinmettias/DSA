using DSAExperimentation.LeetCode.Solutions.RangeSumOfBST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RangeSumOfBST;

public sealed partial class RangeSumOfBSTPromiseTests
{
    public static TheoryData<int?[], int, int, bool> Inputs =>
        new()
        {
            { [10, 5, 15, 3, 7, null, 18], 7, 15, true },
            { [10, 5, 15, 3, 7, 13, 18, 1, null, 6], 6, 10, true },
            { [10, 15, 5], 1, 20, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASearchTreeOfUniqueValues(int?[] levelOrder, int low, int high, bool expected)
        => Assert.Equal(expected, RangeSumOfBSTPromise.IsKeptBy(LeetCodeWireFormat.ToBinaryTree(levelOrder), low, high));
}
