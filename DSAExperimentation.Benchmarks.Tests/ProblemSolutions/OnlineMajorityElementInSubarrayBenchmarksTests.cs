using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OnlineMajorityElementInSubarrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing checkers for the same queries - a per-query range tally against the HashMap position
// index - so a harness whose arms disagree is timing two different problems. Setup builds both
// checkers and the query list from one seeded Random, so the same Length must rebuild the same
// workload.
//
// WEAK BY CONSTRUCTION, and reported as such: each arm returns the sum of its answers over every
// generated query rather than the answers themselves, so agreement witnesses that the two checkers
// agree on the total. Two checkers that disagreed on two queries by equal and opposite amounts would
// still sum the same. Values that differ per query would need each answer compared, which the arms'
// long return type does not offer.
public sealed partial class OnlineMajorityElementInSubarrayBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().PositionIndexWithBinarySearch(),
            BuildHarness().PositionIndexWithBinarySearch());

    [Fact]
    public void TallyEveryValueInRange_SmallestLength_AgreesWithPositionIndexWithBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PositionIndexWithBinarySearch(), harness.TallyEveryValueInRange());
    }

    [Fact]
    public void PositionIndexWithBinarySearch_SmallestLength_AgreesWithTallyEveryValueInRange()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TallyEveryValueInRange(), harness.PositionIndexWithBinarySearch());
    }

    private static OnlineMajorityElementInSubarrayBenchmarks BuildHarness()
    {
        var harness = new OnlineMajorityElementInSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
