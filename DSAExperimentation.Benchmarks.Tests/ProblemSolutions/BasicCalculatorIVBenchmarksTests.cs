using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BasicCalculatorIVBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this pins the size of the baseline's answer to what the workload fixes. The arms return LC 770's
// sorted term list, and the expression is a sum of Length distinct variables with no evalvars, so every term
// survives and the answer must hold exactly one term per variable - a decisive count known without either arm.
public sealed partial class BasicCalculatorIVBenchmarksTests
{
    // The smaller of Setup's [Params(5, 55)] variable counts.
    private const int SmallestLength = 5;

    [Fact]
    public void DictionaryPolynomial_FiveVariableSum_ReturnsOneTermPerVariable() =>
        Assert.Equal(SmallestLength, BuildHarness().DictionaryPolynomial().Count);

    private static BasicCalculatorIVBenchmarks BuildHarness()
    {
        var harness = new BasicCalculatorIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
