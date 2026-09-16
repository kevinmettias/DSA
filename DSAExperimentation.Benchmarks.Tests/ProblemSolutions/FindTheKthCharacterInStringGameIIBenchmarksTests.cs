using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheKthCharacterInStringGameIIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - materializing word and indexing into it
// against tracing k back down the doubling rounds - so a harness whose arms disagree is timing two
// different problems. Both are handed the same operations list and the same target position, and
// Setup derives both from OperationCount, so the same OperationCount must rebuild the same pair.
public sealed partial class FindTheKthCharacterInStringGameIIBenchmarksTests
{
    private const int SmallestOperationCount = 8;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForceSimulation(),
            BuildHarness().BruteForceSimulation());

    [Fact]
    public void BruteForceSimulation_SmallestOperationCount_AgreesWithBackwardTrace()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BackwardTrace(), harness.BruteForceSimulation());
    }

    [Fact]
    public void BackwardTrace_SmallestOperationCount_AgreesWithBruteForceSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSimulation(), harness.BackwardTrace());
    }

    private static FindTheKthCharacterInStringGameIIBenchmarks BuildHarness()
    {
        var harness = new FindTheKthCharacterInStringGameIIBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
