using DSAExperimentation.LeetCode.DistinctEchoSubstrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctEchoSubstrings;

// Harness only. Both counting strategies are DistinctEchoSubstringsSolution's - the
// naive substring comparison that used to live untested as the benchmark baseline,
// and the RollingHash-screened sweep - pinned here to LeetCode's published examples
// plus the cases that separate "counts echoes" from "counts distinct echoes"
// ("aaaa", "abababab", where the same echo is found at several starts).
public sealed partial class DistinctEchoSubstringsTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "abcabcabc", 3 },
            { "leetcodeleetcode", 2 },
            { "bbb", 1 },
            { "a", 0 },
            { "abcdef", 0 },
            { "aaaa", 2 },
            { "abababab", 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctEchoesByNaiveSubstringComparison_LeetCodeExamples_ReturnsExpectedCount(
        string text, int expected) =>
        Assert.Equal(expected, DistinctEchoSubstringsSolution.CountDistinctEchoesByNaiveSubstringComparison(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDistinctEchoesByRollingHashScreen_LeetCodeExamples_ReturnsExpectedCount(
        string text, int expected) =>
        Assert.Equal(expected, DistinctEchoSubstringsSolution.CountDistinctEchoesByRollingHashScreen(text));
}
