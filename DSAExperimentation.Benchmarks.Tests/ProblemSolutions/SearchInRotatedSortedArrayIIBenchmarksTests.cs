using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInRotatedSortedArrayIIBenchmarks (ARCHITECTURE 17.9): both arms answer
// the same membership question about the same rotated, duplicate-banded array, so a harness whose
// arms disagree is timing two different problems. The array comes from the shared
// RotatedSortedArrayWorkloads fixture with no randomness, and the target sits inside the
// pre-rotation segment, which the duplicate banding only overwrites at the two ends - so the target
// is present by construction and the Setup test can assert that positive.
//
// Weak by construction, and reported as such with this batch: both arms answer with a single bool
// and the fixture pins it to true, so agreement witnesses only that neither arm ever reports
// "absent" for a value the workload guarantees is there. Strengthening it would mean changing an
// arm's return type, which is not this harness's call.
public sealed partial class SearchInRotatedSortedArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().HasTargetByLinearScan(), BuildHarness().HasTargetByLinearScan());
        Assert.True(BuildHarness().HasTargetByLinearScan());
    }

    [Fact]
    public void HasTargetByLinearScan_DuplicateBoundaryBand_AgreesWithHasTargetByTrimDuplicatesThenBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByTrimDuplicatesThenBinarySearch(), harness.HasTargetByLinearScan());
    }

    [Fact]
    public void HasTargetByTrimDuplicatesThenBinarySearch_DuplicateBoundaryBand_AgreesWithHasTargetByLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasTargetByLinearScan(), harness.HasTargetByTrimDuplicatesThenBinarySearch());
    }

    private static SearchInRotatedSortedArrayIIBenchmarks BuildHarness()
    {
        var harness = new SearchInRotatedSortedArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
