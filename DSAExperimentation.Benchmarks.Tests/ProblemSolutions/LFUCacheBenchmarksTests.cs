using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LFUCacheBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - what LC 460's least-frequently-used cache answers when the same
// call script is replayed against it - so a harness whose arms disagree is timing two different
// problems. Each arm creates its own cache inside the call and the script is a fixed list of
// stateless closures over whatever cache they are handed, so one harness instance is safe to call
// twice in either order; the arms cannot share eviction state through it.
//
// Each arm answers with the sum of every value the script's gets returned. That sum is an
// aggregate over the whole replay rather than the cache's final contents, so it would move if an
// eviction differed (a hit and a miss contribute different amounts), but it is not a witness that
// the two caches held the same entries at any point.
public sealed partial class LFUCacheBenchmarksTests
{
    private const int SmallestCapacity = 200;

    [Fact]
    public void Setup_SameCapacity_RebuildsTheSameCallScript() =>
        Assert.Equal(
            BuildHarness().DictionaryLinearScan(),
            BuildHarness().DictionaryLinearScan());

    [Fact]
    public void DictionaryLinearScan_SeededCallScript_AgreesWithLfuCachePrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LfuCachePrimitive(), harness.DictionaryLinearScan());
    }

    [Fact]
    public void LfuCachePrimitive_SeededCallScript_AgreesWithDictionaryLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryLinearScan(), harness.LfuCachePrimitive());
    }

    private static LFUCacheBenchmarks BuildHarness()
    {
        var harness = new LFUCacheBenchmarks { Capacity = SmallestCapacity };
        harness.Setup();

        return harness;
    }
}
