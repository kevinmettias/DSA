using DSAExperimentation.LeetCode.ImplementMagicDictionary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementMagicDictionary;

// Harness only: both strategies are ImplementMagicDictionarySolution's. LeetCode's
// own shape here is a buildDict-once, search-many stateful object, so each Examples
// row pairs one dictionary with the batch of search words replayed against it - the
// same "call script" shape LRUCacheTests/MinStackTests use for their own
// instance-API problems.
public sealed class ImplementMagicDictionaryTests
{
    public static TheoryData<string[], string[], bool[]> Examples =>
        new()
        {
            {
                ["hello", "leetcode"],
                ["hello", "hhllo", "hell", "leetcoded"],
                [false, true, false, false]
            },
            {
                ["hello"],
                ["hi"],
                [false]
            },
            {
                ["hello"],
                ["hxllx"],
                [false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBruteForce_LeetCodeExamples_MatchesExpectedSequence(
        string[] dictionary, string[] searchWords, bool[] expected)
        => AssertSearchSequence(ImplementMagicDictionarySolution.CreateByBruteForce(), dictionary, searchWords, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTrieSearch_LeetCodeExamples_MatchesExpectedSequence(
        string[] dictionary, string[] searchWords, bool[] expected)
        => AssertSearchSequence(ImplementMagicDictionarySolution.CreateByTrieSearch(), dictionary, searchWords, expected);

    private static void AssertSearchSequence(
        ImplementMagicDictionarySolution.IMagicDictionary magicDictionary,
        string[] dictionary,
        string[] searchWords,
        bool[] expected)
    {
        magicDictionary.BuildDict(dictionary);

        for (var i = 0; i < searchWords.Length; i++)
        {
            Assert.Equal(expected[i], magicDictionary.Search(searchWords[i]));
        }
    }
}
