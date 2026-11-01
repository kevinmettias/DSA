using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HandOfStraightsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture
// fixes independently of either arm. The hand is laid down as whole, non-overlapping runs of
// exactly GroupSize consecutive values and then shuffled, so it always straightens, and that is
// what forces each strategy through its full group-forming sweep instead of returning early on the
// first missing card.
public sealed partial class HandOfStraightsBenchmarksTests
{
    private const int SmallestHandCount = 500;

    // The fixture's runs are complete groups of GroupSize consecutive values, so the hand always
    // straightens.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void IsNStraightHandByBclDictionary_WholeConsecutiveRuns_Straightens() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsNStraightHandByBclDictionary());

    private static HandOfStraightsBenchmarks BuildHarness()
    {
        var harness = new HandOfStraightsBenchmarks { HandCount = SmallestHandCount };
        harness.Setup();

        return harness;
    }
}
