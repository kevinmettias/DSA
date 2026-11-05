using DSAExperimentation.LeetCode.Solutions.SubarrayWithElementsGreaterThanVaryingThreshold;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubarrayWithElementsGreaterThanVaryingThreshold;

public sealed partial class SubarrayWithElementsGreaterThanVaryingThresholdAnswerCheckTests
{
    public static TheoryData<int[], int, int, int, bool> Answers =>
        new()
        {
            { [1, 3, 4, 3, 1], 6, 3, 3, true },
            { [6, 5, 6, 5, 8], 7, 2, 1, true },
            { [1, 1], 10, -1, -1, true },
            { [1, 3, 4, 3, 1], 6, 2, 3, false },
            { [6, 5, 6, 5, 8], 7, -1, 1, false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForTheSizeOfAQualifyingSubarray(int[] nums, int threshold, int answer, int recorded, bool expected)
        => Assert.Equal(expected, SubarrayWithElementsGreaterThanVaryingThresholdAnswerCheck.IsAccepted(nums, threshold, answer, recorded));
}
