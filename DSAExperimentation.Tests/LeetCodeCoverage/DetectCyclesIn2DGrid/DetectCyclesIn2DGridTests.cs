using DSAExperimentation.LeetCode.DetectCyclesIn2DGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetectCyclesIn2DGrid;

// Harness only. Both strategies live in DetectCyclesIn2DGridSolution; this file
// pins them to LeetCode's published examples plus the smallest grids that can and
// cannot close a cycle, which is also what finally gets the parent-tracked DFS
// baseline - previously benchmark-only - under assertion.
public sealed partial class DetectCyclesIn2DGridTests
{
    public static TheoryData<CycleGridCase> Examples =>
        new()
        {
            { new CycleGridCase(Grid("aaaa", "abba", "abba", "aaaa"), HasCycle: true) },
            { new CycleGridCase(Grid("ccca", "cdcc", "ccec", "fccc"), HasCycle: true) },
            { new CycleGridCase(Grid("abb", "bzb", "bbb"), HasCycle: false) },
            { new CycleGridCase(Grid("aa", "aa"), HasCycle: true) },
            { new CycleGridCase(Grid("ab", "ba"), HasCycle: false) },
            { new CycleGridCase(Grid("a"), HasCycle: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByParentTrackedDepthFirstSearch_LeetCodeExamples_ReportsWhetherACycleExists(
        CycleGridCase example) =>
        Assert.Equal(
            example.HasCycle,
            DetectCyclesIn2DGridSolution.HasCycleByParentTrackedDepthFirstSearch(example.Grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByDisjointSetEdgeUnion_LeetCodeExamples_ReportsWhetherACycleExists(
        CycleGridCase example) =>
        Assert.Equal(
            example.HasCycle,
            DetectCyclesIn2DGridSolution.HasCycleByDisjointSetEdgeUnion(example.Grid));

    private static char[][] Grid(params string[] rows) =>
        rows.Select(row => row.ToCharArray()).ToArray();

    // One LeetCode example: the grid under test and whether a cycle closes in it. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct CycleGridCase(char[][] Grid, bool HasCycle);
}
