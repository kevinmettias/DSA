using DSAExperimentation.LeetCode.Solutions.VowelsOfAllSubstrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.VowelsOfAllSubstrings;

// Harness only. Both strategies are VowelsOfAllSubstringsSolution's - including the
// O(n^2) substring scan, which the benchmark used to own privately and nothing
// asserted; it is now the independent check that the closed form is actually
// counting the same thing.
public sealed partial class VowelsOfAllSubstringsSolutionTests
{
    public static TheoryData<string, long> Cases =>
        new()
        {
            { "vowelsofallsubstrings", 523L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVowelsBySubstringScan_Cases_ReturnsTotalVowelsOverAllSubstrings(
        string word, long expected) =>
        Assert.Equal(expected, VowelsOfAllSubstringsSolution.CountVowelsBySubstringScan(word));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountVowelsByContributionFormula_Cases_ReturnsTotalVowelsOverAllSubstrings(
        string word, long expected) =>
        Assert.Equal(expected, VowelsOfAllSubstringsSolution.CountVowelsByContributionFormula(word));
}
