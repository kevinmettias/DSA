using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidSquareBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidSquareSolution's competing strategies for the same question - the six-distance min/max scan
// against sorting the same distances - so a harness whose arms disagree is classifying two different
// batches of quads. Both arms walk the same _batches, so one harness instance is safe to hand to
// both.
//
// The agreement is weak by construction, and it is worth saying so plainly: the arms return only HOW
// MANY quads were valid, so agreeing on the count witnesses that the two classifiers agree on every
// quad's verdict only in aggregate, and one arm accepting a quad the other rejected would still
// total the same. The count is decisive on this fixture, though, and is asserted alongside the
// agreement: Setup alternates exact axis-aligned squares with four independently random points, and
// a random quad is an exact square with probability zero, so the batch's answer is exactly half its
// size. Making the per-quad verdicts observable would need the arms' return type changed, which is a
// harness decision and not this file's.
public sealed partial class ValidSquareBenchmarksTests
{
    // The smaller of Setup's [Params(5_000, 100_000)] batch counts.
    private const int SmallestBatchCount = 5_000;

    // Setup alternates square batches with random quads, so exactly the even-indexed half is valid.
    private const int SquareBatchDivisor = 2;

    private const int ExpectedValidQuadCount = SmallestBatchCount / SquareBatchDivisor;

    [Fact]
    public void Setup_SameBatchCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MinMaxScan(), BuildHarness().MinMaxScan());

    [Fact]
    public void MinMaxScan_SmallestBatchCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedValidQuadCount, harness.MinMaxScan());
        Assert.Equal(harness.MergeSort(), harness.MinMaxScan());
    }

    [Fact]
    public void MergeSort_SmallestBatchCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedValidQuadCount, harness.MergeSort());
        Assert.Equal(harness.MinMaxScan(), harness.MergeSort());
    }

    private static ValidSquareBenchmarks BuildHarness()
    {
        var harness = new ValidSquareBenchmarks { BatchCount = SmallestBatchCount };
        harness.Setup();

        return harness;
    }
}
