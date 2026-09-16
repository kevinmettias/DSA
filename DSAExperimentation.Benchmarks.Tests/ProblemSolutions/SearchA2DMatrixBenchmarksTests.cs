using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchA2DMatrixBenchmarks (ARCHITECTURE 17.9): both arms search the one
// matrix for the one target, so a harness whose arms disagree is timing two different problems.
// Setup numbers every cell 0..Size*Size-1 in row order without any randomness, so the
// benchmark's target - that last value - is present by construction; the Setup test asserts that
// presence as well as the rebuild, because a reintroduced per-row counter would silently make the
// searched value absent and turn both arms into the -1 fast path.
//
// Weak by construction, and reported as such with this batch: each arm answers with a single bool
// for the same (matrix, target) query, so once the target is known present every run of either arm
// is true. Agreement witnesses that neither arm reports "absent" for a value the workload
// guarantees is there, but cannot distinguish the two strategies' search behaviour any further.
// Strengthening it would mean changing an arm's return type, which is not this harness's call.
public sealed partial class SearchA2DMatrixBenchmarksTests
{
    private const int SmallestSize = 40;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().HasTargetByLinearScan(), BuildHarness().HasTargetByLinearScan());
        Assert.True(BuildHarness().HasTargetByLinearScan());
    }

    [Fact]
    public void HasTargetByLinearScan_RowOrderGrid_AgreesWithHasTargetByBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByBinarySearch(), harness.HasTargetByLinearScan());
    }

    [Fact]
    public void HasTargetByBinarySearch_RowOrderGrid_AgreesWithHasTargetByLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByLinearScan(), harness.HasTargetByBinarySearch());
    }

    private static SearchA2DMatrixBenchmarks BuildHarness()
    {
        var harness = new SearchA2DMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
