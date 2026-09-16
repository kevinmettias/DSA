using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EncryptAndDecryptStringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-encrypting the whole dictionary on every decrypt
// against precomputing each word's encryption once into this repo's own HashMap frequency table - so
// a harness whose arms disagree is timing two different problems. Setup builds every query as the
// true encryption of some dictionary word, which is what makes a match total of zero impossible and
// keeps both arms doing genuine lookups instead of scanning for something that is not there. Each
// arm constructs its own encrypter inside the measured call and only reads the prepared inputs, so
// one harness is safe to call twice in either order.
//
// Both arms return the match count summed over all DecryptCalls queries rather than one call's
// answer, so agreement witnesses that the two strategies agreed on every query in the script, not
// on any single one.
public sealed partial class EncryptAndDecryptStringsBenchmarksTests
{
    private const int SmallestDictionarySize = 50;
    private const int MinimumMatchCountForQueriesEncryptedFromDictionaryWords = 1;

    [Fact]
    public void Setup_QueriesEncryptedFromTheDictionary_MatchAtLeastOnceAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();
        var matches = harness.PrecomputedFrequencyMap();

        Assert.True(matches >= MinimumMatchCountForQueriesEncryptedFromDictionaryWords);
        Assert.Equal(matches, BuildHarness().PrecomputedFrequencyMap());
    }

    [Fact]
    public void RecomputeEveryDecrypt_QueriesEncryptedFromTheDictionary_AgreesWithPrecomputedFrequencyMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrecomputedFrequencyMap(), harness.RecomputeEveryDecrypt());
    }

    [Fact]
    public void PrecomputedFrequencyMap_QueriesEncryptedFromTheDictionary_AgreesWithRecomputeEveryDecrypt()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecomputeEveryDecrypt(), harness.PrecomputedFrequencyMap());
    }

    private static EncryptAndDecryptStringsBenchmarks BuildHarness()
    {
        var harness = new EncryptAndDecryptStringsBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
