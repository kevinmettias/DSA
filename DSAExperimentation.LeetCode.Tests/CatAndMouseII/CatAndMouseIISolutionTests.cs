using DSAExperimentation.LeetCode.CatAndMouseII;

namespace DSAExperimentation.LeetCode.Tests.CatAndMouseII;

// Harness only. Both strategies - the uncached minimax recursion and the same
// recurrence routed through Memoizer - are CatAndMouseIISolution's, so this file
// only pins them to LeetCode's published examples plus the wall, capture and
// jump-distance cases that separate a win from a loss.
public sealed partial class CatAndMouseIISolutionTests
{
    public static TheoryData<CanMouseWinExample> Examples =>
        new()
        {
            // LeetCode examples 2 and 3; example 1 is in MemoizedOnlyExamples below.
            { new CanMouseWinExample(Grid: ["M.C...F"], CatJump: 1, MouseJump: 4, Expected: true) },
            { new CanMouseWinExample(Grid: ["M.C...F"], CatJump: 1, MouseJump: 3, Expected: false) },

            // Boards of this file's own, Mouse moving first and either animal free to jump
            // over the other but not over a wall: Mouse steps straight onto the food; a
            // jump of 4 clears the cat to the food, a jump of 1 must stop beside it and is
            // caught; with the cat between them Mouse can only stay or step onto it; a wall
            // keeps the cat from the food Mouse reaches in two moves; the cat sits one step
            // from the food Mouse needs four moves to reach round a wall.
            { new CanMouseWinExample(Grid: ["C..", "...", ".MF"], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinExample(Grid: ["F.C.M"], CatJump: 1, MouseJump: 4, Expected: true) },
            { new CanMouseWinExample(Grid: ["F.C.M"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinExample(Grid: ["MCF"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinExample(Grid: ["M.F", "###", "C.."], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinExample(Grid: ["M#F", "..C"], CatJump: 1, MouseJump: 1, Expected: false) },
        };

    // LeetCode example 1, asserted on the memoized arm alone. Its 3x5 board caps the
    // game at 30 plies, and the exhaustive recursion re-derives every repeated
    // position to that depth with up to nine moves per ply, so it does not finish.
    public static TheoryData<CanMouseWinExample> MemoizedOnlyExamples =>
        new()
        {
            { new CanMouseWinExample(Grid: ["####F", "#C...", "M...."], CatJump: 1, MouseJump: 2, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMouseWinByExhaustiveRecursion_LeetCodeExamples_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinExample example)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByExhaustiveRecursion(
            example.Grid, example.CatJump, example.MouseJump);

        Assert.Equal(example.Expected, mouseWins);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    [MemberData(nameof(MemoizedOnlyExamples))]
    public void CanMouseWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinExample example)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByMemoizedRecursion(
            example.Grid, example.CatJump, example.MouseJump);

        Assert.Equal(example.Expected, mouseWins);
    }

    // One example: the board, each animal's jump distance, and whether the mouse
    // can force the food. The row names every position - a bare `bool` argument would read
    // as "true" and say nothing about what is true.
    public readonly record struct CanMouseWinExample(string[] Grid, int CatJump, int MouseJump, bool Expected);
}
