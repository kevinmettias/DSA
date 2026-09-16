using DSAExperimentation.LeetCode.CountAnagrams;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAnagrams;

// Harness only: the algorithms live in CountAnagramsSolution. One test method per
// strategy over one shared set of examples, so a failure names the strategy that
// broke (TwoSumTests precedent). Every expected value stays well under 1e9+7, so
// brute force's exact distinct-permutation product and the modular-factorial
// strategy's residue are directly comparable via plain equality.
public sealed partial class CountAnagramsTests
{
    public static TheoryData<string, long> Examples =>
        new()
        {
            { "too hot", 18 }, // "too": 3!/2! = 3 ; "hot": 3! = 6 ; product = 18
            { "aa", 1 }, // 2!/2! = 1
            { "a", 1 }, // single letter: exactly one arrangement
            { "banana", 60 }, // 6!/(1!*3!*2!) = 720/12 = 60
            { "ab ba", 4 }, // "ab": 2! = 2 ; "ba": 2! = 2 ; product = 4
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAnagramsByBruteForce_LeetCodeExamples_ReturnsDistinctPermutationProduct(string sentence, long expected)
    {
        var actual = CountAnagramsSolution.CountAnagramsByBruteForce(sentence);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountAnagramsByModularFactorial_LeetCodeExamples_ReturnsDistinctPermutationProduct(string sentence, long expected)
    {
        var actual = CountAnagramsSolution.CountAnagramsByModularFactorial(sentence);
        Assert.Equal(expected, actual);
    }
}
