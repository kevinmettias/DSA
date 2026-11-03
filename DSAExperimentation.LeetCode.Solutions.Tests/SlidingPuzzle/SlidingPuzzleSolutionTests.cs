using DSAExperimentation.LeetCode.Solutions.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SlidingPuzzle;

// Harness only. The puzzle itself is Domain/Games/SlidingPuzzle's, on this folder's
// TwoByThreeBoard, and both search strategies are SlidingPuzzleSolution's - this file just pins them
// to LeetCode's published examples.
public sealed partial class SlidingPuzzleSolutionTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { new[] { new[] { 1, 2, 3 }, new[] { 4, 0, 5 } }, 1 },
            { new[] { new[] { 1, 2, 3 }, new[] { 5, 4, 0 } }, -1 },
            { new[] { new[] { 4, 1, 2 }, new[] { 5, 0, 3 } }, 5 },
            { new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 0 } }, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByMutationQueue_LeetCodeExamples_ReturnsFewestSlidesOrMinusOne(
        int[][] board, int expected) =>
        Assert.Equal(expected, SlidingPuzzleSolution.MinMovesByMutationQueue(board));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMovesByReduceGraph_LeetCodeExamples_ReturnsFewestSlidesOrMinusOne(
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
