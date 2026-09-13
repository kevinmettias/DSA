using DSAExperimentation.LeetCode.DetectCyclesIn2DGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetectCyclesIn2DGrid;

// Harness only. Both strategies live in DetectCyclesIn2DGridSolution; this file
// pins them to LeetCode's published examples plus the smallest grids that can and
// cannot close a cycle, which is also what finally gets the parent-tracked DFS
// baseline - previously benchmark-only - under assertion.
public sealed class DetectCyclesIn2DGridTests
{
    public static TheoryData<char[][], bool> Examples =>
        new()
        {
            { Grid("aaaa", "abba", "abba", "aaaa"), true },
            { Grid("ccca", "cdcc", "ccec", "fccc"), true },
            { Grid("abb", "bzb", "bbb"), false },
            { Grid("aa", "aa"), true },
            { Grid("ab", "ba"), false },
            { Grid("a"), false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsCycleByParentTrackedDepthFirstSearch_LeetCodeExamples_ReportsWhetherACycleExists(
        char[][] grid, bool expected) =>
        Assert.Equal(
            expected,
            DetectCyclesIn2DGridSolution.ContainsCycleByParentTrackedDepthFirstSearch(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ContainsCycleByDisjointSetEdgeUnion_LeetCodeExamples_ReportsWhetherACycleExists(
        char[][] grid, bool expected) =>
        Assert.Equal(
            expected,
            DetectCyclesIn2DGridSolution.ContainsCycleByDisjointSetEdgeUnion(grid));

    private static char[][] Grid(params string[] rows) =>
        rows.Select(row => row.ToCharArray()).ToArray();
}
