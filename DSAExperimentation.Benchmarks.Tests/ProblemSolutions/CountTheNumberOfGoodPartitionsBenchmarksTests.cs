using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfGoodPartitionsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// a range that comes from the workload's shape rather than from either arm.
//
// The generated array is private and the partition count is the only thing either arm reports, so the
// documented shape is asserted through that: an ArrayLength-long array admits only ArrayLength - 1
// cut positions, so no partition count can reach 2^(ArrayLength - 1).
public sealed partial class CountTheNumberOfGoodPartitionsBenchmarksTests
{
    private const int SmallestArrayLength = 16;

    private const long PartitionCount = 1L << (SmallestArrayLength - 1);

    // The whole array is always one valid partition - it needs no cut at all - so no run can
    // report zero.
    private const long FewestPartitions = 1;

    [Fact]
    public void BruteForce_SmallestArrayLength_StaysWithinThePartitionCount() =>
        Assert.InRange(BuildHarness().BruteForce(SmallestArrayLength), FewestPartitions, PartitionCount);

    private static CountTheNumberOfGoodPartitionsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfGoodPartitionsBenchmarks();
        harness.Setup();

        return harness;
    }
}
