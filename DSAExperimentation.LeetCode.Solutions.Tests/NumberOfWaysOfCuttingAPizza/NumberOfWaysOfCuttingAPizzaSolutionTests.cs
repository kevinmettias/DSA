using DSAExperimentation.LeetCode.Solutions.NumberOfWaysOfCuttingAPizza;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysOfCuttingAPizza;

// Harness only. Both strategies are NumberOfWaysOfCuttingAPizzaSolution's - this
// file pins them to the two edge cases the
// original test carried: a single piece, which never cuts at all, and a pizza with
// fewer apples than requested pieces, which no arrangement of cuts can satisfy.
public sealed partial class NumberOfWaysOfCuttingAPizzaSolutionTests
{
    public static TheoryData<string[], int, int> Cases =>
        new()
        {
            { ["A..", "AAA", "..."], 1, 1 },
            { ["..", "AA"], 3, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByUnmemoizedRecursion_Cases_ReturnsWaysEveryPieceKeepsAnApple(
        string[] pizza, int pieceCount, int expected)
    {
        var actual = NumberOfWaysOfCuttingAPizzaSolution.CountWaysByUnmemoizedRecursion(pizza, pieceCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWaysByMemoizedRecursion_Cases_ReturnsWaysEveryPieceKeepsAnApple(
        string[] pizza, int pieceCount, int expected)
    {
        var actual = NumberOfWaysOfCuttingAPizzaSolution.CountWaysByMemoizedRecursion(pizza, pieceCount);

        Assert.Equal(expected, actual);
    }
}
