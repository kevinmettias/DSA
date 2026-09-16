using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidBoomerangBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidBoomerangSolution's competing strategies for the same question - the floating-point Heron
// area check, which needs an epsilon, against the exact-integer cross product - so a harness whose
// arms disagree is classifying two different sets of triples. Both arms walk the same _triples, so
// one harness instance is safe to hand to both.
//
// This agreement is weak by construction, and the weakness is worth stating plainly rather than
// dressing up: both arms return only HOW MANY triples were boomerangs, so agreeing on that count
// witnesses that the two classifiers agree on every triple's verdict only in aggregate - one arm
// could reject a triple another accepted and the totals would still match. The count is at least
// bounded from the fixture, though: Setup's even-index triples put point 3 exactly on the line
// through points 1 and 2, so none of those can be a boomerang, and the count therefore cannot reach
// the full batch; the odd-index triples clear that line unless their vertical offset is zero, so at
// least one of them is a boomerang. Those two fixture-derived bounds are asserted alongside the
// agreement. Making the per-triple verdicts observable would need the arms' return type changed,
// which is a harness decision and not this file's.
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
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().IntegerCrossProduct(), BuildHarness().IntegerCrossProduct());

    [Fact]
    public void FloatingPointAreaCheck_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntegerCrossProduct(), harness.FloatingPointAreaCheck());
        Assert.InRange(harness.FloatingPointAreaCheck(), FewestBoomerangs, MostBoomerangs);
    }

    [Fact]
    public void IntegerCrossProduct_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FloatingPointAreaCheck(), harness.IntegerCrossProduct());
        Assert.InRange(harness.IntegerCrossProduct(), FewestBoomerangs, MostBoomerangs);
    }

    private static ValidBoomerangBenchmarks BuildHarness()
    {
        var harness = new ValidBoomerangBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
