using DSAExperimentation.LeetCode.PaintingAGridWithThreeDifferentColors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PaintingAGridWithThreeDifferentColors;

// Harness only. Both strategies live in
// PaintingAGridWithThreeDifferentColorsSolution; this file pins them to the same
// examples, so a failure names the strategy that broke. The brute force was
// previously only a [Benchmark(Baseline = true)] arm with nothing asserting it -
// these are its first assertions.
public sealed class PaintingAGridWithThreeDifferentColorsTests
{
    // LeetCode's three published examples, plus the two hand-counted grids the
    // pre-migration test carried. m=1 has no vertical adjacency at all, so only the
    // horizontal neighbour constrains it (1x1 -> 3 free colors, 1x2 -> 3*2 = 6);
    // 2x2 was counted by hand to cross-check the column-compatibility transition
    // rather than just the column-pattern count - for pattern (a,b) with a != b a
    // compatible (c,d) needs c != a, d != b and c != d, which leaves exactly 3
    // successors for each of the 6 length-2 patterns, so 6*3 = 18.
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 1, 1, 3 },
            { 1, 2, 6 },
            { 2, 1, 6 },
            { 2, 2, 18 },
            { 5, 5, 580986 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ColorTheGridByBruteForce_LeetCodeExamples_ReturnsColoringCount(
        int rows, int columns, int expected)
    {
        var actual = PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByBruteForce(rows, columns);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ColorTheGridByColumnPatternDynamicProgramming_LeetCodeExamples_ReturnsColoringCount(
        int rows, int columns, int expected)
    {
        var actual = PaintingAGridWithThreeDifferentColorsSolution.ColorTheGridByColumnPatternDynamicProgramming(
            rows, columns);

        Assert.Equal(expected, actual);
    }
}
