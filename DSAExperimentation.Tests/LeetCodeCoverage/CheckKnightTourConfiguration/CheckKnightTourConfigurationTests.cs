using DSAExperimentation.LeetCode.CheckKnightTourConfiguration;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckKnightTourConfiguration;

// Harness only. Both strategies are CheckKnightTourConfigurationSolution's - the
// O(n^4) board rescan the benchmark's baseline arm used to own and nothing
// asserted, and the single-pass move -> cell inversion this file used to inline.
//
// The "valid moves but the knight starts elsewhere" case is new: LC 2596 requires
// the tour to begin at the top-left cell, and neither of the two inlined copies
// checked it.
public sealed class CheckKnightTourConfigurationTests
{
    public static TheoryData<KnightTourCase> Examples =>
        new()
        {
            // LC example 1: a genuine tour of a 5 x 5 board.
            {
                new KnightTourCase(
                    [
                        [0, 11, 16, 5, 20],
                        [17, 4, 19, 10, 15],
                        [12, 1, 8, 21, 6],
                        [3, 18, 23, 14, 9],
                        [24, 13, 2, 7, 22],
                    ],
                    Expected: true)
            },

            // LC example 2: the 8th move is not a knight's move.
            {
                new KnightTourCase(
                    [
                        [0, 3, 6],
                        [5, 8, 1],
                        [2, 7, 4],
                    ],
                    Expected: false)
            },

            // The whole board is the starting cell, so there is no move to check.
            { new KnightTourCase([[0]], Expected: true) },

            // A board too small for any knight's move at all.
            {
                new KnightTourCase(
                    [
                        [0, 3],
                        [2, 1],
                    ],
                    Expected: false)
            },

            // Row-major order: the very first step is a single square right.
            {
                new KnightTourCase(
                    [
                        [0, 1, 2],
                        [3, 4, 5],
                        [6, 7, 8],
                    ],
                    Expected: false)
            },

            // Another genuine 5 x 5 tour.
            {
                new KnightTourCase(
                    [
                        [0, 13, 18, 7, 24],
                        [5, 8, 1, 12, 17],
                        [14, 19, 6, 23, 2],
                        [9, 4, 21, 16, 11],
                        [20, 15, 10, 3, 22],
                    ],
                    Expected: true)
            },

            // The same tour with moves 7 and 24 swapped, which breaks the walk at
            // move 6 - late enough that an early-exiting scan still does real work.
            {
                new KnightTourCase(
                    [
                        [0, 13, 18, 24, 7],
                        [5, 8, 1, 12, 17],
                        [14, 19, 6, 23, 2],
                        [9, 4, 21, 16, 11],
                        [20, 15, 10, 3, 22],
                    ],
                    Expected: false)
            },

            // Every consecutive pair IS a knight's move, but move 0 sits at (3, 3)
            // rather than the top-left cell LC 2596 requires the knight to start on.
            {
                new KnightTourCase(
                    [
                        [20, 3, 12, 9, 22],
                        [13, 8, 21, 4, 11],
                        [2, 19, 10, 23, 16],
                        [7, 14, 17, 0, 5],
                        [18, 1, 6, 15, 24],
                    ],
                    Expected: false)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckValidGridByBoardRescan_LeetCodeExamples_ReturnsWhetherTheTourIsValid(
        KnightTourCase example) =>
        Assert.Equal(
            example.Expected,
            CheckKnightTourConfigurationSolution.CheckValidGridByBoardRescan(example.Grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckValidGridByPositionLookup_LeetCodeExamples_ReturnsWhetherTheTourIsValid(
        KnightTourCase example) =>
        Assert.Equal(
            example.Expected,
            CheckKnightTourConfigurationSolution.CheckValidGridByPositionLookup(example.Grid));

    // One LeetCode example: the board and whether it holds a valid knight's tour. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct KnightTourCase(int[][] Grid, bool Expected);
}
