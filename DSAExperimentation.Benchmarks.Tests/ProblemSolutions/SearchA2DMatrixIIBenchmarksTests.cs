using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchA2DMatrixIIBenchmarks (ARCHITECTURE 17.9): all three arms are the same
// problem's strategies for one (matrix, target) question, so a harness whose arms disagree is
// timing two different problems. Setup numbers every cell 0..Size*Size-1 in row order and the arms
// search for the class's own below-everything target, so the Setup test asserts the negative the
// fixture is built to force - which is also what keeps all three arms on their full worst-case
// walk instead of an early exit.
//
// Weak by construction, and reported as such with this batch: every arm answers with a single bool
// and the fixture pins the answer to false, so agreement witnesses only that none of the three ever
// reports "present" for a target below every cell. Strengthening it would mean changing an arm's
// return type, which is not this harness's call.
public sealed partial class SearchA2DMatrixIIBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().HasTargetByFullScan(), BuildHarness().HasTargetByFullScan());
        Assert.False(BuildHarness().HasTargetByFullScan());
    }

    [Fact]
    public void HasTargetByFullScan_BelowRangeTarget_AgreesWithHasTargetByPerRowBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByPerRowBinarySearch(), harness.HasTargetByFullScan());
    }

    [Fact]
    public void HasTargetByPerRowBinarySearch_BelowRangeTarget_AgreesWithHasTargetByStaircaseSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByStaircaseSearch(), harness.HasTargetByPerRowBinarySearch());
    }

    [Fact]
    public void HasTargetByStaircaseSearch_BelowRangeTarget_AgreesWithHasTargetByFullScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByFullScan(), harness.HasTargetByStaircaseSearch());
    }

    private static SearchA2DMatrixIIBenchmarks BuildHarness()
    {
        var harness = new SearchA2DMatrixIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
