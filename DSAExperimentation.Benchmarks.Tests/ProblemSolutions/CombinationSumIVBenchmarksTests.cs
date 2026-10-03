using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumIVBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// that the largest Target stays inside LC 377's promise of an answer that fits a 32-bit int. Past it both arms
// overflow to the same wrong number and still agree, so the count is pinned to a value derived outside them.
public sealed partial class CombinationSumIVBenchmarksTests
{
    private const int LargestTarget = 34;

    // How many ordered sequences of 1, 2, 3, 5 and 10 sum to 34, counted over 64-bit integers by
    // count[s] = sum of count[s - n] for every n <= s: the largest such count below int.MaxValue, since
    // at 35 it is already 2,719,190,965.
    private const int OrderedSumsToLargestTarget = 1_438_436_011;

    [Fact]
    public void Tabulation_LargestTarget_CountsEveryOrderedSumWithoutOverflow() =>
        Assert.Equal(OrderedSumsToLargestTarget, BuildHarness().Tabulation());

    [Fact]
    public void Memoized_LargestTarget_CountsEveryOrderedSumWithoutOverflow() =>
        Assert.Equal(OrderedSumsToLargestTarget, BuildHarness().Memoized());

    private static CombinationSumIVBenchmarks BuildHarness() => new() { Target = LargestTarget };
}
