using DSAExperimentation.LeetCode.Solutions.CountKSubsequencesOfAStringWithMaximumBeauty;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountKSubsequencesOfAStringWithMaximumBeauty;

// Harness only: the algorithms live in
// CountKSubsequencesOfAStringWithMaximumBeautySolution. One test method per
// strategy over one shared set of cases - a three-way frequency tie the subsequence
// has to choose from - so a failure names the strategy that broke; randomized inputs
// then check that the two strategies agree. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class CountKSubsequencesOfAStringWithMaximumBeautySolutionTests
{
    public static TheoryData<string, int, long> Cases =>
        new()
        {
            { "abcabcabc", 2, 27 }, // a,b,c all tied at freq3: choose 2 of 3 -> C(3,2)*3^2 = 27
        };

    public static TheoryData<string, int> RandomizedInputs =>
        new() { { "aaaabbbbccccdddd", 3 }, { "thequickbrownfoxjumpsoverthelazydog", 5 }, { "zzzzyyyyxxxx", 2 } };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByBruteForceCombinations_Cases_ReturnsMaxBeautyCount(string text, int subsequenceLength, long expected)
    {
        var actual = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(text, subsequenceLength);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByGroupedFrequencyProduct_Cases_ReturnsMaxBeautyCount(string text, int subsequenceLength, long expected)
    {
        var actual = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByGroupedFrequencyProduct(text, subsequenceLength);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(RandomizedInputs))]
    public void BothStrategies_RandomizedInputs_Agree(string text, int subsequenceLength)
    {
        var bruteForce = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(text, subsequenceLength);
        var grouped = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByGroupedFrequencyProduct(text, subsequenceLength);

        Assert.Equal(bruteForce, grouped);
    }
}
