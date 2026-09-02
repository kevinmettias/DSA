using DSAExperimentation.LeetCode.NumberOfIslands;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfIslands;

// Harness only. The flood-fill count is NumberOfIslandsSolution's; this file
// just pins it to LeetCode's published examples plus the original coverage grid.
public sealed class NumberOfIslandsTests
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
}
