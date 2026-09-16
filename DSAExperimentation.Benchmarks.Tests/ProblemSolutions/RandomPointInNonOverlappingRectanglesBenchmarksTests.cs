using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RandomPointInNonOverlappingRectanglesBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - an area-weighted uniformly random point in
// one of the rectangles - so a harness whose arms disagree is timing two different problems.
//
// Both arms are given their own Random(1), draw once over the total area, resolve that draw to the
// same rectangle index (the first prefix area strictly greater than the draw), then draw the point
// inside it with the same two calls - so the seeded streams advance in lockstep and the two summed
// coordinate totals must be exactly equal. Here the agreement really is value for value. Setup
// builds the rectangles and the prefix areas from one fixed seed, so the same RectangleCount must
// rebuild them.
public sealed partial class RandomPointInNonOverlappingRectanglesBenchmarksTests
{
    private const int SmallestRectangleCount = 50;

    [Fact]
    public void Setup_SameRectangleCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_SmallestRectangleCount_DrawsTheSameSeededSequence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchUpperBound());
    }

    [Fact]
    public void BinarySearchUpperBound_SmallestRectangleCount_DrawsTheSameSeededSequence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchUpperBound(), harness.LinearScan());
    }

    private static RandomPointInNonOverlappingRectanglesBenchmarks BuildHarness()
    {
        var harness = new RandomPointInNonOverlappingRectanglesBenchmarks { RectangleCount = SmallestRectangleCount };
        harness.Setup();

        return harness;
    }
}
