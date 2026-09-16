using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - convolving two Pascal rows against
// the single binomial Vandermonde's identity collapses them to - so a harness whose arms disagree
// is timing two different problems. Setup draws both the visible person's position and the visible
// count from one fixed seed and reads them only through the arms, so the strongest observable
// workload property is that the shared answer lies inside the residue range LC 3881's own modulus
// allows, together with the rebuild producing that same residue.
public sealed partial class DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarksTests
{
    private const int SmallestPersonCount = 200;

    // LC 3881's answers are reported modulo 10^9 + 7, so no residue ever reaches the modulus
    // itself - one below it is the largest value the workload may report.
    private const int DirectionAssignmentResidueUpperBound = 1_000_000_006;

    [Fact]
    public void Setup_SeededPositionAndVisibleCount_StayInsideTheProblemModulusAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();
        var assignments = harness.PascalConvolution();

        Assert.InRange(assignments, 0, DirectionAssignmentResidueUpperBound);
        Assert.Equal(assignments, BuildHarness().PascalConvolution());
    }

    [Fact]
    public void PascalConvolution_SeededPositionAndVisibleCount_AgreesWithVandermondeIdentity()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.VandermondeIdentity(), harness.PascalConvolution());
    }

    [Fact]
    public void VandermondeIdentity_SeededPositionAndVisibleCount_AgreesWithPascalConvolution()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PascalConvolution(), harness.VandermondeIdentity());
    }

    private static DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks BuildHarness()
    {
        var harness = new DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks { PersonCount = SmallestPersonCount };
        harness.Setup();

        return harness;
    }
}
