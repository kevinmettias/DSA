using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TallestBillboardBenchmarks (ARCHITECTURE 17.9): both arms are
// TallestBillboardSolution's - the 3^RodCount un-memoized recursion against the same recursion behind
// this repo's Memoizer, one visit per (index, difference) state - so a harness whose arms disagree is
// timing two different questions. Both answer with a bare int, and the rods are a seeded draw, so a
// rebuild at the same RodCount has to produce the same height.
public sealed partial class TallestBillboardBenchmarksTests
{
    private const int SmallestRodCount = 12;

    [Fact]
    public void Setup_SameRodCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().UnmemoizedRecursion(),
            BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_SeededRodSet_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SeededRodSet_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static TallestBillboardBenchmarks BuildHarness()
    {
        var harness = new TallestBillboardBenchmarks { RodCount = SmallestRodCount };
        harness.Setup();

        return harness;
    }
}
