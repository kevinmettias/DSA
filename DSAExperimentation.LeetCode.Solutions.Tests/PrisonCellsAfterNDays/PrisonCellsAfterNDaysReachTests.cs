using DSAExperimentation.LeetCode.Solutions.PrisonCellsAfterNDays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrisonCellsAfterNDays;

public sealed partial class PrisonCellsAfterNDaysReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PrisonCellsAfterNDaysReach.IsWithin(new int[0], 1_000_000));

    [Fact]
    public void IsWithin_DayCountPastTheReach_IsFalse() => Assert.False(PrisonCellsAfterNDaysReach.IsWithin(new int[0], 1_000_001));
}
