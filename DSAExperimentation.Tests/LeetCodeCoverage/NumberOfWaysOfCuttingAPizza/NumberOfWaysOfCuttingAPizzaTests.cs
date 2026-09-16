using DSAExperimentation.LeetCode.NumberOfWaysOfCuttingAPizza;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysOfCuttingAPizza;

// Harness only. Both strategies are NumberOfWaysOfCuttingAPizzaSolution's - this
// file pins them to LeetCode's published examples plus the two edge cases the
// original test carried: a single piece, which never cuts at all, and a pizza with
// fewer apples than requested pieces, which no arrangement of cuts can satisfy.
public sealed partial class NumberOfWaysOfCuttingAPizzaTests
{
    public static TheoryData<string[], int, int> Examples =>
        new()
        {
            { ["A..", "AAA", "..."], 3, 3 },
            { ["A..", "AA.", "..."], 3, 1 },
            { ["A..", "A..", "..."], 1, 1 },
            { ["A..", "AAA", "..."], 1, 1 },
            { ["..", "AA"], 3, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByUnmemoizedRecursion_LeetCodeExamples_ReturnsWaysEveryPieceKeepsAnApple(
        string[] pizza, int pieceCount, int expected)
    {
        var actual = NumberOfWaysOfCuttingAPizzaSolution.CountWaysByUnmemoizedRecursion(pizza, pieceCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByMemoizedRecursion_LeetCodeExamples_ReturnsWaysEveryPieceKeepsAnApple(
        string[] pizza, int pieceCount, int expected)
    {
        var actual = NumberOfWaysOfCuttingAPizzaSolution.CountWaysByMemoizedRecursion(pizza, pieceCount);

        Assert.Equal(expected, actual);
    }
}
