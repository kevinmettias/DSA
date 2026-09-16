using DSAExperimentation.LeetCode.FrequenciesOfShortestSupersequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrequenciesOfShortestSupersequences;

// Harness only. The letter-precedence graph is LeetCode.FrequenciesOfShortestSupersequences.LetterGraph
// and both subset-search strategies are FrequenciesOfShortestSupersequencesSolution's -
// this file just pins them to LeetCode's published examples. Neither LC nor either
// strategy promises an output order, so assertions compare the two result sets
// after sorting each into a canonical order.
public sealed partial class FrequenciesOfShortestSupersequencesTests
{
    public static TheoryData<string[], int[][]> Examples =>
        new()
        {
            { ["ab", "ba"], [Freq(('a', 1), ('b', 2)), Freq(('a', 2), ('b', 1))] },
            { ["aa", "ac"], [Freq(('a', 2), ('c', 1))] },
            { ["aa", "bb", "cc"], [Freq(('a', 2), ('b', 2), ('c', 2))] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SupersequenceFrequenciesByDfsSkipSet_LeetCodeExamples_ReturnsMinimumFrequencyVectors(
        string[] words, int[][] expected) =>
        Assert.Equal(
            Canonicalize(expected),
            Canonicalize(FrequenciesOfShortestSupersequencesSolution.SupersequenceFrequenciesByDfsSkipSet(words)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SupersequenceFrequenciesByTopologicalSort_LeetCodeExamples_ReturnsMinimumFrequencyVectors(
        string[] words, int[][] expected) =>
        Assert.Equal(
            Canonicalize(expected),
            Canonicalize(
                FrequenciesOfShortestSupersequencesSolution.SupersequenceFrequenciesByTopologicalSort(words)));

    private static int[] Freq(params (char Letter, int Count)[] entries)
    {
        var frequency = new int[26];

        foreach (var (letter, count) in entries)
        {
            frequency[letter - 'a'] = count;
        }

        return frequency;
    }

    private static List<string> Canonicalize(int[][] frequencies) =>
        frequencies.Select(frequency => string.Join(',', frequency)).Order().ToList();
}
