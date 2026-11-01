using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubmatricesWithAllOnesBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes
// from the workload's shape rather than from either arm.
//
// The matrix is private and the all-ones submatrix count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts submatrices of a Size x Size binary
// matrix, and a matrix that size holds exactly (Size * (Size + 1) / 2)^2 submatrices in total.
public sealed partial class CountSubmatricesWithAllOnesBenchmarksTests
{
    private const int SmallestSize = 50;

    private const int SubmatricesPerAxis = SmallestSize * (SmallestSize + 1) / 2;

    private const int SubmatrixCount = SubmatricesPerAxis * SubmatricesPerAxis;

    [Fact]
    public void RunningMinScan_SmallestSize_StaysWithinTheSubmatrixCount() =>
        Assert.InRange(BuildHarness().RunningMinScan(), 0, SubmatrixCount);

    private static CountSubmatricesWithAllOnesBenchmarks BuildHarness()
    {
        var harness = new CountSubmatricesWithAllOnesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
