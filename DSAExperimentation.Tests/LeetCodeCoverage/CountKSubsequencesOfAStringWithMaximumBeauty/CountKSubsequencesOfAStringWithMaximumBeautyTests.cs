using DSAExperimentation.LeetCode.CountKSubsequencesOfAStringWithMaximumBeauty;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountKSubsequencesOfAStringWithMaximumBeauty;

// Harness only: the algorithms live in
// CountKSubsequencesOfAStringWithMaximumBeautySolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed partial class CountKSubsequencesOfAStringWithMaximumBeautyTests
{
    public static TheoryData<string, int, long> Examples =>
        new()
        {
            { "bcca", 2, 4 }, // c(freq2) forced in, then choose 1 of {a,b} (freq1 each): C(2,1)*2^1*1^1 = 4
            { "abbcd", 4, 2 }, // only 4 distinct chars exist - the single set {a,b,c,d}, product 1*2*1*1 = 2
            { "abcabcabc", 2, 27 }, // a,b,c all tied at freq3: choose 2 of 3 -> C(3,2)*3^2 = 27
        };

    public static TheoryData<string, int> RandomizedInputs =>
        new() { { "aaaabbbbccccdddd", 3 }, { "thequickbrownfoxjumpsoverthelazydog", 5 }, { "zzzzyyyyxxxx", 2 } };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForceCombinations_LeetCodeExamples_ReturnsMaxBeautyCount(string text, int subsequenceLength, long expected)
    {
        var actual = CountKSubsequencesOfAStringWithMaximumBeautySolution.CountByBruteForceCombinations(text, subsequenceLength);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByGroupedFrequencyProduct_LeetCodeExamples_ReturnsMaxBeautyCount(string text, int subsequenceLength, long expected)
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
