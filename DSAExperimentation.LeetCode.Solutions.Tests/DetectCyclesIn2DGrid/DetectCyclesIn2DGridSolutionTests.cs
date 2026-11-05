using DSAExperimentation.LeetCode.Solutions.DetectCyclesIn2DGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DetectCyclesIn2DGrid;

// Harness only. Both strategies live in DetectCyclesIn2DGridSolution; this file
// pins them to the smallest grids that can and
// cannot close a cycle, which is also what finally gets the parent-tracked DFS
// baseline - previously benchmark-only - under assertion.
public sealed partial class DetectCyclesIn2DGridSolutionTests
{
    public static TheoryData<CycleGridCase> Cases =>
        new()
        {
            { new CycleGridCase(Grid("abb", "bzb", "bbb"), HasCycle: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasCycleByParentTrackedDepthFirstSearch_Cases_ReportsWhetherACycleExists(
        CycleGridCase testCase) =>
        Assert.Equal(
            testCase.HasCycle,
            DetectCyclesIn2DGridSolution.HasCycleByParentTrackedDepthFirstSearch(testCase.Grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasCycleByDisjointSetEdgeUnion_Cases_ReportsWhetherACycleExists(
        CycleGridCase testCase) =>
        Assert.Equal(
            testCase.HasCycle,
            DetectCyclesIn2DGridSolution.HasCycleByDisjointSetEdgeUnion(testCase.Grid));

    private static char[][] Grid(params string[] rows) =>
        rows.Select(row => row.ToCharArray()).ToArray();

    // One case: the grid under test and whether a cycle closes in it. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct CycleGridCase(char[][] Grid, bool HasCycle);
}
