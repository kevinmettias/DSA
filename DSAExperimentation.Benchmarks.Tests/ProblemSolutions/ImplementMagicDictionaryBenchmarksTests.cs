using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.ImplementMagicDictionary;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementMagicDictionaryBenchmarks (ARCHITECTURE 17.9). Both arms are
// competing strategies for the same question - the Hamming-distance scan over the remembered
// word list against the trie walk - so they are asserted to agree. Both return the count of
// matches over a fixed batch of search words, which is a proxy rather than the per-word
// verdicts: [GlobalSetup] derives every search word from a dictionary word by changing exactly
// one character, so under LeetCode 676's own "exactly one character different" rule every
// query is a guaranteed match and an arm that agreed on the count would still be wrong about
// which words matched. The agreement is therefore honest and weak on its own, which is why each
// [Fact] also replays the same two factories over a small dictionary whose queries genuinely
// differ in outcome - a real match, an exact word (no substitution spent), a two-character
// change, and a length mismatch - so the strategies are reconciled on a question with more than
// one answer. The dictionary is seeded, so the same DictionarySize must rebuild the same words
// and therefore the same match count.
public sealed partial class ImplementMagicDictionaryBenchmarksTests
{
    private const int SmallestDictionarySize = 10_000;

    [Fact]
    public void Setup_SameDictionarySize_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
        Assert.Equal(BuildHarness().TrieSearch(), BuildHarness().TrieSearch());
    }

    // Every generated search word is one character away from a dictionary word, so the match
    // count is the number of distinct dictionary words - and RandomWord draws eight characters
    // from a twenty-six letter alphabet, so Distinct() drops nothing.
    [Fact]
    public void BruteForce_EverySearchWordIsOneCharacterAway_MatchesTheWholeDictionary()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrieSearch(), harness.BruteForce());
        Assert.Equal(SmallestDictionarySize, harness.BruteForce());
    }

    [Fact]
    public void TrieSearch_DiscriminatingQueries_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.TrieSearch());
        Assert.Equal(
            MatchingQueryCount(ImplementMagicDictionarySolution.CreateByBruteForce()),
            MatchingQueryCount(ImplementMagicDictionarySolution.CreateByTrieSearch()));
    }

    private static ImplementMagicDictionaryBenchmarks BuildHarness()
    {
        var harness = new ImplementMagicDictionaryBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }

    // One query per outcome the problem distinguishes, so a strategy that got any of them wrong
    // reports a different total than one that got them all right.
    private static int MatchingQueryCount(ImplementMagicDictionarySolution.IMagicDictionary magicDictionary)
    {
        magicDictionary.BuildDict(DiscriminatingDictionary);

        var matches = 0;

        foreach (var query in DiscriminatingQueries)
        {
            if (magicDictionary.Search(query))
            {
                matches++;
            }
        }

        return matches;
    }

    private static readonly string[] DiscriminatingDictionary = ["hello", "leetcode", "aaaa", "ab"];

    // Matches: hello -> hallo, leetcode -> leetcoda. Rejected: hello itself (no substitution
    // spent), a two-character change of aaaa, a word of a length no dictionary word has, and two
    // characters against the two-character word.
    private static readonly string[] DiscriminatingQueries =
        ["hallo", "leetcoda", "hello", "aabb", "helloaa", "zz"];
}
