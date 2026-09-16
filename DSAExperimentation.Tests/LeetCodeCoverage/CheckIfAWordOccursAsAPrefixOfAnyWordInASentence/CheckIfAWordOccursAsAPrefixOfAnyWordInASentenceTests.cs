using DSAExperimentation.LeetCode.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

// Harness only. Both the StartsWith scan and the Trie<bool> insert-then-HasPrefix
// round trip are CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution's; this
// file pins them to LeetCode's published examples plus the boundary cases the two
// have to agree on - a whole-word match, a first-word match, and a search word that
// occurs inside words without ever starting one.
public sealed class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceTests
{
    public static TheoryData<PrefixWordExample> Examples =>
        new()
        {
            { new PrefixWordExample(Sentence: "i love eating burger", SearchWord: "burg", Expected: 4) },
            { new PrefixWordExample(Sentence: "this problem is an easy problem", SearchWord: "pro", Expected: 2) },
            { new PrefixWordExample(Sentence: "i am tired", SearchWord: "you", Expected: -1) },
            { new PrefixWordExample(Sentence: "i am tired", SearchWord: "tired", Expected: 3) },
            { new PrefixWordExample(Sentence: "burger burg burgers", SearchWord: "burg", Expected: 1) },
            { new PrefixWordExample(Sentence: "hellohello hellohellohello", SearchWord: "ell", Expected: -1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfPrefixWordByStartsWithScan_LeetCodeExamples_ReturnsFirstMatchingWordPosition(
        PrefixWordExample example)
    {
        var position =
            CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByStartsWithScan(
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText(example.Sentence),
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix(example.SearchWord));

        Assert.Equal(example.Expected, position);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfPrefixWordByTriePerWord_LeetCodeExamples_ReturnsFirstMatchingWordPosition(
        PrefixWordExample example)
    {
        var position =
            CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.IndexOfPrefixWordByTriePerWord(
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText(example.Sentence),
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix(example.SearchWord));

        Assert.Equal(example.Expected, position);
    }

    // One LeetCode example: the sentence to search, the prefix to look for, and the
    // 1-based position of the first word it starts. Both strings are the same type and the
    // question is not symmetric, so the row names which is which rather than leaving two
    // interchangeable positions.
    public readonly record struct PrefixWordExample(string Sentence, string SearchWord, int Expected);
}
