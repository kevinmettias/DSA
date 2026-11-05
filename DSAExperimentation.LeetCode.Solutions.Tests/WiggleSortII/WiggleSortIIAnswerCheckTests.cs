using DSAExperimentation.LeetCode.Solutions.WiggleSortII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WiggleSortII;

public sealed partial class WiggleSortIIAnswerCheckTests
{
    public static TheoryData<int[], int[], int[], bool> Answers =>
        new()
        {
            { [1, 5, 1, 1, 6, 4], [1, 6, 1, 5, 1, 4], [1, 6, 1, 5, 1, 4], true },
            { [1, 5, 1, 1, 6, 4], [1, 5, 1, 6, 1, 4], [1, 6, 1, 5, 1, 4], true },
            { [1, 3, 2, 2, 3, 1], [2, 3, 1, 3, 1, 2], [2, 3, 1, 3, 1, 2], true },
            { [1, 5, 1, 1, 6, 4], [1, 1, 5, 1, 6, 4], [1, 6, 1, 5, 1, 4], false },
            { [1, 5, 1, 1, 6, 4], [1, 6, 1, 5, 2, 4], [1, 6, 1, 5, 1, 4], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForTheValuesInAWiggle(int[] nums, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, WiggleSortIIAnswerCheck.IsAccepted(nums, answer, recorded));
}
