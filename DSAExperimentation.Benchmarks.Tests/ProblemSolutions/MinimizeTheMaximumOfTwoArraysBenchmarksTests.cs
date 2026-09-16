using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeTheMaximumOfTwoArraysBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a hand-rolled lo/hi bisection over the feasibility
// predicate against this repo's own BinarySearch.LowerBound over the same predicate re-expressed as an
// on-demand IRandomAccessSequence - so a harness whose arms disagree is timing two different problems.
// Both arms return the smallest maximum as an int, so they are compared directly; the two search the same
// candidate range in different directions (an inclusive hi that converges down against a leftmost-true
// index that maps back through the 1-based offset), so agreeing on the number also pins that the range and
// the offset agree. Both arms only read the divisors and the two unique counts, so one harness is safe to
// read twice in either order, and Setup derives those counts from UniqueCountScale alone.
public sealed partial class MinimizeTheMaximumOfTwoArraysBenchmarksTests
{
    private const int SmallestUniqueCountScale = 1_000;

    [Fact]
    public void Setup_SameUniqueCountScale_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ManualBinarySearch(), BuildHarness().ManualBinarySearch());

    [Fact]
    public void ManualBinarySearch_CoprimeDivisorsAndEqualCounts_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.ManualBinarySearch());
    }

    [Fact]
    public void SequenceLowerBound_CoprimeDivisorsAndEqualCounts_AgreesWithManualBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualBinarySearch(), harness.SequenceLowerBound());
    }

    private static MinimizeTheMaximumOfTwoArraysBenchmarks BuildHarness()
    {
        var harness = new MinimizeTheMaximumOfTwoArraysBenchmarks { UniqueCountScale = SmallestUniqueCountScale };
        harness.Setup();

        return harness;
    }
}
