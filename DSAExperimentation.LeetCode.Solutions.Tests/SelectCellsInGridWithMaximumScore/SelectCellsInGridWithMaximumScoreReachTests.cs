using DSAExperimentation.LeetCode.Solutions.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SelectCellsInGridWithMaximumScore;

public sealed partial class SelectCellsInGridWithMaximumScoreReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SelectCellsInGridWithMaximumScoreReach.IsWithin([new int[45]]));

    [Fact]
    public void IsWithin_GridElementsPastTheReach_IsFalse() => Assert.False(SelectCellsInGridWithMaximumScoreReach.IsWithin([new int[46]]));
}
