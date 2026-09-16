using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CapacityToShipPackagesWithinDDaysBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a hand-rolled lo/hi bisection of the monotone
// feasibility predicate against the same predicate searched through BinarySearch.LowerBound over an
// on-demand IRandomAccessSequence<bool> - so a harness whose arms disagree is timing two different
// predicates, and the class exists to show the reusable abstraction costs nothing. Setup draws the
// weights from one fixed seed and derives the day budget from Length, so the same Length must rebuild
// the same pair.
public sealed partial class CapacityToShipPackagesWithinDDaysBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ManualBinarySearch(), BuildHarness().ManualBinarySearch());

    [Fact]
    public void ManualBinarySearch_SeededWeightsAndDayBudget_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.ManualBinarySearch());
    }

    [Fact]
    public void SequenceLowerBound_SeededWeightsAndDayBudget_AgreesWithManualBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualBinarySearch(), harness.SequenceLowerBound());
    }

    private static CapacityToShipPackagesWithinDDaysBenchmarks BuildHarness()
    {
        var harness = new CapacityToShipPackagesWithinDDaysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
