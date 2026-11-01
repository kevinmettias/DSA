using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GenerateParenthesesBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on the smallest workload, so this asserts only a count derived
// independently of either arm: the well-formed combinations of Pairs pairs are counted by the
// Pairs-th Catalan number, which is 42 at five.
public sealed partial class GenerateParenthesesBenchmarksTests
{
    private const int SmallestPairs = 5;

    // The fifth Catalan number: the number of well-formed combinations of five pairs.
    private const int ExpectedCombinationCount = 42;

    [Fact]
    public void RecursiveSpecialized_FivePairs_ReturnsTheFifthCatalanNumberOfCombinations() =>
        Assert.Equal(ExpectedCombinationCount, BuildHarness().RecursiveSpecialized().Count);

    private static GenerateParenthesesBenchmarks BuildHarness() => new() { Pairs = SmallestPairs };
}
