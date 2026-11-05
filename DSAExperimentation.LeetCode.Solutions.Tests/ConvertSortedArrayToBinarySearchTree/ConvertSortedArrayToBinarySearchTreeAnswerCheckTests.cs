using DSAExperimentation.LeetCode.Solutions.ConvertSortedArrayToBinarySearchTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConvertSortedArrayToBinarySearchTree;

public sealed partial class ConvertSortedArrayToBinarySearchTreeAnswerCheckTests
{
    public static TheoryData<int[], int?[], int?[], bool> Answers =>
        new()
        {
            { [-10, -3, 0, 5, 9], [0, -3, 9, -10, null, 5], [0, -3, 9, -10, null, 5], true },
            { [-10, -3, 0, 5, 9], [0, -10, 5, null, -3, null, 9], [0, -3, 9, -10, null, 5], true },
            { [1, 2, 3], [1, null, 2, null, 3], [2, 1, 3], false },
            { [1, 2, 3], [2, 3, 1], [2, 1, 3], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAHeightBalancedSearchTreeOfTheValues(int[] nums, int?[] answer, int?[] recorded, bool expected)
        => Assert.Equal(expected, ConvertSortedArrayToBinarySearchTreeAnswerCheck.IsAccepted(nums, LeetCodeWireFormat.ToBinaryTree(answer), LeetCodeWireFormat.ToBinaryTree(recorded)));
}
