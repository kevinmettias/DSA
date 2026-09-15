using DSAExperimentation.LeetCode.EvaluateDivision;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EvaluateDivision;

// Harness only. Both strategies are EvaluateDivisionSolution's - this file pins them
// to LeetCode's published examples, including unknown-variable and
// unknown-variable-to-itself queries.
public sealed class EvaluateDivisionTests
{
    public static TheoryData<(string Dividend, string Divisor, double Value)[], string, string, double> Examples =>
        new()
        {
            { [("a", "b", 2.0), ("b", "c", 3.0)], "a", "c", 6.0 },
            { [("a", "b", 2.0), ("b", "c", 3.0)], "b", "a", 0.5 },
            { [("a", "b", 2.0), ("b", "c", 3.0)], "a", "e", -1.0 },
            { [("a", "b", 2.0), ("b", "c", 3.0)], "a", "a", 1.0 },
            { [("a", "b", 2.0), ("b", "c", 3.0)], "x", "x", -1.0 },
            { [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)], "a", "c", 3.75 },
            { [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)], "c", "b", 0.4 },
            { [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)], "bc", "cd", 5.0 },
            { [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)], "cd", "bc", 0.2 },
            { [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)], "a", "cd", -1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByDictionaryDfs_LeetCodeExamples_ComputesProductOrReportsUnreachable(
        (string Dividend, string Divisor, double Value)[] equations, string dividend, string divisor, double expected) =>
        Assert.Equal(
            expected,
            EvaluateDivisionSolution.EvaluateByDictionaryDfs(
                equations,
                new EvaluateDivisionSolution.Dividend(dividend),
                new EvaluateDivisionSolution.Divisor(divisor)),
            5);

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByHashMapStackDfs_LeetCodeExamples_ComputesProductOrReportsUnreachable(
        (string Dividend, string Divisor, double Value)[] equations, string dividend, string divisor, double expected) =>
        Assert.Equal(
            expected,
            EvaluateDivisionSolution.EvaluateByHashMapStackDfs(
                equations,
                new EvaluateDivisionSolution.Dividend(dividend),
                new EvaluateDivisionSolution.Divisor(divisor)),
            5);
}
