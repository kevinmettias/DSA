using DSAExperimentation.LeetCode.Solutions.NumberOfIslands;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfIslands;

// Harness only. Both strategies are NumberOfIslandsSolution's; this file
// just pins them to the original coverage grid.
// Neither arm mutates the grid, so the same grid can be handed to both.
public sealed partial class NumberOfIslandsSolutionTests
{
    public static TheoryData<char[][], int> Cases =>
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
            { new[] { new[] { '0' } }, 0 },
            { new[] { new[] { '1', '1', '1' } }, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountIslandsByConnectedComponents_Cases_ReturnsComponentCount(
        char[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfIslandsSolution.CountIslandsByConnectedComponents(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountIslandsByBreadthFirstSink_Cases_ReturnsComponentCount(
        char[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfIslandsSolution.CountIslandsByBreadthFirstSink(grid));
}
