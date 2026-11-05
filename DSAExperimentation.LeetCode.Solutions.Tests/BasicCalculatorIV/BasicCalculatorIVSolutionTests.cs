using DSAExperimentation.LeetCode.Solutions.BasicCalculatorIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BasicCalculatorIV;

// Harness only: both strategies live in BasicCalculatorIVSolution and are asserted
// against the same cases, among them the known-variable substitution cases and
// the product-of-sums expansion to a degree-two term.
public sealed partial class BasicCalculatorIVSolutionTests
{
    public static TheoryData<string, string[], int[], string[]> Cases =>
        new()
        {
            { "e - 8 + temperature - 1", [], [], ["1*e", "1*temperature", "-9"] },
            { "(e + 8) * (e - 8)", ["e"], [3], ["-55"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void EvaluateByDictionaryPolynomial_Cases_ReturnsSortedNonZeroTerms(
        string expression, string[] evalvars, int[] evalints, string[] expected)
    {
        var actual = BasicCalculatorIVSolution.EvaluateByDictionaryPolynomial(expression, evalvars, evalints);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void EvaluateByHashMapMergeSort_Cases_ReturnsSortedNonZeroTerms(
        string expression, string[] evalvars, int[] evalints, string[] expected)
    {
        var actual = BasicCalculatorIVSolution.EvaluateByHashMapMergeSort(expression, evalvars, evalints);

        Assert.Equal(expected, actual);
    }
}
