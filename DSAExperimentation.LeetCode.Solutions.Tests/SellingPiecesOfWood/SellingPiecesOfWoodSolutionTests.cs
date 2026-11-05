using DSAExperimentation.LeetCode.Solutions.SellingPiecesOfWood;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SellingPiecesOfWood;

// Harness only. Both strategies are SellingPiecesOfWoodSolution's, and pinning them
// to the same cases is what finally puts the un-memoized recursion - previously a
// benchmark arm asserted by nothing - under test beside the Memoizer-cached
// recurrence it is measured against. Every board here is small enough that the
// baseline's exponential branching stays trivial.
public sealed partial class SellingPiecesOfWoodSolutionTests
{
    public static TheoryData<int, int, (int Height, int Width, int Price)[], long> Cases =>
        new()
        {
            // Nothing is listed, so no cut of the board is worth anything.
            { 2, 2, [], 0L },

            // The whole board is itself the only listed piece.
            { 1, 1, [(1, 1, 5)], 5L },

            // Cutting beats selling whole: four unit squares at 1 each outsell the
            // 2 x 2 board's own listed price of 3.
            { 2, 2, [(2, 2, 3), (1, 1, 1)], 4L },

            // Only unit squares are listed, so the answer is the board's area.
            { 2, 3, [(1, 1, 1)], 6L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SellingWoodByUnmemoizedRecursion_Cases_ReturnsMostMoneyEarnable(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices, long expected)
    {
        var actual = SellingPiecesOfWoodSolution.SellingWoodByUnmemoizedRecursion(boardHeight, boardWidth, prices);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SellingWoodByMemoizedRecursion_Cases_ReturnsMostMoneyEarnable(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices, long expected)
    {
        var actual = SellingPiecesOfWoodSolution.SellingWoodByMemoizedRecursion(boardHeight, boardWidth, prices);

        Assert.Equal(expected, actual);
    }
}
