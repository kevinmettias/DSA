using DSAExperimentation.LeetCode.SellingPiecesOfWood;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SellingPiecesOfWood;

// Harness only. Both strategies are SellingPiecesOfWoodSolution's, and pinning them
// to the same examples is what finally puts the un-memoized recursion - previously a
// benchmark arm asserted by nothing - under test beside the Memoizer-cached
// recurrence it is measured against. Every board here is small enough that the
// baseline's exponential branching stays trivial.
public sealed class SellingPiecesOfWoodTests
{
    public static TheoryData<int, int, (int Height, int Width, int Price)[], long> Examples =>
        new()
        {
            // LeetCode example 1: a 3 x 5 board, best cut into a 2 x 2 piece (7), a
            // second 2 x 2 piece (7), a 1 x 4 piece (2) and a 2 x 1 piece (3).
            { 3, 5, [(1, 4, 2), (2, 2, 7), (2, 1, 3)], 19L },

            // LeetCode example 2: a 4 x 6 board, best cut into three 3 x 2 pieces
            // (10 each) and a 1 x 4 piece (2).
            { 4, 6, [(3, 2, 10), (1, 4, 2), (4, 1, 3)], 32L },

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
    [MemberData(nameof(Examples))]
    public void SellingWoodByUnmemoizedRecursion_LeetCodeExamples_ReturnsMostMoneyEarnable(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices, long expected) =>
        Assert.Equal(
            expected,
            SellingPiecesOfWoodSolution.SellingWoodByUnmemoizedRecursion(boardHeight, boardWidth, prices));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SellingWoodByMemoizedRecursion_LeetCodeExamples_ReturnsMostMoneyEarnable(
        int boardHeight, int boardWidth, (int Height, int Width, int Price)[] prices, long expected) =>
        Assert.Equal(
            expected,
            SellingPiecesOfWoodSolution.SellingWoodByMemoizedRecursion(boardHeight, boardWidth, prices));
}
