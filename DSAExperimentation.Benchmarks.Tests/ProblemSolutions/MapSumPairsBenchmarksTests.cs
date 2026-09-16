using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MapSumPairsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the inserted keys for the query prefix against
// walking a trie and folding the subtotals - so a harness whose arms disagree is timing two
// different problems. Each arm builds its own strategy object inside the call, so no state is
// shared between the two calls and one harness is safe to call twice in either order. Both arms
// return the same inserted-then-summed total the replay accumulates. Setup draws the keys, values
// and query prefixes from one fixed seed and each query prefix is a real leading substring of an
// inserted key, so the same KeyCount must rebuild the same workload.
public sealed partial class MapSumPairsBenchmarksTests
{
    private const int SmallestKeyCount = 5_000;

    [Fact]
    public void Setup_SameKeyCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().DictionaryScan(), BuildHarness().DictionaryScan());
        Assert.Equal(BuildHarness().TrieFoldSum(), BuildHarness().TrieFoldSum());
    }

    [Fact]
    public void DictionaryScan_MatchingPrefixQueries_AgreesWithTrieFoldSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrieFoldSum(), harness.DictionaryScan());
    }

    [Fact]
    public void TrieFoldSum_MatchingPrefixQueries_AgreesWithDictionaryScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryScan(), harness.TrieFoldSum());
    }

    private static MapSumPairsBenchmarks BuildHarness()
    {
        var harness = new MapSumPairsBenchmarks { KeyCount = SmallestKeyCount };
        harness.Setup();

        return harness;
    }
}
