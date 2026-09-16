using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LastDayWhereYouCanStillCrossBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - bisecting the flood schedule by hand against the
// sequence's own lower-bound search - so a harness whose arms disagree is timing two different
// problems. Setup builds the FloodSchedule from one fixed seed, so the same Size must rebuild the
// same flood order; otherwise two published numbers were never comparable in the first place.
public sealed partial class LastDayWhereYouCanStillCrossBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameFloodSchedule() =>
        Assert.Equal(BuildHarness().ManualBinarySearch(), BuildHarness().ManualBinarySearch());

    [Fact]
    public void ManualBinarySearch_SeededFloodOrder_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.ManualBinarySearch());
    }

    [Fact]
    public void SequenceLowerBound_SeededFloodOrder_AgreesWithManualBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualBinarySearch(), harness.SequenceLowerBound());
    }

    private static LastDayWhereYouCanStillCrossBenchmarks BuildHarness()
    {
        var harness = new LastDayWhereYouCanStillCrossBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
