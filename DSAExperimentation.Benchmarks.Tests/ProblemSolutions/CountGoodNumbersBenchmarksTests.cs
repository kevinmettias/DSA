using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodNumbersBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing at the smallest [Params] length, so this pins the baseline to the digit
// product 5^ceil(n/2) * 4^floor(n/2) mod 1e9+7 there, computed independently of both arms. The class
// carries no [GlobalSetup] - the length is the whole workload.
public sealed partial class CountGoodNumbersBenchmarksTests
{
    private const long SmallestLength = 1_000;
    private const int GoodNumbersOfLengthOneThousand = 36020987;

    [Fact]
    public void RepeatedMultiplication_LengthOneThousand_MatchesTheDigitProduct() =>
        Assert.Equal(GoodNumbersOfLengthOneThousand, BuildHarness().RepeatedMultiplication());

    private static CountGoodNumbersBenchmarks BuildHarness() =>
        new() { Length = SmallestLength };
}
