using DSAExperimentation.LeetCode.Solutions.ValidSquare;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidSquare;

// Harness only. Both strategies are ValidSquareSolution's; this file pins them to a
// square tilted 45 degrees and to four copies of one point. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class ValidSquareSolutionTests
{
    public static TheoryData<SquareCase> Cases =>
        new()
        {
            // A square tilted 45 degrees, then four copies of one point.
            { new SquareCase(P1: [0, 0], P2: [1, 1], P3: [2, 0], P4: [1, -1], Expected: true) },
            { new SquareCase(P1: [5, 5], P2: [5, 5], P3: [5, 5], P4: [5, 5], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidSquareByMergeSort_Cases_ReturnsWhetherFourPointsFormASquare(SquareCase testCase)
    {
        var actual = ValidSquareSolution.IsValidSquareByMergeSort(
            testCase.P1, testCase.P2, testCase.P3, testCase.P4);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsValidSquareByMinMaxScan_Cases_ReturnsWhetherFourPointsFormASquare(SquareCase testCase)
    {
        var actual = ValidSquareSolution.IsValidSquareByMinMaxScan(
            testCase.P1, testCase.P2, testCase.P3, testCase.P4);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: four points, and whether they form a square. Each corner
    // is named for the role it plays, so a row cannot be read with two of the four
    // silently exchanged.
    public readonly record struct SquareCase(int[] P1, int[] P2, int[] P3, int[] P4, bool Expected);
}
