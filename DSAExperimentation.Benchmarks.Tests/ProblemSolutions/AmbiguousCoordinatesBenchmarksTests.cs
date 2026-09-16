using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AmbiguousCoordinatesBenchmarks (ARCHITECTURE 17.9): its two arms are
// AmbiguousCoordinatesSolution's competing strategies for the same question - rebuild every dotted candidate and
// re-scan the finished string against slicing the split point directly - so a harness whose arms disagree is
// reporting two different coordinate sets. Both enumerate the same split points, whole forms before dotted ones,
// and the solution documents that each offered form is already in the scan's own order, so the outer order is
// pinned by construction and the harness compares positionally. Setup builds the parenthesized digit run from one
// seed, so the same Length must rebuild the same run.
public sealed partial class AmbiguousCoordinatesBenchmarksTests
{
    // The smaller of Setup's [Params(8, 16)] digit-run lengths.
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RebuildAndRescan()),
            AnswerText.Of(BuildHarness().RebuildAndRescan()));

    [Fact]
    public void RebuildAndRescan_EightDigitRun_AgreesWithSliceAndCheckBoundary()
    {
        var harness = BuildHarness();

        Assert.NotEmpty(harness.RebuildAndRescan());
        Assert.Equal(AnswerText.Of(harness.SliceAndCheckBoundary()), AnswerText.Of(harness.RebuildAndRescan()));
    }

    [Fact]
    public void SliceAndCheckBoundary_EightDigitRun_AgreesWithRebuildAndRescan()
    {
        var harness = BuildHarness();

        Assert.NotEmpty(harness.SliceAndCheckBoundary());
        Assert.Equal(AnswerText.Of(harness.RebuildAndRescan()), AnswerText.Of(harness.SliceAndCheckBoundary()));
    }

    private static AmbiguousCoordinatesBenchmarks BuildHarness()
    {
        var harness = new AmbiguousCoordinatesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
