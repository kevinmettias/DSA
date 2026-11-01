using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CreateSortedArrayThroughInstructionsBenchmarks (ARCHITECTURE 17.9): the generic
// arm check compares the arms only with each other, so what stays here is a ceiling derived without
// either arm. Every insertion costs at most the number of values already inserted, so a run of Length
// values can never total more than the triangle number, which is the documented ceiling the reading
// has to sit under.
public sealed partial class CreateSortedArrayThroughInstructionsBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int MinimumInstructionCostTotal = 0;
    private const int MaximumInstructionCostTotal = SmallestLength * (SmallestLength - 1) / 2;

    [Fact]
    public void PairwiseScan_TwoHundredSeededInstructions_StaysUnderTheTriangleNumber() =>
        Assert.InRange(
            BuildHarness().PairwiseScan(),
            MinimumInstructionCostTotal,
            MaximumInstructionCostTotal);

    private static CreateSortedArrayThroughInstructionsBenchmarks BuildHarness()
    {
        var harness = new CreateSortedArrayThroughInstructionsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
