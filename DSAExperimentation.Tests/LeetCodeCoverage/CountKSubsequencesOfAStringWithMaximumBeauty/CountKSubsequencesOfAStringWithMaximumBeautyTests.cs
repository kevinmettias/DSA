using DSAExperimentation.LeetCode.CountKSubsequencesOfAStringWithMaximumBeauty;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountKSubsequencesOfAStringWithMaximumBeauty;

// Harness only: the algorithms live in
// CountKSubsequencesOfAStringWithMaximumBeautySolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class CountKSubsequencesOfAStringWithMaximumBeautyTests
{
    public static TheoryData<string, int, long> Examples =>
        new()
        {
            { "bcca", 2, 4 }, // c(freq2) forced in, then choose 1 of {a,b} (freq1 each): C(2,1)*2^1*1^1 = 4
            { "abbcd", 4, 2 }, // only 4 distinct chars exist - the single set {a,b,c,d}, product 1*2*1*1 = 2
            { "abcabcabc", 2, 27 }, // a,b,c all tied at freq3: choose 2 of 3 -> C(3,2)*3^2 = 27
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForceCombinations_LeetCodeExamples_ReturnsMaxBeautyCount(string s, int k, long expected)
        => Assert.Equal(expected, CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByGroupedFrequencyProduct_LeetCodeExamples_ReturnsMaxBeautyCount(string s, int k, long expected)
        => Assert.Equal(expected, CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByGroupedFrequencyProduct(s, k));

    [Theory]
    [InlineData("aaaabbbbccccdddd", 3)]
    [InlineData("thequickbrownfoxjumpsoverthelazydog", 5)]
    [InlineData("zzzzyyyyxxxx", 2)]
    public void BothStrategies_RandomizedInputs_Agree(string s, int k)
    {
        var bruteForce = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(s, k);
        var grouped = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByGroupedFrequencyProduct(s, k);

        Assert.Equal(bruteForce, grouped);
    }
}
