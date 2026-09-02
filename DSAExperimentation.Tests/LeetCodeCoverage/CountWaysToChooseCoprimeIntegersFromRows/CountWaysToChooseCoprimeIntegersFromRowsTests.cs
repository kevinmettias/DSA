using DSAExperimentation.LeetCode.CountWaysToChooseCoprimeIntegersFromRows;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountWaysToChooseCoprimeIntegersFromRows;

// Harness only. Both strategies are
// CountWaysToChooseCoprimeIntegersFromRowsSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class CountWaysToChooseCoprimeIntegersFromRowsTests
{
    public static TheoryData<int[][], long> Examples =>
        new()
        {
            { [[1, 2], [3, 4]], 3 },
            { [[2, 2], [2, 2]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByBruteForce_LeetCodeExamples_ReturnsCoprimeChoiceCount(int[][] mat, long expected) =>
        Assert.Equal(expected, CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByBruteForce(mat));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByGcdCountingDp_LeetCodeExamples_ReturnsCoprimeChoiceCount(int[][] mat, long expected) =>
        Assert.Equal(expected, CountWaysToChooseCoprimeIntegersFromRowsSolution.CountWaysByGcdCountingDp(mat));
}
