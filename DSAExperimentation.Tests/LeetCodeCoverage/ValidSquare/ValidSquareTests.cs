using DSAExperimentation.LeetCode.ValidSquare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSquare;

// Harness only. Both strategies are ValidSquareSolution's; this file pins them to
// LeetCode's published examples.
public sealed partial class ValidSquareTests
{
    public static TheoryData<SquareExample> Examples =>
        new()
        {
            { new SquareExample(P1: [0, 0], P2: [1, 1], P3: [1, 0], P4: [0, 1], Expected: true) },
            { new SquareExample(P1: [0, 0], P2: [1, 1], P3: [2, 0], P4: [1, -1], Expected: true) },
            { new SquareExample(P1: [0, 0], P2: [1, 1], P3: [1, 0], P4: [0, 12], Expected: false) },
            { new SquareExample(P1: [0, 0], P2: [1, 1], P3: [2, 2], P4: [3, 3], Expected: false) },
            { new SquareExample(P1: [5, 5], P2: [5, 5], P3: [5, 5], P4: [5, 5], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidSquareByMergeSort_LeetCodeExamples_ReturnsWhetherFourPointsFormASquare(SquareExample example)
    {
        var actual = ValidSquareSolution.IsValidSquareByMergeSort(
            example.P1, example.P2, example.P3, example.P4);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidSquareByMinMaxScan_LeetCodeExamples_ReturnsWhetherFourPointsFormASquare(SquareExample example)
    {
        var actual = ValidSquareSolution.IsValidSquareByMinMaxScan(
            example.P1, example.P2, example.P3, example.P4);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: four points, and whether they form a square. Each corner
    // is named for the role it plays, so a row cannot be read with two of the four
    // silently exchanged.
    public readonly record struct SquareExample(int[] P1, int[] P2, int[] P3, int[] P4, bool Expected);
}
