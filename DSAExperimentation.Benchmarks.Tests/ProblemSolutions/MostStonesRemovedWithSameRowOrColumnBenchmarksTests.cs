using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MostStonesRemovedWithSameRowOrColumnBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) pair sweep against the O(n) axis-keyed unions - so
// a harness whose arms disagree is timing two different problems. Both arms only read the stone field built
// in [GlobalSetup], so one harness instance is safe to call twice in either order. Setup draws that field from
// one fixed seed, so the same StoneCount must rebuild the same stones; otherwise two published numbers were
// never comparable in the first place.
public sealed partial class MostStonesRemovedWithSameRowOrColumnBenchmarksTests
{
    private const int SmallestStoneCount = 200;

    [Fact]
    public void Setup_SameStoneCount_RebuildsTheSameStoneField() =>
        Assert.Equal(BuildHarness().PairwiseScanThenUnion(), BuildHarness().PairwiseScanThenUnion());

    [Fact]
    public void PairwiseScanThenUnion_DistinctSeededStones_AgreesWithRowColumnKeyedUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowColumnKeyedUnion(), harness.PairwiseScanThenUnion());
    }

    [Fact]
    public void RowColumnKeyedUnion_DistinctSeededStones_AgreesWithPairwiseScanThenUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScanThenUnion(), harness.RowColumnKeyedUnion());
    }

    private static MostStonesRemovedWithSameRowOrColumnBenchmarks BuildHarness()
    {
        var harness = new MostStonesRemovedWithSameRowOrColumnBenchmarks { StoneCount = SmallestStoneCount };
        harness.Setup();

        return harness;
    }
}
