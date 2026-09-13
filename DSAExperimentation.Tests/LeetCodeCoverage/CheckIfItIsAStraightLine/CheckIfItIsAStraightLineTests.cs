using DSAExperimentation.LeetCode.CheckIfItIsAStraightLine;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfItIsAStraightLine;

// Harness only: both strategies live in CheckIfItIsAStraightLineSolution and are
// asserted against the same examples. The O(n^3) every-triple baseline previously
// existed only as a benchmark arm and was never asserted by anything; it answers the
// same question as the anchored one-pass scan, so both run the same example set.
public sealed class CheckIfItIsAStraightLineTests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            { [[1, 2], [2, 3], [3, 4], [4, 5], [5, 6], [6, 7]], true },
            { [[1, 1], [2, 2], [3, 4], [4, 5], [5, 6], [7, 7]], false },
            { [[3, 1], [3, 5], [3, -2]], true },
            { [[0, 0], [1, 1]], true },
            { [[-4, 7], [2, 7], [9, 7]], true },
            { [[0, 0], [-2, 4], [-5, 10]], true },
            { [[0, 0], [1, 1], [1, 2]], false },
            { [[1, 1], [2, 2], [3, 3], [4, 5]], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckStraightLineByBruteForceEveryTriple_LeetCodeExamples_MatchesExpectedAnswer(
        int[][] coordinates,
        bool expected) =>
        Assert.Equal(
            expected,
            CheckIfItIsAStraightLineSolution.CheckStraightLineByBruteForceEveryTriple(coordinates));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckStraightLineByAnchoredCrossProductScan_LeetCodeExamples_MatchesExpectedAnswer(
        int[][] coordinates,
        bool expected) =>
        Assert.Equal(
            expected,
            CheckIfItIsAStraightLineSolution.CheckStraightLineByAnchoredCrossProductScan(coordinates));
}
