using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheKthCharacterInStringGameIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for one question - growing word one round at a time against the
// closed-form popcount of k - so a harness whose arms disagree is timing two different problems.
// The class has no Setup, so KthPosition is the whole workload.
public sealed partial class FindTheKthCharacterInStringGameIBenchmarksTests
{
    private const int SmallestKthPosition = 10;

    [Fact]
    public void Simulation_SmallestKthPosition_AgreesWithBitCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitCount(), harness.Simulation());
    }

    [Fact]
    public void BitCount_SmallestKthPosition_AgreesWithSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Simulation(), harness.BitCount());
    }

    private static FindTheKthCharacterInStringGameIBenchmarks BuildHarness() =>
        new() { KthPosition = SmallestKthPosition };
}
