using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PossibleBipartitionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - can the generated dislikes split the people into two groups? - so a
// harness whose arms disagree is timing two different problems. PersonCount is the only [Params] axis
// and Setup derives the pair list from it, so the same PersonCount must rebuild the same dislikes.
//
// Both arms answer with one bool, and Setup's generator builds the dislikes strictly across a fixed
// A/B split, so the fixture's verdict is always "yes". The agreement witnessed here is therefore
// weak by construction: it catches an arm that ever says no, but an arm stuck on yes would pass it.
// That is the fixture's design, not a gap in this assertion.
public sealed partial class PossibleBipartitionBenchmarksTests
{
    private const int SmallestPersonCount = 200;

    [Fact]
    public void Setup_SamePersonCount_RebuildsTheSameDislikes() =>
        Assert.Equal(
            BuildHarness().CanBipartitionByColorArrayDfs(),
            BuildHarness().CanBipartitionByColorArrayDfs());

    [Fact]
    public void CanBipartitionByColorArrayDfs_AgreesWithBipartiteCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanBipartitionByBipartiteCheck(),
            harness.CanBipartitionByColorArrayDfs());
    }

    [Fact]
    public void CanBipartitionByBipartiteCheck_AgreesWithColorArrayDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CanBipartitionByColorArrayDfs(),
            harness.CanBipartitionByBipartiteCheck());
    }

    private static PossibleBipartitionBenchmarks BuildHarness()
    {
        var harness = new PossibleBipartitionBenchmarks { PersonCount = SmallestPersonCount };
        harness.Setup();

        return harness;
    }
}
