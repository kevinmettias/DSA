using DSAExperimentation.LeetCode.Solutions.MaximumEmployeesToBeInvitedToAMeeting;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumEmployeesToBeInvitedToAMeeting;

// Harness only. Both strategies are
// MaximumEmployeesToBeInvitedToAMeetingSolution's - including the manual peel,
// which the benchmark used to own privately as its baseline arm and nothing
// asserted. EmployeeNode/EmployeeTopology moved beside the solution, so this
// folder no longer carries a Fixtures/ subfolder.
public sealed partial class MaximumEmployeesToBeInvitedToAMeetingSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // A single 4-cycle.
            { [3, 0, 1, 2], 4 },

            // One mutual pair with a one-employee chain hanging off each member.
            { [1, 0, 0, 1], 4 },

            // A mutual pair with a two-employee chain on each side (6) beats the
            // separate 3-cycle it shares the input with.
            { [1, 0, 0, 1, 2, 3, 7, 8, 6], 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumInvitedByManualPeelAndCycleWalk_Cases_ReturnsLargestSeatableGroup(
        int[] favorite, int expected) =>
        Assert.Equal(
            expected,
            MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByManualPeelAndCycleWalk(favorite));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumInvitedByGraphPrimitiveComposition_Cases_ReturnsLargestSeatableGroup(
        int[] favorite, int expected) =>
        Assert.Equal(
            expected,
            MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByGraphPrimitiveComposition(favorite));
}
