using DSAExperimentation.LeetCode.AvailableCapturesForRook;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AvailableCapturesForRook;

// Harness only. Both strategies are AvailableCapturesForRookSolution's - this file
// pins them to LeetCode's published examples, given as one string per rank
// (ParseBoard widens them into the char[][] the problem's own signature takes).
public sealed class AvailableCapturesForRookTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            // Two pawns reachable straight up and straight down, one more on the
            // rook's own rank past the edge of nothing at all - three captures.
            {
                [
                    "........",
                    "...p....",
                    "...R...p",
                    "........",
                    "........",
                    "...p....",
                    "........",
                    "........",
                ],
                3
            },

            // A bishop on every side: each ray stops on a blocker, so no capture.
            {
                [
                    "........",
                    ".ppppp..",
                    ".ppBpp..",
                    ".pBRBp..",
                    ".ppBpp..",
                    ".ppppp..",
                    "........",
                    "........",
                ],
                0
            },

            // Mixed: up and both sides reach a pawn (the nearer pawn shadows the
            // farther one on the left rank), while a bishop blocks the ray down.
            {
                [
                    "........",
                    "...p....",
                    "...p....",
                    "pp.R.pB.",
                    "........",
                    "...B....",
                    "...p....",
                    "........",
                ],
                3
            },

            // Nothing but the rook: every ray runs off the edge.
            {
                [
                    "........",
                    "........",
                    "........",
                    "...R....",
                    "........",
                    "........",
                    "........",
                    "........",
                ],
                0
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumRookCapturesByFullBoardScan_LeetCodeExamples_ReturnsCapturablePawnCount(
        string[] rows, int expected) =>
        Assert.Equal(expected, AvailableCapturesForRookSolution.NumRookCapturesByFullBoardScan(ParseBoard(rows)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumRookCapturesByRayWalk_LeetCodeExamples_ReturnsCapturablePawnCount(
        string[] rows, int expected) =>
        Assert.Equal(expected, AvailableCapturesForRookSolution.NumRookCapturesByRayWalk(ParseBoard(rows)));

    private static char[][] ParseBoard(string[] rows) => rows.Select(r => r.ToCharArray()).ToArray();
}
