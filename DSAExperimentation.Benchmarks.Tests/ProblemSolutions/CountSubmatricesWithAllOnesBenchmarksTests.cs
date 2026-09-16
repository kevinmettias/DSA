using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubmatricesWithAllOnesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the running-minimum scan against the monotonic-stack
// reduction to LC 907 - so a harness whose arms disagree is timing two different problems, not two
// ways of answering one. Setup draws the binary matrix from one fixed seed, so the same Size must
// rebuild the same matrix; otherwise two published numbers were never comparable in the first place.
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
    public void Setup_SameSize_RebuildsTheSameMatrix()
    {
        Assert.InRange(BuildHarness().RunningMinScan(), 0, SubmatrixCount);
        Assert.Equal(BuildHarness().RunningMinScan(), BuildHarness().RunningMinScan());
    }

    [Fact]
    public void RunningMinScan_SeededBinaryMatrix_AgreesWithMonotonicStackDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackDp(), harness.RunningMinScan());
    }

    [Fact]
    public void MonotonicStackDp_SeededBinaryMatrix_AgreesWithRunningMinScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningMinScan(), harness.MonotonicStackDp());
    }

    private static CountSubmatricesWithAllOnesBenchmarks BuildHarness()
    {
        var harness = new CountSubmatricesWithAllOnesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
