using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RangeSumQuery2DImmutableBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the sum of every region the query batch asks for -
// so a harness whose arms disagree is timing two different problems. Each arm's factory builds its
// own instance inside the call and the replay sums the returned region sums, so one harness is safe
// to call twice in either order. Setup builds the random matrix and the query batch from one fixed
// seed, so the same Size must rebuild the same pair.
public sealed partial class RangeSumQuery2DImmutableBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceCellScan(), BuildHarness().BruteForceCellScan());

    [Fact]
    public void BruteForceCellScan_SmallestSize_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceCellScan(), harness.RowFenwickTreeQuery());
    }

    [Fact]
    public void RowFenwickTreeQuery_SmallestSize_AgreesWithTheBruteForceArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowFenwickTreeQuery(), harness.BruteForceCellScan());
    }

    private static RangeSumQuery2DImmutableBenchmarks BuildHarness()
    {
        var harness = new RangeSumQuery2DImmutableBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
