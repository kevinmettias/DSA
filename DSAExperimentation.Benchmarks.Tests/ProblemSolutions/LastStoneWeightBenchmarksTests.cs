using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LastStoneWeightBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the array for the two heaviest stones against a max
// heap - so a harness whose arms disagree is timing two different problems. Setup draws the weights
// from one fixed seed over a range wide enough that the smashes keep re-inserting crushed stones
// rather than annihilating them, so the same StoneCount must rebuild the same pile; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class LastStoneWeightBenchmarksTests
{
    private const int SmallestStoneCount = 200;

    [Fact]
    public void Setup_SameStoneCount_RebuildsTheSamePile() =>
        Assert.Equal(BuildHarness().LinearRescanEachSmash(), BuildHarness().LinearRescanEachSmash());

    [Fact]
    public void LinearRescanEachSmash_RandomWeights_AgreesWithMaxHeapSmash()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MaxHeapSmash(), harness.LinearRescanEachSmash());
    }

    [Fact]
    public void MaxHeapSmash_RandomWeights_AgreesWithLinearRescanEachSmash()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearRescanEachSmash(), harness.MaxHeapSmash());
    }

    private static LastStoneWeightBenchmarks BuildHarness()
    {
        var harness = new LastStoneWeightBenchmarks { StoneCount = SmallestStoneCount };
        harness.Setup();

        return harness;
    }
}
