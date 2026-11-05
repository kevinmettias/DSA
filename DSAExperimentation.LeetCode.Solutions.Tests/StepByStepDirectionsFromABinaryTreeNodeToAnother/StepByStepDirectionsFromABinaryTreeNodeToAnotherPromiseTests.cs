using DSAExperimentation.LeetCode.Solutions.StepByStepDirectionsFromABinaryTreeNodeToAnother;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StepByStepDirectionsFromABinaryTreeNodeToAnother;

public sealed partial class StepByStepDirectionsFromABinaryTreeNodeToAnotherPromiseTests
{
    public static TheoryData<int?[], int, int, bool> Inputs =>
        new()
        {
            { [5, 1, 2, 3, null, 6, 4], 3, 6, true },
            { [2, 1], 2, 1, true },
            { [2, 1], 2, 3, false },
            { [2, 1], 1, 1, false },
            { [2, 2], 2, 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForTwoDifferentValuesOfATreeOfUniqueValues(int?[] levelOrder, int startValue, int destValue, bool expected)
        => Assert.Equal(expected, StepByStepDirectionsFromABinaryTreeNodeToAnotherPromise.IsKeptBy(LeetCodeWireFormat.ToBinaryTree(levelOrder), startValue, destValue));
}
