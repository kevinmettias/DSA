using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks (ARCHITECTURE 17.9):
// Setup draws both the visible person's position and the visible count from one fixed seed and
// reads them only through the arms, so the strongest observable property known without either arm
// is that the answer lies inside the residue range LC 3881's own modulus allows.
public sealed partial class DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarksTests
{
    private const int SmallestPersonCount = 200;

    // LC 3881's answers are reported modulo 10^9 + 7, so no residue ever reaches the modulus
    // itself - one below it is the largest value the workload may report.
    private const int DirectionAssignmentResidueUpperBound = 1_000_000_006;

    [Fact]
    public void PascalConvolution_SeededPositionAndVisibleCount_StaysInsideTheProblemModulus() =>
        Assert.InRange(BuildHarness().PascalConvolution(), 0, DirectionAssignmentResidueUpperBound);

    private static DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks BuildHarness()
    {
        var harness = new DirectionAssignmentsWithExactlyKVisiblePeopleBenchmarks { PersonCount = SmallestPersonCount };
        harness.Setup();

        return harness;
    }
}
