using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LastRemainingIntegerAfterAlternatingDeletionOperationsBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - replaying the
// alternating deletions over a list against the head/step closed form - so a harness whose arms
// disagree is timing two different problems.
//
// There is no [GlobalSetup] here: StartingCount is the whole input, so there is no prepared workload
// to rebuild and no Setup member to cover. Each arm is a pure function of that one parameter, which
// is why both tests build the harness directly.
public sealed partial class LastRemainingIntegerAfterAlternatingDeletionOperationsBenchmarksTests
{
    private const long SmallestStartingCount = 10_000;

    [Fact]
    public void ListSimulation_TenThousandIntegers_AgreesWithHeadStepSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeadStepSimulation(), harness.ListSimulation());
    }

    [Fact]
    public void HeadStepSimulation_TenThousandIntegers_AgreesWithListSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListSimulation(), harness.HeadStepSimulation());
    }

    private static LastRemainingIntegerAfterAlternatingDeletionOperationsBenchmarks BuildHarness() =>
        new() { StartingCount = SmallestStartingCount };
}
