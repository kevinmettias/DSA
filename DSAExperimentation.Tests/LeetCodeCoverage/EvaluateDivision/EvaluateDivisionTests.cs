using DSAExperimentation.LeetCode.EvaluateDivision;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EvaluateDivision;

// Harness only. Both strategies are EvaluateDivisionSolution's - this file pins them
// to LeetCode's published examples, including unknown-variable and
// unknown-variable-to-itself queries.
public sealed class EvaluateDivisionTests
{
    public static TheoryData<DivisionQueryCase> Examples =>
        new()
        {
            {
                new DivisionQueryCase(
                    [("a", "b", 2.0), ("b", "c", 3.0)], Dividend: "a", Divisor: "c", Expected: 6.0)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 2.0), ("b", "c", 3.0)], Dividend: "b", Divisor: "a", Expected: 0.5)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 2.0), ("b", "c", 3.0)], Dividend: "a", Divisor: "e", Expected: -1.0)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 2.0), ("b", "c", 3.0)], Dividend: "a", Divisor: "a", Expected: 1.0)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 2.0), ("b", "c", 3.0)], Dividend: "x", Divisor: "x", Expected: -1.0)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)],
                    Dividend: "a",
                    Divisor: "c",
                    Expected: 3.75)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)],
                    Dividend: "c",
                    Divisor: "b",
                    Expected: 0.4)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)],
                    Dividend: "bc",
                    Divisor: "cd",
                    Expected: 5.0)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)],
                    Dividend: "cd",
                    Divisor: "bc",
                    Expected: 0.2)
            },
            {
                new DivisionQueryCase(
                    [("a", "b", 1.5), ("b", "c", 2.5), ("bc", "cd", 5.0)],
                    Dividend: "a",
                    Divisor: "cd",
                    Expected: -1.0)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByDictionaryDfs_LeetCodeExamples_ComputesProductOrReportsUnreachable(
        DivisionQueryCase example)
    {
        var actual = EvaluateDivisionSolution.EvaluateByDictionaryDfs(
            example.Equations,
            new EvaluateDivisionSolution.Dividend(example.Dividend),
            new EvaluateDivisionSolution.Divisor(example.Divisor));

        Assert.Equal(example.Expected, actual, 5);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByHashMapStackDfs_LeetCodeExamples_ComputesProductOrReportsUnreachable(
        DivisionQueryCase example)
    {
        var actual = EvaluateDivisionSolution.EvaluateByHashMapStackDfs(
            example.Equations,
            new EvaluateDivisionSolution.Dividend(example.Dividend),
            new EvaluateDivisionSolution.Divisor(example.Divisor));

        Assert.Equal(example.Expected, actual, 5);
    }

    // One LeetCode example: the known a/b equations, the query's own dividend and divisor,
    // and the product the query evaluates to (-1 when it does not resolve). The two query
    // variables are named fields rather than two adjacent `string` parameters, so a row is
    // written `new DivisionQueryCase(Dividend: ..., Divisor: ...)` and a dividend/divisor
    // swap has to be typed out by name instead of falling out of a position the compiler
    // would have accepted either way. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct DivisionQueryCase(
        (string Dividend, string Divisor, double Value)[] Equations,
        string Dividend,
        string Divisor,
        double Expected);
}
