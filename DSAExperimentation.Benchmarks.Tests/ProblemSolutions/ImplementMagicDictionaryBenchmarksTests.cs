using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.ImplementMagicDictionary;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementMagicDictionaryBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: the verdicts themselves, known from Setup's construction rather than from either arm. Both arms return
// every Search verdict over a fixed batch of search words. [GlobalSetup] derives every search word from a
// dictionary word by changing exactly one character, so under LeetCode 676's own "exactly one character
// different" rule every query is a guaranteed match. A workload whose verdicts are all true cannot tell a
// strategy that answers true regardless, which is why the same two factories are also replayed over a small
// dictionary whose queries genuinely differ in outcome - a real match, an exact word (no substitution spent), a
// two-character change, and a length mismatch.
public sealed partial class ImplementMagicDictionaryBenchmarksTests
{
    private const int SmallestDictionarySize = 10;

    private static readonly string[] DiscriminatingDictionary = ["hello", "leetcode", "aaaa", "ab"];

    // Matches: hello -> hallo, leetcode -> leetcoda. Rejected: hello itself (no substitution
    // spent), a two-character change of aaaa, a word of a length no dictionary word has, and two
    // characters against the two-character word.
    private static readonly string[] DiscriminatingQueries =
        ["hallo", "leetcoda", "hello", "aabb", "helloaa", "zz"];

    private static readonly bool[] DiscriminatingVerdicts = [true, true, false, false, false, false];

    // Every generated search word is one character away from a dictionary word, so there is one
    // matching verdict per distinct dictionary word - and RandomWord draws eight characters from a
    // twenty-six letter alphabet, so Distinct() drops nothing.
    [Fact]
    public void BruteForce_EverySearchWordIsOneCharacterAway_MatchesTheWholeDictionary() =>
        AssertMatchesTheWholeDictionary(BuildHarness().BruteForce());

    [Fact]
    public void TrieSearch_EverySearchWordIsOneCharacterAway_MatchesTheWholeDictionary() =>
        AssertMatchesTheWholeDictionary(BuildHarness().TrieSearch());

    [Fact]
    public void BruteForce_DiscriminatingQueries_MatchesOnlyTheOneSubstitutionAway() =>
        Assert.Equal(DiscriminatingVerdicts, VerdictsOf(ImplementMagicDictionarySolution.CreateByBruteForce()));

    [Fact]
    public void TrieSearch_DiscriminatingQueries_MatchesOnlyTheOneSubstitutionAway() =>
        Assert.Equal(DiscriminatingVerdicts, VerdictsOf(ImplementMagicDictionarySolution.CreateByTrieSearch()));

    private static void AssertMatchesTheWholeDictionary(bool[] verdicts)
    {
        Assert.Equal(SmallestDictionarySize, verdicts.Length);
        Assert.DoesNotContain(false, verdicts);
    }

    // One query per outcome the problem distinguishes, so a strategy that got any of them wrong
    // reports a different verdict than one that got them all right.
    private static bool[] VerdictsOf(ImplementMagicDictionarySolution.IMagicDictionary magicDictionary)
    {
        magicDictionary.BuildDict(DiscriminatingDictionary);

        return [.. DiscriminatingQueries.Select(magicDictionary.Search)];
    }

    private static ImplementMagicDictionaryBenchmarks BuildHarness()
    {
        var harness = new ImplementMagicDictionaryBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
