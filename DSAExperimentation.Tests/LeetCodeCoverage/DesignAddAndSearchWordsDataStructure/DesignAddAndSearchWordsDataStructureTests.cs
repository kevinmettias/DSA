using DSAExperimentation.LeetCode.DesignAddAndSearchWordsDataStructure;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAddAndSearchWordsDataStructure;

// Harness only. Both strategies are DesignAddAndSearchWordsDataStructureSolution's
// - this file adds every word in wordsToAdd, then replays searches against each
// IWordDictionaryStrategy implementation and checks the results LeetCode itself
// publishes, so a failure still names the strategy that broke.
public sealed class DesignAddAndSearchWordsDataStructureTests
{
    public static TheoryData<string[], string[], bool[]> Examples =>
        new()
        {
            {
                ["bad", "dad", "mad"],
                ["pad", "bad", ".ad", "b.."],
                new[] { false, true, true, true }
            },
            {
                Array.Empty<string>(),
                ["a", "."],
                new[] { false, false }
            },
            {
                ["a"],
                ["a", ".", "aa", ".."],
                new[] { true, true, false, false }
            },
            {
                ["abc"],
                ["...", "a.c", "ab.", "..."],
                new[] { true, true, true, true }
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WordDictionaryByLinearScan_LeetCodeExamples_MatchesDotWildcardPattern(
        string[] wordsToAdd, string[] searches, bool[] expected) =>
        AssertSearchResults(
            new DesignAddAndSearchWordsDataStructureSolution.WordDictionaryByLinearScan(),
            wordsToAdd,
            searches,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void WordDictionaryByTrie_LeetCodeExamples_MatchesDotWildcardPattern(
        string[] wordsToAdd, string[] searches, bool[] expected) =>
        AssertSearchResults(
            new DesignAddAndSearchWordsDataStructureSolution.WordDictionaryByTrie(),
            wordsToAdd,
            searches,
            expected);

    private static void AssertSearchResults(
        DesignAddAndSearchWordsDataStructureSolution.IWordDictionaryStrategy dictionary,
        string[] wordsToAdd,
        string[] searches,
        bool[] expected)
    {
        foreach (var word in wordsToAdd)
        {
            dictionary.AddWord(word);
        }

        for (var i = 0; i < searches.Length; i++)
        {
            Assert.Equal(expected[i], dictionary.Search(searches[i]));
        }
    }
}
