using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LastStoneWeightIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the half-capacity subset recurrence filled bottom-up against
// the same recurrence memoized top-down - so a harness whose arms disagree is timing two different
// problems. Setup draws the weights from one fixed seed, so the same Length must rebuild the same
// pile; otherwise two published numbers were never comparable in the first place.
public sealed partial class LastStoneWeightIIBenchmarksTests
{
    private const int SmallestLength = 30;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePile() =>
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());

    [Fact]
    public void Tabulation_RandomWeights_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_RandomWeights_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static LastStoneWeightIIBenchmarks BuildHarness()
    {
        var harness = new LastStoneWeightIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
