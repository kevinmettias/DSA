using DSAExperimentation.LeetCode.Solutions.SatisfiabilityOfEqualityEquations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SatisfiabilityOfEqualityEquations;

// LeetCode 990. Satisfiability of Equality Equations. See
// SatisfiabilityOfEqualityEquationsSolution for the two strategies: a naive
// adjacency-list BFS reachability query per inequality, and this repo's own
// DisjointSet over the 26-letter alphabet. Harness only - the cases are stated
// once and each strategy gets its own theory so a failure names the arm that broke.
// The one case is a lone inequality, which nothing contradicts. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class SatisfiabilityOfEqualityEquationsSolutionTests
{
    public static TheoryData<EquationsCase> Cases =>
        new()
        {
            new EquationsCase(Equations: ["a!=b"], IsSatisfiable: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsSatisfiableByAdjacencyBfs_Cases_ReturnsWhetherEquationsAreSatisfiable(
        EquationsCase testCase) =>
        Assert.Equal(
            testCase.IsSatisfiable,
            SatisfiabilityOfEqualityEquationsSolution.IsSatisfiableByAdjacencyBfs(testCase.Equations));

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsSatisfiableByDisjointSet_Cases_ReturnsWhetherEquationsAreSatisfiable(
        EquationsCase testCase) =>
        Assert.Equal(
            testCase.IsSatisfiable,
            SatisfiabilityOfEqualityEquationsSolution.IsSatisfiableByDisjointSet(testCase.Equations));

    // One case: the equation system and whether it can hold. The expectation is
    // named rather than carried by its position, so the row reads as an assertion
    // instead of as a bare `true`.
    public readonly record struct EquationsCase(string[] Equations, bool IsSatisfiable);
}
