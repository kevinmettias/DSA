using DSAExperimentation.LeetCode.Solutions.CheckIfItIsAStraightLine;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfItIsAStraightLine;

// Harness only: both strategies live in CheckIfItIsAStraightLineSolution and are
// asserted against the same cases. The O(n^3) every-triple baseline previously
// existed only as a benchmark arm and was never asserted by anything; it answers the
// same question as the anchored one-pass scan, so both run the same cases.
public sealed partial class CheckIfItIsAStraightLineSolutionTests
{
    public static TheoryData<StraightLineCase> Cases =>
        new()
        {
            { new StraightLineCase([[3, 1], [3, 5], [3, -2]], Expected: true) },
            { new StraightLineCase([[0, 0], [1, 1]], Expected: true) },
            { new StraightLineCase([[-4, 7], [2, 7], [9, 7]], Expected: true) },
            { new StraightLineCase([[0, 0], [-2, 4], [-5, 10]], Expected: true) },
            { new StraightLineCase([[0, 0], [1, 1], [1, 2]], Expected: false) },
            { new StraightLineCase([[1, 1], [2, 2], [3, 3], [4, 5]], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsStraightLineByBruteForceEveryTriple_Cases_MatchesExpectedAnswer(
        StraightLineCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            CheckIfItIsAStraightLineSolution.IsStraightLineByBruteForceEveryTriple(testCase.Coordinates));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsStraightLineByAnchoredCrossProductScan_Cases_MatchesExpectedAnswer(
        StraightLineCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            CheckIfItIsAStraightLineSolution.IsStraightLineByAnchoredCrossProductScan(testCase.Coordinates));

    // One case: the points under test and whether they are collinear.
    // The expected value is named at every construction site, so a row reads as the
    // case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct StraightLineCase(int[][] Coordinates, bool Expected);
}
