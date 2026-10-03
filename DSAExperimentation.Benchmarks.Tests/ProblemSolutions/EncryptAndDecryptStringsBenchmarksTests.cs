using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EncryptAndDecryptStringsBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: that every query finds a match, which follows from Setup's construction rather than from either arm. Setup
// builds every query as the true encryption of some dictionary word, which is what keeps both arms doing genuine
// lookups instead of scanning for something that is not there, and each arm returns every count Decrypt reported.
public sealed partial class EncryptAndDecryptStringsBenchmarksTests
{
    private const int SmallestDictionarySize = 50;
    private const int MinimumMatchCountForQueriesEncryptedFromDictionaryWords = 1;

    [Fact]
    public void RecomputeEveryDecrypt_QueriesEncryptedFromTheDictionary_EachMatchAtLeastOnce() =>
        AssertEachMatchesAtLeastOnce(BuildHarness().RecomputeEveryDecrypt());

    [Fact]
    public void PrecomputedFrequencyMap_QueriesEncryptedFromTheDictionary_EachMatchAtLeastOnce() =>
        AssertEachMatchesAtLeastOnce(BuildHarness().PrecomputedFrequencyMap());

    private static void AssertEachMatchesAtLeastOnce(int[] matches) =>
        Assert.All(matches, count => Assert.True(count >= MinimumMatchCountForQueriesEncryptedFromDictionaryWords));

    private static EncryptAndDecryptStringsBenchmarks BuildHarness()
    {
        var harness = new EncryptAndDecryptStringsBenchmarks { DictionarySize = SmallestDictionarySize };
        harness.Setup();

        return harness;
    }
}
