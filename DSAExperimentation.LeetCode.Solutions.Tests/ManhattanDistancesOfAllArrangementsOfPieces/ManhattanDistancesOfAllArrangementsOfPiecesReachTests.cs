using DSAExperimentation.LeetCode.Solutions.ManhattanDistancesOfAllArrangementsOfPieces;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ManhattanDistancesOfAllArrangementsOfPieces;

public sealed partial class ManhattanDistancesOfAllArrangementsOfPiecesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ManhattanDistancesOfAllArrangementsOfPiecesReach.IsWithin(1, 1, 4));

    [Fact]
    public void IsWithin_PieceCountPastTheReach_IsFalse() => Assert.False(ManhattanDistancesOfAllArrangementsOfPiecesReach.IsWithin(1, 1, 5));
}
