using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DifferentWaysToAddParenthesesBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests
// cannot pin: the results themselves, known from Setup's construction rather than from either arm. Setup builds
// "1+1+...+1" out of OperandCount copies of one operand, so there is one result per way to fully parenthesize
// OperandCount operands - the (OperandCount - 1)th Catalan number, which is 42 for the six the benchmark's smallest
// parameter lists - and every one of them evaluates to OperandCount.
public sealed partial class DifferentWaysToAddParenthesesBenchmarksTests
{
    private const int SmallestOperandCount = 6;
    private const int ExpectedParenthesizationsOfSixOnes = 42;

    [Fact]
    public void PlainRecursion_SixRepeatedOnes_EvaluatesEveryCatalanParenthesization() =>
        Assert.Equal(EveryParenthesizationOfSixOnes(), BuildHarness().PlainRecursion());

    [Fact]
    public void MemoizedSubstring_SixRepeatedOnes_EvaluatesEveryCatalanParenthesization() =>
        Assert.Equal(EveryParenthesizationOfSixOnes(), BuildHarness().MemoizedSubstring());

    // Adding up six ones gives six however the sum is parenthesized.
    private static IEnumerable<int> EveryParenthesizationOfSixOnes() =>
        Enumerable.Repeat(SmallestOperandCount, ExpectedParenthesizationsOfSixOnes);

    private static DifferentWaysToAddParenthesesBenchmarks BuildHarness()
    {
        var harness = new DifferentWaysToAddParenthesesBenchmarks { OperandCount = SmallestOperandCount };
        harness.Setup();

        return harness;
    }
}
