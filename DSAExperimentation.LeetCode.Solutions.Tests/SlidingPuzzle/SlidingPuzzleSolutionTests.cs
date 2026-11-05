using DSAExperimentation.LeetCode.Solutions.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SlidingPuzzle;

// Harness only. The puzzle itself is Domain/Games/SlidingPuzzle's, on this folder's
// TwoByThreeBoard, and both search strategies are SlidingPuzzleSolution's - this file just pins them
// to a board that is already solved, which takes no slides. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class SlidingPuzzleSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 0 } }, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByMutationQueue_Cases_ReturnsFewestSlidesOrMinusOne(
        int[][] board, int expected) =>
        Assert.Equal(expected, SlidingPuzzleSolution.MinMovesByMutationQueue(board));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinMovesByReduceGraph_Cases_ReturnsFewestSlidesOrMinusOne(
        int[][] board, int expected) =>
        Assert.Equal(expected, SlidingPuzzleSolution.MinMovesByReduceGraph(board));

    // Every arrangement of the 2x3 board, each joined to the two or three one slide away.
    [Fact]
    public void BuildPuzzleGraph_HoldsEveryArrangementOfTheBoard()
    {
        var graph = SlidingPuzzleSolution.BuildPuzzleGraph();

        Assert.Equal(720, graph.Count);
        Assert.True(graph.TryGetNode("123450", out var solved));
        Assert.Equal(["120453", "123405"], solved.Successors.Select(successor => successor.State));
    }
}
