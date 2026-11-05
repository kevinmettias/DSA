using DSAExperimentation.LeetCode.Solutions.NumberOfValidMoveCombinationsOnChessboard;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfValidMoveCombinationsOnChessboard;

public sealed partial class NumberOfValidMoveCombinationsOnChessboardReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfValidMoveCombinationsOnChessboardReach.IsWithin(new string[4], Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));

    [Fact]
    public void IsWithin_PiecesLengthPastTheReach_IsFalse() => Assert.False(NumberOfValidMoveCombinationsOnChessboardReach.IsWithin(new string[5], Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));
}
