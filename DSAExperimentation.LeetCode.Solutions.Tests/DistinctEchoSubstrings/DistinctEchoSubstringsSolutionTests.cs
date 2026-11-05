using DSAExperimentation.LeetCode.Solutions.DistinctEchoSubstrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DistinctEchoSubstrings;

// Harness only. Both counting strategies are DistinctEchoSubstringsSolution's - the
// naive substring comparison that used to live untested as the benchmark baseline,
// and the RollingHash-screened sweep - pinned here to a string with no echo and to
// "bbb", where the echo "bb" is found at two starts, which separates "counts
// echoes" from "counts distinct echoes". LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class DistinctEchoSubstringsSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "bbb", 1 },
            { "abcdef", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDistinctEchoesByNaiveSubstringComparison_Cases_ReturnsExpectedCount(
        string text, int expected) =>
        Assert.Equal(expected, DistinctEchoSubstringsSolution.CountDistinctEchoesByNaiveSubstringComparison(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDistinctEchoesByRollingHashScreen_Cases_ReturnsExpectedCount(
        string text, int expected) =>
        Assert.Equal(expected, DistinctEchoSubstringsSolution.CountDistinctEchoesByRollingHashScreen(text));
}
