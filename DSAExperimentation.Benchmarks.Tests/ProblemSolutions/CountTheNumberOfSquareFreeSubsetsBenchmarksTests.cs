using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfSquareFreeSubsetsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// a range that comes from the workload's shape rather than from either arm.
//
// The generated array is private and the subset count is the only thing either arm reports, so the
// documented shape is asserted through that: an array of Length values has 2^Length subsets, and the
// empty subset the problem excludes is the one that is never counted.
public sealed partial class CountTheNumberOfSquareFreeSubsetsBenchmarksTests
{
    private const int SmallestLength = 15;

    private const long NonEmptySubsetCount = (1L << SmallestLength) - 1;

    // The problem counts non-empty subsets, and the seeded array holds values in [1, 30) that
    // are square-free, so at least one subset qualifies.
    private const long FewestSquareFreeSubsets = 1;

    [Fact]
    public void BruteForce_SmallestLength_StaysWithinTheNonEmptySubsetCount() =>
        Assert.InRange(BuildHarness().BruteForce(), FewestSquareFreeSubsets, NonEmptySubsetCount);

    private static CountTheNumberOfSquareFreeSubsetsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfSquareFreeSubsetsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
