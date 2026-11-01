using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInRotatedSortedArrayIIBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the verdict the workload is built to have. The array comes from
// the shared RotatedSortedArrayWorkloads fixture with no randomness, and the target sits inside the
// pre-rotation segment, which the duplicate banding only overwrites at the two ends - so the target
// is present by construction and the test can assert that positive.
public sealed partial class SearchInRotatedSortedArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void HasTargetByLinearScan_DuplicateBoundaryBand_FindsTheTarget() =>
        Assert.True(BuildHarness().HasTargetByLinearScan());

    private static SearchInRotatedSortedArrayIIBenchmarks BuildHarness()
    {
        var harness = new SearchInRotatedSortedArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
