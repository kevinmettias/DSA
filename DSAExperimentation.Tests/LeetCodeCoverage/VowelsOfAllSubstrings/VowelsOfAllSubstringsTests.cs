using DSAExperimentation.LeetCode.VowelsOfAllSubstrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.VowelsOfAllSubstrings;

// Harness only. Both strategies are VowelsOfAllSubstringsSolution's - including the
// O(n^2) substring scan, which the benchmark used to own privately and nothing
// asserted; it is now the independent check that the closed form is actually
// counting the same thing.
public sealed partial class VowelsOfAllSubstringsTests
{
    public static TheoryData<string, long> Examples =>
        new()
        {
            { "aba", 6L },
            { "abc", 3L },
            { "ltcd", 0L },
            { "a", 1L },
            { "b", 0L },
            { "aeiou", 35L },
            { "vowelsofallsubstrings", 523L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVowelsBySubstringScan_LeetCodeExamples_ReturnsTotalVowelsOverAllSubstrings(
        string word, long expected) =>
        Assert.Equal(expected, VowelsOfAllSubstringsSolution.CountVowelsBySubstringScan(word));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountVowelsByContributionFormula_LeetCodeExamples_ReturnsTotalVowelsOverAllSubstrings(
        string word, long expected) =>
        Assert.Equal(expected, VowelsOfAllSubstringsSolution.CountVowelsByContributionFormula(word));
}
