using DSAExperimentation.LeetCode.Solutions.MaximumEmployeesToBeInvitedToAMeeting;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumEmployeesToBeInvitedToAMeeting;

public sealed partial class MaximumEmployeesToBeInvitedToAMeetingPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 2, 1, 2], true },
            { [1, 2, 0], true },
            { [10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoEmployeeFavoursThemself(int[] favorite, bool expected)
        => Assert.Equal(expected, MaximumEmployeesToBeInvitedToAMeetingPromise.IsKeptBy(favorite));
}
