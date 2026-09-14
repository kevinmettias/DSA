using DSAExperimentation.LeetCode.CatAndMouseII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CatAndMouseII;

// Harness only. Both strategies - the uncached minimax recursion and the same
// recurrence routed through Memoizer - are CatAndMouseIISolution's, so this file
// only pins them to LeetCode's published examples plus the wall, capture and
// jump-distance cases that separate a win from a loss.
public sealed class CatAndMouseIITests
{
    public static TheoryData<string[], int, int, bool> Examples =>
        new()
        {
            { ["C..", "...", ".MF"], 1, 1, true },
            { ["F.C.M"], 1, 4, true },
            { ["F.C.M"], 1, 1, false },
            { ["M.C...F"], 1, 4, true },
            { ["M.C...F"], 1, 3, false },
            { ["MCF"], 1, 1, false },
            { ["M.F", "###", "C.."], 1, 1, true },
            { ["M#F", "..C"], 1, 1, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMouseWinByExhaustiveRecursion_LeetCodeExamples_ReturnsWhetherMouseForcesTheFood(
        string[] grid, int catJump, int mouseJump, bool expected) =>
        Assert.Equal(expected, CatAndMouseIISolution.CanMouseWinByExhaustiveRecursion(grid, catJump, mouseJump));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMouseWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherMouseForcesTheFood(
        string[] grid, int catJump, int mouseJump, bool expected) =>
        Assert.Equal(expected, CatAndMouseIISolution.CanMouseWinByMemoizedRecursion(grid, catJump, mouseJump));
}
