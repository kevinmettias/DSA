using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NRepeatedElementInSize2NArrayBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) pairwise scan against the O(n) Set<int> pass - so a harness whose
// arms disagree is timing two different problems. Both arms only read the value array built in [GlobalSetup], so
// one harness instance is safe to call twice in either order. Setup derives that array from Length alone (n
// distinct values, then n copies of the repeat), so the same Length must rebuild the same array; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class NRepeatedElementInSize2NArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArray() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_RepeatAtTheEnd_AgreesWithTrackingSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrackingSet(), harness.PairwiseScan());
    }

    [Fact]
    public void TrackingSet_RepeatAtTheEnd_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.TrackingSet());
    }

    private static NRepeatedElementInSize2NArrayBenchmarks BuildHarness()
    {
        var harness = new NRepeatedElementInSize2NArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
