using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberVBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the bottom-up table against the memoized recurrence over
// the same houses and colors - so a harness whose arms disagree is timing two different
// problems. Each arm answers with a single long, compared directly. Setup draws both the
// house values and the color assignment from one seeded stream, so the same HouseCount must
// rebuild the same pair of arrays and therefore the same maximum.
public sealed partial class HouseRobberVBenchmarksTests
{
    private const int SmallestHouseCount = 500;

    [Fact]
    public void Setup_SameHouseCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());

    [Fact]
    public void Tabulation_MaximumAmount_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_MaximumAmount_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static HouseRobberVBenchmarks BuildHarness()
    {
        var harness = new HouseRobberVBenchmarks { HouseCount = SmallestHouseCount };
        harness.Setup();

        return harness;
    }
}
