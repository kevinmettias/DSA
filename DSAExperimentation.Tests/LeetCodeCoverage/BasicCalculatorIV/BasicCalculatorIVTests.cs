using DSAExperimentation.LeetCode.BasicCalculatorIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BasicCalculatorIV;

// Harness only: both strategies live in BasicCalculatorIVSolution and are asserted
// against the same examples, including the known-variable substitution cases and
// the product-of-sums expansion to a degree-two term.
public sealed class BasicCalculatorIVTests
{
    public static TheoryData<string, string[], int[], string[]> Examples =>
        new()
        {
            { "e + 8 - a + 5", ["e"], [1], ["-1*a", "14"] },
            { "e - 8 + temperature - 1", [], [], ["1*e", "1*temperature", "-9"] },
            { "(e + 8) * (e - 8)", [], [], ["1*e*e", "-64"] },
            { "(e + 8) * (e - 8)", ["e"], [3], ["-55"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByDictionaryPolynomial_LeetCodeExamples_ReturnsSortedNonZeroTerms(
        string expression, string[] evalvars, int[] evalints, string[] expected)
    {
        var actual = BasicCalculatorIVSolution.EvaluateByDictionaryPolynomial(expression, evalvars, evalints);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByHashMapMergeSort_LeetCodeExamples_ReturnsSortedNonZeroTerms(
        string expression, string[] evalvars, int[] evalints, string[] expected)
    {
        var actual = BasicCalculatorIVSolution.EvaluateByHashMapMergeSort(expression, evalvars, evalints);

        Assert.Equal(expected, actual);
    }
}
