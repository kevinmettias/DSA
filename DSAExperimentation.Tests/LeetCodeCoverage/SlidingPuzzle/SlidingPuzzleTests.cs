using DSAExperimentation.LeetCode.SlidingPuzzle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SlidingPuzzle;

// Harness only. The board-permutation graph itself is Domain.SlidingPuzzle's and
// both search strategies are SlidingPuzzleSolution's - this file just pins them
// to LeetCode's published examples.
public sealed partial class SlidingPuzzleTests
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
}
