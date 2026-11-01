using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidSquareBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong count would pass, so this pins the baseline's
// count to the one the fixture's construction fixes.
//
// The arms return only HOW MANY quads were valid, so the count is all this harness can witness. It
// is decisive on this fixture: Setup alternates exact axis-aligned squares with four independently
// random points, and a random quad is an exact square with probability zero, so the batch's answer
// is exactly half its size. Making the per-quad verdicts observable would need the arms' return
// type changed, which is a harness decision and not this file's.
public sealed partial class ValidSquareBenchmarksTests
{
    // The smaller of Setup's [Params(5_000, 100_000)] batch counts.
    private const int SmallestBatchCount = 5_000;

    // Setup alternates square batches with random quads, so exactly the even-indexed half is valid.
    private const int SquareBatchDivisor = 2;

    private const int ExpectedValidQuadCount = SmallestBatchCount / SquareBatchDivisor;

    [Fact]
    public void MinMaxScan_AlternatingSquaresAndRandomQuads_CountsHalfTheBatch() =>
        Assert.Equal(ExpectedValidQuadCount, BuildHarness().MinMaxScan());

    private static ValidSquareBenchmarks BuildHarness()
    {
        var harness = new ValidSquareBenchmarks { BatchCount = SmallestBatchCount };
        harness.Setup();

        return harness;
    }
}
