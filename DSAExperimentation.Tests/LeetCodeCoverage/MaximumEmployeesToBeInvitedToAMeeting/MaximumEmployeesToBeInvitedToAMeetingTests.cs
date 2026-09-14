using DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumEmployeesToBeInvitedToAMeeting;

// Harness only. Both strategies are
// MaximumEmployeesToBeInvitedToAMeetingSolution's - including the manual peel,
// which the benchmark used to own privately as its baseline arm and nothing
// asserted. EmployeeNode/EmployeeTopology moved beside the solution, so this
// folder no longer carries a Fixtures/ subfolder.
public sealed class MaximumEmployeesToBeInvitedToAMeetingTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LC example 1: employees 1 and 2 favor each other, with 0 and 3 both
            // chaining into 2 - one of them joins the pair.
            { [2, 2, 1, 2], 3 },

            // LC example 2: a single 3-cycle fills the table.
            { [1, 2, 0], 3 },

            // LC example 3: a single 4-cycle.
            { [3, 0, 1, 2], 4 },

            // Two mutual pairs and no chains: both pairs share one table.
            { [1, 0, 3, 2], 4 },

            // One mutual pair with a one-employee chain hanging off each member.
            { [1, 0, 0, 1], 4 },

            // A mutual pair with no chains against a 3-cycle: the longer cycle wins.
            { [1, 0, 3, 4, 2], 3 },

            // A mutual pair with a two-employee chain on each side (6) beats the
            // separate 3-cycle it shares the input with.
            { [1, 0, 0, 1, 2, 3, 7, 8, 6], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumInvitedByManualPeelAndCycleWalk_LeetCodeExamples_ReturnsLargestSeatableGroup(
        int[] favorite, int expected) =>
        Assert.Equal(
            expected,
            MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByManualPeelAndCycleWalk(favorite));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumInvitedByGraphPrimitiveComposition_LeetCodeExamples_ReturnsLargestSeatableGroup(
        int[] favorite, int expected) =>
        Assert.Equal(
            expected,
            MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByGraphPrimitiveComposition(favorite));
}
