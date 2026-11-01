using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidBoomerangBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong count would pass, so this bounds the
// baseline's count from the fixture's construction.
//
// Both arms return only HOW MANY triples were boomerangs, so the count is all this harness can
// witness. It is bounded from the fixture: Setup's even-index triples put point 3 exactly on the
// line through points 1 and 2, so none of those can be a boomerang, and the count therefore cannot
// reach the full batch; the odd-index triples clear that line unless their vertical offset is zero,
// so at least one of them is a boomerang. Making the per-triple verdicts observable would need the
// arms' return type changed, which is a harness decision and not this file's.
public sealed partial class ValidBoomerangBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] triple counts.
    private const int SmallestLength = 200;

    // Setup alternates exact-collinear and near-collinear triples, so only the odd-indexed half can
    // ever be a boomerang.
    private const int CollinearBatchDivisor = 2;

    private const int MostBoomerangs = SmallestLength / CollinearBatchDivisor;

    // The odd-indexed triples are drawn off a 21-value offset range, so the fixture's own seed
    // leaves at least one of them genuinely non-collinear.
    private const int FewestBoomerangs = 1;

    [Fact]
    public void FloatingPointAreaCheck_AlternatingCollinearTriples_CountsWithinTheFixtureBounds() =>
        Assert.InRange(BuildHarness().FloatingPointAreaCheck(), FewestBoomerangs, MostBoomerangs);

    private static ValidBoomerangBenchmarks BuildHarness()
    {
        var harness = new ValidBoomerangBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
