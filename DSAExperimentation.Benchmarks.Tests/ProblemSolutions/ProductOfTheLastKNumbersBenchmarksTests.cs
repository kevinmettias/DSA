using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProductOfTheLastKNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the product of the last Length numbers added to the stream -
// so a harness whose arms disagree is timing two different problems. Length is the only [Params] axis
// and Setup seeds both stores from it, so the same Length must rebuild the same stream.
//
// The two stores are fields rather than locals, so arm order is only safe while neither arm mutates:
// both answer GetProduct, which reads and never appends, so one harness serves both arms in either
// order - which is what asserting both orders here demonstrates.
public sealed partial class ProductOfTheLastKNumbersBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameStreams() =>
        Assert.Equal(BuildHarness().ReplayLastKFromRawStream(), BuildHarness().ReplayLastKFromRawStream());

    [Fact]
    public void ReplayLastKFromRawStream_AgreesWithPrefixProductDivision()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixProductDivision(), harness.ReplayLastKFromRawStream());
    }

    [Fact]
    public void PrefixProductDivision_AgreesWithReplayLastKFromRawStream()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReplayLastKFromRawStream(), harness.PrefixProductDivision());
    }

    private static ProductOfTheLastKNumbersBenchmarks BuildHarness()
    {
        var harness = new ProductOfTheLastKNumbersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
