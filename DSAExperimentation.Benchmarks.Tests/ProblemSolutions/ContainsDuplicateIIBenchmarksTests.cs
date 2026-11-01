using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainsDuplicateIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this pins the baseline to the verdict the
// workload's shape decides, derived independently of either arm.
//
// The array is private, but its documented shape is decisive: it is a permutation of distinct
// integers, so no value repeats anywhere and certainly not within the window, and the answer is
// false - the full worst-case scan the class comment says the workload exists to force, with no
// early exit to make the brute force look competitive.
public sealed partial class ContainsDuplicateIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void HasNearbyDuplicateByBruteForce_DistinctPermutation_ReturnsFalse() =>
        Assert.False(BuildHarness().HasNearbyDuplicateByBruteForce());

    private static ContainsDuplicateIIBenchmarks BuildHarness()
    {
        var harness = new ContainsDuplicateIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
