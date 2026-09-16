using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RandomPickWithWeightBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an index drawn with probability proportional to its
// weight - so a harness whose arms disagree is timing two different problems.
//
// Both arms are given their own Random(1) and both turn one draw over the total weight into an
// index the same way (the first prefix sum strictly greater than the draw), so the seeded streams
// advance identically and the two running totals must be exactly equal: here the agreement really
// is value for value, not merely distributional. Setup draws the weights from one fixed seed, so
// the same WeightCount must rebuild the same weights.
public sealed partial class RandomPickWithWeightBenchmarksTests
{
    private const int SmallestWeightCount = 50;

    [Fact]
    public void Setup_SameWeightCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_SmallestWeightCount_DrawsTheSameSeededSequence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchUpperBound());
    }

    [Fact]
    public void BinarySearchUpperBound_SmallestWeightCount_DrawsTheSameSeededSequence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchUpperBound(), harness.LinearScan());
    }

    private static RandomPickWithWeightBenchmarks BuildHarness()
    {
        var harness = new RandomPickWithWeightBenchmarks { WeightCount = SmallestWeightCount };
        harness.Setup();

        return harness;
    }
}
