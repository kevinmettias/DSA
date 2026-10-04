using DSAExperimentation.LeetCode.PalindromePairs;

namespace DSAExperimentation.LeetCode.Tests.PalindromePairs;

// Harness only. Every strategy is PalindromePairsSolution's - this file pins them to
// LeetCode's published examples, expressed as sets of index pairs since a word may pair
// with more than one other word and pair order isn't specified. The first three rows are
// LeetCode's examples; in the fourth, by hand, neither "abcxyz" nor "xyzabc" is a palindrome.
//
// The composed strategies are also run on seeded random lists of short unique words over
// {a, b} - a two-letter alphabet makes palindromic joins common - including the empty word,
// against PairsByDefinition, written here apart from the solution: a pair counts when the
// joined words equal their own reversal. Those lists are compared in order after sorting,
// so a pair reported twice fails too.
public sealed partial class PalindromePairsSolutionTests
{
    private const int RandomListCount = 200;
    private const int LargestWordCount = 12;
    private const int LongestWordLength = 6;
    private const string Alphabet = "ab";

    public static TheoryData<string[], (int First, int Second)[]> Examples =>
        new()
        {
            {
                // "dcba"+"abcd", "abcd"+"dcba", "s"+"lls", "lls"+"sssll" (llssssll) all read the same forwards and backwards.
                ["abcd", "dcba", "lls", "s", "sssll"],
                [(0, 1), (1, 0), (3, 2), (2, 4)]
            },
            { ["bat", "tab", "cat"], [(0, 1), (1, 0)] },
            { ["a", ""], [(0, 1), (1, 0)] },
            { ["abc", "xyz"], [] },
        };

    public static TheoryData<int> RandomListSeeds
    {
        get
        {
            var seeds = new TheoryData<int>();

            for (var seed = 1; seed <= RandomListCount; seed++)
            {
                seeds.Add(seed);
            }

            return seeds;
        }
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPairsByBruteForce_LeetCodeExamples_ReturnsAllValidPairs(
        string[] words, (int First, int Second)[] expected) =>
        Assert.Equal(expected.ToHashSet(), PalindromePairsSolution.FindPairsByBruteForce(words).ToHashSet());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPairsByHashMapComplementLookup_LeetCodeExamples_ReturnsAllValidPairs(
        string[] words, (int First, int Second)[] expected) =>
        Assert.Equal(
            expected.ToHashSet(), PalindromePairsSolution.FindPairsByHashMapComplementLookup(words).ToHashSet());

    [Theory]
    [MemberData(nameof(RandomListSeeds))]
    public void FindPairsByHashMapComplementLookup_RandomShortWords_MatchesPairsByDefinition(int seed)
    {
        var words = RandomUniqueWords(seed);

        Assert.Equal(PairsByDefinition(words), PalindromePairsSolution.FindPairsByHashMapComplementLookup(words).Order());
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPairsByReversedWordTrie_LeetCodeExamples_ReturnsAllValidPairs(
        string[] words, (int First, int Second)[] expected) =>
        Assert.Equal(expected.ToHashSet(), PalindromePairsSolution.FindPairsByReversedWordTrie(words).ToHashSet());

    [Theory]
    [MemberData(nameof(RandomListSeeds))]
    public void FindPairsByReversedWordTrie_RandomShortWords_MatchesPairsByDefinition(int seed)
    {
        var words = RandomUniqueWords(seed);

        Assert.Equal(PairsByDefinition(words), PalindromePairsSolution.FindPairsByReversedWordTrie(words).Order());
    }

    // Up to LargestWordCount draws of up to LongestWordLength letters, duplicates dropped,
    // since LeetCode's words are unique.
    private static string[] RandomUniqueWords(int seed)
    {
        var random = new Random(seed);
        var drawCount = random.Next(1, LargestWordCount + 1);

        return [.. Enumerable.Range(0, drawCount).Select(_ => RandomWord(random)).Distinct()];
    }

    private static string RandomWord(Random random)
    {
        var letters = new char[random.Next(0, LongestWordLength + 1)];

        for (var i = 0; i < letters.Length; i++)
        {
            letters[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        return new string(letters);
    }

    // Every ordered pair of distinct indices whose joined words read the same reversed,
    // in sorted order.
    private static List<(int First, int Second)> PairsByDefinition(string[] words)
    {
        var pairs = new List<(int First, int Second)>();

        for (var first = 0; first < words.Length; first++)
        {
            for (var second = 0; second < words.Length; second++)
            {
                var joined = words[first] + words[second];

                if (first != second && joined.SequenceEqual(joined.Reverse()))
                {
                    pairs.Add((first, second));
                }
            }
        }

        return pairs;
    }
}
