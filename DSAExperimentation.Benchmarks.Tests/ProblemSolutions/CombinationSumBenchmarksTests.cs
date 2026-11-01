using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on a rebuilt workload at the smallest target, so this asserts what that check
// cannot - that every combination the candidate set makes sums to Target, a property of the problem
// rather than of either arm, and the count at the largest target, which the generic check never
// builds.
public sealed partial class CombinationSumBenchmarksTests
{
    private const int SmallestTarget = 30;

    // The harness's largest target, and how many combinations of 2, 3, 5 and 7 sum to it - counted
    // by the coin-change recurrence (ways[s] += ways[s - c], one candidate at a time), which is
    // inside the fewer-than-150 LC 39 promises its tests stay under.
    private const int LargestTarget = 40;
    private const int CombinationsSummingToLargestTarget = 90;

    [Fact]
    public void SpecializedRecursive_CandidatesTwoThreeFiveSeven_SumsEveryCombinationToTarget() =>
        Assert.All(BuildHarness().SpecializedRecursive(), combination => Assert.Equal(SmallestTarget, combination.Sum()));

    // BenchmarkArmsTests only builds the smallest target, so the largest one's count is pinned here.
    [Fact]
    public void Backtracking_LargestTarget_FindsFewerCombinationsThanLeetCodePromises()
    {
        var harness = new CombinationSumBenchmarks { Target = LargestTarget };
        harness.Setup();

        Assert.Equal(CombinationsSummingToLargestTarget, harness.Backtracking().Count);
    }

    private static CombinationSumBenchmarks BuildHarness()
    {
        var harness = new CombinationSumBenchmarks { Target = SmallestTarget };
        harness.Setup();

        return harness;
    }
}
