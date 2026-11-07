using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for BitboardGameBenchmarks (ARCHITECTURE 17.9). The generic check compares the
// arms on the smallest parameters only, so every opening's arms are compared here at both depths.
public sealed partial class BitboardGameBenchmarksTests
{
    public static TheoryData<int, int> Workloads => new()
    {
        { 2, 0 }, { 2, 1 }, { 4, 0 }, { 4, 1 },
    };

    [Theory]
    [MemberData(nameof(Workloads))]
    public void Arms_EveryOpeningAndDepth_AnswerAlike(int depth, int opening)
    {
        var harness = new BitboardGameBenchmarks { Depth = depth, Opening = opening };
        harness.Setup();

        Assert.Equal(harness.SetMasks(), harness.Shifts());
    }
}
