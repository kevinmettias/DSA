using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CreateSortedArrayThroughInstructionsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the textbook rescan of everything inserted so far
// against one Fenwick-tree sweep - so a harness whose arms disagree is timing two different problems.
// Setup draws the instruction values from one fixed seed; every insertion costs at most the number of
// values already inserted, so a run of Length values can never total more than the triangle number,
// which is the documented ceiling the reading has to sit under. The same Length must rebuild the same
// instructions and with them the same total.
public sealed partial class CreateSortedArrayThroughInstructionsBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int MinimumInstructionCostTotal = 0;
    private const int MaximumInstructionCostTotal = SmallestLength * (SmallestLength - 1) / 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().PairwiseScan(),
            MinimumInstructionCostTotal,
            MaximumInstructionCostTotal);
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());
    }

    [Fact]
    public void PairwiseScan_TwoHundredSeededInstructions_AgreesWithFenwickTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickTreeSweep(), harness.PairwiseScan());
    }

    [Fact]
    public void FenwickTreeSweep_TwoHundredSeededInstructions_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.FenwickTreeSweep());
    }

    private static CreateSortedArrayThroughInstructionsBenchmarks BuildHarness()
    {
        var harness = new CreateSortedArrayThroughInstructionsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
