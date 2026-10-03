using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindInMountainArrayBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins where the target is, from the mountain's construction alone.
// The ascending slope is 0, 2, 4, ... and the descending slope starts one below the even peak and
// falls by 2, so it holds only odd values: the target, second from the end, has no copy on the
// ascending half, and LC 1095's minimum index for it is Length - 2 - which is what makes the linear
// scan walk nearly the whole array.
public sealed partial class FindInMountainArrayBenchmarksTests
{
    private const int SmallestLength = 1_000;
    private const int TargetIndex = SmallestLength - 2;

    [Fact]
    public void LinearScan_TargetOnlyOnTheDescendingSlope_FindsItSecondFromTheEnd() =>
        Assert.Equal(TargetIndex, BuildHarness().LinearScan());

    [Fact]
    public void PeakBisectionThenBinarySearch_TargetOnlyOnTheDescendingSlope_FindsItSecondFromTheEnd() =>
        Assert.Equal(TargetIndex, BuildHarness().PeakBisectionThenBinarySearch());

    private static FindInMountainArrayBenchmarks BuildHarness()
    {
        var harness = new FindInMountainArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
