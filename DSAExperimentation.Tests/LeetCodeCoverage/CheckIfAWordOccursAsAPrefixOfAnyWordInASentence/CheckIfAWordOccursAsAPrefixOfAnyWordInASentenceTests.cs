using DSAExperimentation.LeetCode.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

// Harness only. Both the StartsWith scan and the Trie<bool> insert-then-HasPrefix
// round trip are CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution's; this
// file pins them to LeetCode's published examples plus the boundary cases the two
// have to agree on - a whole-word match, a first-word match, and a search word that
// occurs inside words without ever starting one.
public sealed class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "i love eating burger", "burg", 4 },
            { "this problem is an easy problem", "pro", 2 },
            { "i am tired", "you", -1 },
            { "i am tired", "tired", 3 },
            { "burger burg burgers", "burg", 1 },
            { "hellohello hellohellohello", "ell", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfPrefixWordByStartsWithScan_LeetCodeExamples_ReturnsFirstMatchingWordPosition(
        string sentence, string searchWord, int expected) =>
        Assert.Equal(
            expected,
            CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByStartsWithScan(
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText(sentence),
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix(searchWord)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfPrefixWordByTriePerWord_LeetCodeExamples_ReturnsFirstMatchingWordPosition(
        string sentence, string searchWord, int expected) =>
        Assert.Equal(
            expected,
            CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByTriePerWord(
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText(sentence),
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix(searchWord)));
}
