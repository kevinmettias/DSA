using DSAExperimentation.LeetCode.SatisfiabilityOfEqualityEquations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SatisfiabilityOfEqualityEquations;

// LeetCode 990. Satisfiability of Equality Equations. See
// SatisfiabilityOfEqualityEquationsSolution for the two strategies: a naive
// adjacency-list BFS reachability query per inequality, and this repo's own
// DisjointSet over the 26-letter alphabet. Harness only - the examples are stated
// once and each strategy gets its own theory so a failure names the arm that broke.
public sealed class SatisfiabilityOfEqualityEquationsTests
{
    public static TheoryData<string[], bool> Examples =>
        new()
        {
            { ["a==b", "b!=a"], false },
            { ["b==a", "a==b"], true },
            { ["a==b", "b==c", "a==c"], true },
            { ["c==c", "b==d", "x!=z"], true },
            { ["a==b", "b!=c", "c==a"], false },
            { ["a!=a"], false },
            { ["a!=b"], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EquationsPossibleByAdjacencyBfs_LeetCodeExamples_ReturnsWhetherEquationsAreSatisfiable(
        string[] equations, bool expected) =>
        Assert.Equal(expected, SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByAdjacencyBfs(equations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EquationsPossibleByDisjointSet_LeetCodeExamples_ReturnsWhetherEquationsAreSatisfiable(
        string[] equations, bool expected) =>
        Assert.Equal(expected, SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByDisjointSet(equations));
}
