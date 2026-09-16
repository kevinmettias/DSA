using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CandyBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for
// the same question - repeatedly relaxing both neighbour constraints until nothing changes against
// the two-pass left-to-right then right-to-left slope sweep - so a harness whose arms disagree is
// timing two different problems. Setup derives a strictly decreasing rating run, which the class
// comment names as the worst case for repeated relaxation: each pass propagates one extra unit of
// "must exceed my right neighbour" one position further left, so the baseline really does O(n) passes
// of O(n). Setup has no seed to fix - the run is a function of Length alone - so the same Length must
// rebuild the same run and with it the same answer.
public sealed partial class CandyBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RepeatedRelaxation(), BuildHarness().RepeatedRelaxation());

    [Fact]
    public void RepeatedRelaxation_StrictlyDecreasingRatings_AgreesWithTwoPassSlopeConstraints()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoPassSlopeConstraints(), harness.RepeatedRelaxation());
    }

    [Fact]
    public void TwoPassSlopeConstraints_StrictlyDecreasingRatings_AgreesWithRepeatedRelaxation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedRelaxation(), harness.TwoPassSlopeConstraints());
    }

    private static CandyBenchmarks BuildHarness()
    {
        var harness = new CandyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
