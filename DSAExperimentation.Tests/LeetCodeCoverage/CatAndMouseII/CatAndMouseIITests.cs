using DSAExperimentation.LeetCode.CatAndMouseII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CatAndMouseII;

// Harness only. Both strategies - the uncached minimax recursion and the same
// recurrence routed through Memoizer - are CatAndMouseIISolution's, so this file
// only pins them to LeetCode's published examples plus the wall, capture and
// jump-distance cases that separate a win from a loss.
public sealed class CatAndMouseIITests
{
    public static TheoryData<CanMouseWinExample> Examples =>
        new()
        {
            { new CanMouseWinExample(Grid: ["C..", "...", ".MF"], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinExample(Grid: ["F.C.M"], CatJump: 1, MouseJump: 4, Expected: true) },
            { new CanMouseWinExample(Grid: ["F.C.M"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinExample(Grid: ["M.C...F"], CatJump: 1, MouseJump: 4, Expected: true) },
            { new CanMouseWinExample(Grid: ["M.C...F"], CatJump: 1, MouseJump: 3, Expected: false) },
            { new CanMouseWinExample(Grid: ["MCF"], CatJump: 1, MouseJump: 1, Expected: false) },
            { new CanMouseWinExample(Grid: ["M.F", "###", "C.."], CatJump: 1, MouseJump: 1, Expected: true) },
            { new CanMouseWinExample(Grid: ["M#F", "..C"], CatJump: 1, MouseJump: 1, Expected: false) },
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
    public void CanMouseWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherMouseForcesTheFood(
        CanMouseWinExample example)
    {
        var mouseWins = CatAndMouseIISolution.CanMouseWinByMemoizedRecursion(
            example.Grid, example.CatJump, example.MouseJump);

        Assert.Equal(example.Expected, mouseWins);
    }

    // One LeetCode example: the board, each animal's jump distance, and whether the mouse
    // can force the food. The row names every position - a bare `bool` argument would read
    // as "true" and say nothing about what is true.
    public readonly record struct CanMouseWinExample(string[] Grid, int CatJump, int MouseJump, bool Expected);
}
