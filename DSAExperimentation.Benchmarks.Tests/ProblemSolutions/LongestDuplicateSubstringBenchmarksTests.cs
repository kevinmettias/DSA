using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestDuplicateSubstringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - comparing every pair of suffixes directly against
// this repo's suffix array and its longest-common-prefix array - so a harness whose arms disagree
// is timing two different problems. Both arms return the duplicated substring itself, compared
// directly. Setup draws the text from a fixed seed over a small alphabet, which is what makes
// duplicates plentiful and long rather than diverging on the first character; the same Length
// must rebuild the same text.
public sealed partial class LongestDuplicateSubstringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().SuffixArrayLongestCommonPrefix(),
            BuildHarness().SuffixArrayLongestCommonPrefix());

    [Fact]
    public void AllSuffixPairsBruteForce_SmallestLength_AgreesWithSuffixArrayLongestCommonPrefix()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.SuffixArrayLongestCommonPrefix(),
            harness.AllSuffixPairsBruteForce());
    }

    [Fact]
    public void SuffixArrayLongestCommonPrefix_SmallestLength_AgreesWithAllSuffixPairsBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.AllSuffixPairsBruteForce(),
            harness.SuffixArrayLongestCommonPrefix());
    }

    private static LongestDuplicateSubstringBenchmarks BuildHarness()
    {
        var harness = new LongestDuplicateSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
