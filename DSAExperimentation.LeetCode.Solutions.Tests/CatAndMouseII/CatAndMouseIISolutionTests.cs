using DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CatAndMouseII;

// Harness only. Every strategy - the uncached minimax recursion, the same
// recurrence routed through Memoizer, and retrograde analysis over the game graph -
// is CatAndMouseIISolution's, so this file only pins them to the wall, capture and
// jump-distance cases that separate a win from a loss. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class CatAndMouseIISolutionTests
{
    public static TheoryData<CanMouseWinCase> Cases =>
        new()
        {
            // Boards of this file's own, Mouse moving first and either animal free to jump
            // over the other but not over a wall: Mouse steps straight onto the food; a
            // jump of 4 clears the cat to the food, a jump of 1 must stop beside it and is
            // caught; with the cat between them Mouse can only stay or step onto it; a wall
            // keeps the cat from the food Mouse reaches in two moves; the cat sits one step
            // from the food Mouse needs four moves to reach round a wall; and walls shut both
            // animals in their corners with the food a diagonal step from each, which neither
            // may take, since moves run along rows and columns only.
            { new CanMouseWinCase(Grid: ["C..", "...", ".MF"], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinCase(Grid: ["F.C.M"], CatJump: 1, MouseJump: 4, Expected: true) },
            { new CanMouseWinCase(Grid: ["F.C.M"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinCase(Grid: ["MCF"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinCase(Grid: ["M.F", "###", "C.."], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinCase(Grid: ["M#F", "..C"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinCase(Grid: ["M#C", "#F#"], CatJump: 1, MouseJump: 1, Expected: false) },
        };

    // LeetCode example 1, asserted on the memoized arm alone. Its 3x5 board caps the
    // game at 30 plies, and the exhaustive recursion re-derives every repeated
    // position to that depth with up to nine moves per ply, so it does not finish.
    public static TheoryData<CanMouseWinCase> MemoizedOnlyExamples =>
        new()
        {
            { new CanMouseWinCase(Grid: ["####F", "#C...", "M...."], CatJump: 1, MouseJump: 2, Expected: true) },
        };

    // Boards up to LeetCode's 8 x 8 from the catalog's cases, asserted on retrograde
    // analysis alone: the problem's reach keeps every catalog case past a single row
    // from the catalog harness, since the exhaustive recursion cannot finish them. The
    // fourth is a draw - the food is walled off from both animals and the cat cannot
    // corner the mouse - and a draw is the cat's.
    public static TheoryData<CanMouseWinCase> RetrogradeOnlyBoards =>
        new()
        {
            {
                new CanMouseWinCase(
                    Grid: ["M......", ".......", ".....#C", "......#",
                        ".......", "#F.....", "......."],
                    CatJump: 3, MouseJump: 3, Expected: true)
            },
            {
                new CanMouseWinCase(
                    Grid: [".......", ".......", ".......", "...C...",
                        "...M...", ".......", "...F..."],
                    CatJump: 5, MouseJump: 5, Expected: true)
            },
            {
                new CanMouseWinCase(
                    Grid: ["M.......", "........", ".####...", ".....#..",
                        "......#F", "........", "........", ".....C.."],
                    CatJump: 2, MouseJump: 1, Expected: false)
            },
            {
                new CanMouseWinCase(
                    Grid: ["M....", ".....", "...C.", "#####", "....F"],
                    CatJump: 2, MouseJump: 1, Expected: false)
            },
            {
                new CanMouseWinCase(
                    Grid: ["M.......", "........", "........", "...F....",
                        "........", "........", "........", ".......C"],
                    CatJump: 6, MouseJump: 2, Expected: false)
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanMouseWinByExhaustiveRecursion_Cases_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinCase testCase)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByExhaustiveRecursion(
            testCase.Grid, testCase.CatJump, testCase.MouseJump);

        Assert.Equal(testCase.Expected, mouseWins);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    [MemberData(nameof(MemoizedOnlyExamples))]
    public void CanMouseWinByMemoizedRecursion_Cases_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinCase testCase)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByMemoizedRecursion(
            testCase.Grid, testCase.CatJump, testCase.MouseJump);

        Assert.Equal(testCase.Expected, mouseWins);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    [MemberData(nameof(MemoizedOnlyExamples))]
    [MemberData(nameof(RetrogradeOnlyBoards))]
    public void CanMouseWinByRetrogradeAnalysis_Cases_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinCase testCase)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByRetrogradeAnalysis(
            testCase.Grid, testCase.CatJump, testCase.MouseJump);

        Assert.Equal(testCase.Expected, mouseWins);
    }

    // One case: the board, each animal's jump distance, and whether the mouse
    // can force the food. The row names every position - a bare `bool` argument would read
    // as "true" and say nothing about what is true.
    public readonly record struct CanMouseWinCase(string[] Grid, int CatJump, int MouseJump, bool Expected);
}
