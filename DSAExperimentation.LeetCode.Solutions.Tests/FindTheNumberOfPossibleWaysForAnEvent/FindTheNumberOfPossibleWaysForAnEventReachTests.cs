using DSAExperimentation.LeetCode.Solutions.FindTheNumberOfPossibleWaysForAnEvent;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheNumberOfPossibleWaysForAnEvent;

public sealed partial class FindTheNumberOfPossibleWaysForAnEventReachTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 2, 1000, true },
            { 1000, 1, true },
            { 6, 10, true },
            { 7, 10, false },
            { 10, 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsWithin_Input_HoldsForAtMostAMillionStageAssignments(int performerCount, int stageCount, bool expected)
        => Assert.Equal(expected, FindTheNumberOfPossibleWaysForAnEventReach.IsWithin(performerCount, stageCount, 1));
}
