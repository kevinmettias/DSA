using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EliminationGameBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rebuilding the surviving-numbers list every round against
// tracking only the head element and the current stride - so a harness whose arms disagree is timing
// two different problems. The class carries no [GlobalSetup]: both arms are handed the same
// NumberCount directly, and the survivor LC 390 reports is necessarily one of the numbers the board
// started with, so it always lies in [1, NumberCount].
public sealed partial class EliminationGameBenchmarksTests
{
    private const int SmallestNumberCount = 10_000;
    private const int FirstRemainingNumber = 1;

    [Fact]
    public void ListSimulation_TenThousandNumbers_AgreesWithHeadStepArithmetic()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeadStepArithmetic(), harness.ListSimulation());
    }

    [Fact]
    public void HeadStepArithmetic_TenThousandNumbers_SurvivesFromTheStartingBoardAndAgreesWithListSimulation()
    {
        var harness = BuildHarness();

        Assert.InRange(harness.HeadStepArithmetic(), FirstRemainingNumber, SmallestNumberCount);
        Assert.Equal(harness.ListSimulation(), harness.HeadStepArithmetic());
    }

    private static EliminationGameBenchmarks BuildHarness() => new() { NumberCount = SmallestNumberCount };
}
