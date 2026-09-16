using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMedianFromDataStreamBenchmarks (ARCHITECTURE 17.9): both arms are this
// problem's own design shape - an IMedianFinder replayed over one interleaved AddNum/FindMedian
// stream - and differ only in which implementation backs it, a re-sort on every query against two of
// this repo's own heaps. A harness whose arms disagree has replayed the same stream into two
// different answers. Each arm returns the last median of the run, a double, so it is compared under
// a named relative tolerance rather than by exact equality.
//
// The stream is seeded, so the same StreamLength must rebuild the same sequence of values and the
// same final median.
public sealed partial class FindMedianFromDataStreamBenchmarksTests
{
    // The smaller of Setup's [Params(100, 1_000)] stream lengths.
    private const int SmallestStreamLength = 100;

    // Both arms average the same two middle values of the same multiset, so agreement is expected to
    // the bit; the tolerance exists so a last-bit difference cannot fail the harness for the wrong
    // reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_SameStreamLength_RebuildsTheSameFinalMedian() =>
        Assert.Equal(
            BuildHarness().SortOnEveryQuery(),
            BuildHarness().SortOnEveryQuery(),
            RelativeTolerance);

    [Fact]
    public void SortOnEveryQuery_InterleavedStream_AgreesWithTwoHeaps()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoHeaps(), harness.SortOnEveryQuery(), RelativeTolerance);
    }

    [Fact]
    public void TwoHeaps_InterleavedStream_AgreesWithSortOnEveryQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortOnEveryQuery(), harness.TwoHeaps(), RelativeTolerance);
    }

    private static FindMedianFromDataStreamBenchmarks BuildHarness()
    {
        var harness = new FindMedianFromDataStreamBenchmarks { StreamLength = SmallestStreamLength };
        harness.Setup();

        return harness;
    }
}
