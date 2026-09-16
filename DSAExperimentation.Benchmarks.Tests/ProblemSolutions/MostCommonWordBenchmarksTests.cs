using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MostCommonWordBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the textbook Dictionary<string,int> + HashSet<string> scan against this repo's own
// HashMap<string,int> plus Set<string> - so a harness whose arms disagree is timing two different problems.
// Both arms only read the token list and the banned-word slice built in [GlobalSetup], so one harness
// instance is safe to call twice in either order. Setup draws both from one fixed seed, so the same Length
// must rebuild the same token stream; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MostCommonWordBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameTokensAndBannedWords() =>
        Assert.Equal(BuildHarness().ByDictionaryScan(), BuildHarness().ByDictionaryScan());

    [Fact]
    public void ByDictionaryScan_RepeatingWordPool_AgreesWithByHashMapTally()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByHashMapTally(), harness.ByDictionaryScan());
    }

    [Fact]
    public void ByHashMapTally_RepeatingWordPool_AgreesWithByDictionaryScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByDictionaryScan(), harness.ByHashMapTally());
    }

    private static MostCommonWordBenchmarks BuildHarness()
    {
        var harness = new MostCommonWordBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
