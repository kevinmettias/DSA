using DSAExperimentation.LeetCode.NumberOfIslands;

namespace DSAExperimentation.LeetCode.Tests.NumberOfIslands;

// Harness only. Both flood-fill strategies are NumberOfIslandsSolution's; this file
// just pins them to LeetCode's published examples plus the original coverage grid.
// Neither arm mutates the grid, so the same grid can be handed to both.
public sealed partial class NumberOfIslandsSolutionTests
{
    public static TheoryData<char[][], int> Examples =>
        new()
        {
            {
                new[]
                {
                    new[] { '1', '1', '0' },
                    new[] { '1', '0', '0' },
                    new[] { '0', '0', '1' },
                },
                2
            },
            {
                new[]
                {
                    new[] { '1', '1', '1', '1', '0' },
                    new[] { '1', '1', '0', '1', '0' },
                    new[] { '1', '1', '0', '0', '0' },
                    new[] { '0', '0', '0', '0', '0' },
                },
                1
            },
            {
                new[]
                {
                    new[] { '1', '1', '0', '0', '0' },
                    new[] { '1', '1', '0', '0', '0' },
                    new[] { '0', '0', '1', '0', '0' },
                    new[] { '0', '0', '0', '1', '1' },
                },
                3
            },
            { new[] { new[] { '0' } }, 0 },
            { new[] { new[] { '1', '1', '1' } }, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountIslandsByDepthFirstSink_LeetCodeExamples_ReturnsComponentCount(
        char[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfIslandsSolution.CountIslandsByDepthFirstSink(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountIslandsByBreadthFirstSink_LeetCodeExamples_ReturnsComponentCount(
        char[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfIslandsSolution.CountIslandsByBreadthFirstSink(grid));
}
