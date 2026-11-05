using DSAExperimentation.LeetCode.Solutions.SellingPiecesOfWood;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SellingPiecesOfWood;

public sealed partial class SellingPiecesOfWoodReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SellingPiecesOfWoodReach.IsWithin(5, 1, new ValueTuple<int, int, int>[0]));

    [Fact]
    public void IsWithin_BoardHeightPastTheReach_IsFalse() => Assert.False(SellingPiecesOfWoodReach.IsWithin(6, 1, new ValueTuple<int, int, int>[0]));
}
