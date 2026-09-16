using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxPointsOnALineBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - grouping by reduced cross-product direction against grouping by
// the slope ratio itself - so a harness whose arms disagree is timing two different problems. Both
// arms return the largest number of points on one line, a count LC 149 pins exactly. Setup draws the
// coordinates from one fixed seed over a range wide enough that the answer stays small, so the same
// Length must rebuild the same points and the same count.
public sealed partial class MaxPointsOnALineBenchmarksTests
{
    private const int SmallestLength = 600;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().CrossProduct(), BuildHarness().CrossProduct());
        Assert.Equal(BuildHarness().SlopeGrouping(), BuildHarness().SlopeGrouping());
    }

    [Fact]
    public void CrossProduct_WideCoordinateScatter_AgreesWithSlopeGrouping()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlopeGrouping(), harness.CrossProduct());
    }

    [Fact]
    public void SlopeGrouping_WideCoordinateScatter_AgreesWithCrossProduct()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CrossProduct(), harness.SlopeGrouping());
    }

    private static MaxPointsOnALineBenchmarks BuildHarness()
    {
        var harness = new MaxPointsOnALineBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
