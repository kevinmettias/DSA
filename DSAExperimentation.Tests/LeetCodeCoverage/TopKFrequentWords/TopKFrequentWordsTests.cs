using DSAExperimentation.LeetCode.TopKFrequentWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TopKFrequentWords;

// Harness only. Both strategies are TopKFrequentWordsSolution's - this file just
// pins them to LeetCode's published examples, including the frequency-tie case
// that exercises the required lexicographic tie-break.
public sealed partial class TopKFrequentWordsTests
{
    public static TheoryData<string[], int, string[]> Examples =>
        new()
        {
            { ["i", "love", "leetcode", "i", "love", "coding"], 2, ["i", "love"] },
            { ["the", "day", "is", "sunny", "the", "the", "the", "sunny", "is", "is"], 4, ["the", "is", "sunny", "day"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TopKFrequentByFullSort_LeetCodeExamples_BreaksTiesLexicographically(
        string[] words, int topCount, string[] expected)
    {
        var actual = TopKFrequentWordsSolution.TopKFrequentByFullSort(words, topCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TopKFrequentByMinHeap_LeetCodeExamples_BreaksTiesLexicographically(
        string[] words, int topCount, string[] expected)
    {
        var actual = TopKFrequentWordsSolution.TopKFrequentByMinHeap(words, topCount);

        Assert.Equal(expected, actual);
    }
}
