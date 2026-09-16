using DSAExperimentation.LeetCode.SatisfiabilityOfEqualityEquations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SatisfiabilityOfEqualityEquations;

// LeetCode 990. Satisfiability of Equality Equations. See
// SatisfiabilityOfEqualityEquationsSolution for the two strategies: a naive
// adjacency-list BFS reachability query per inequality, and this repo's own
// DisjointSet over the 26-letter alphabet. Harness only - the examples are stated
// once and each strategy gets its own theory so a failure names the arm that broke.
public sealed class SatisfiabilityOfEqualityEquationsTests
{
    public static TheoryData<EquationsExample> Examples =>
        new()
        {
            new EquationsExample(Equations: ["a==b", "b!=a"], IsSatisfiable: false),
            new EquationsExample(Equations: ["b==a", "a==b"], IsSatisfiable: true),
            new EquationsExample(Equations: ["a==b", "b==c", "a==c"], IsSatisfiable: true),
            new EquationsExample(Equations: ["c==c", "b==d", "x!=z"], IsSatisfiable: true),
            new EquationsExample(Equations: ["a==b", "b!=c", "c==a"], IsSatisfiable: false),
            new EquationsExample(Equations: ["a!=a"], IsSatisfiable: false),
            new EquationsExample(Equations: ["a!=b"], IsSatisfiable: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EquationsPossibleByAdjacencyBfs_LeetCodeExamples_ReturnsWhetherEquationsAreSatisfiable(
        EquationsExample example) =>
        Assert.Equal(
            example.IsSatisfiable,
            SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByAdjacencyBfs(example.Equations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EquationsPossibleByDisjointSet_LeetCodeExamples_ReturnsWhetherEquationsAreSatisfiable(
        EquationsExample example) =>
        Assert.Equal(
            example.IsSatisfiable,
            SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByDisjointSet(example.Equations));

    // One example: the equation system and whether it can hold. The expectation is
    // named rather than carried by its position, so the row reads as an assertion
    // instead of as a bare `true`.
    public readonly record struct EquationsExample(string[] Equations, bool IsSatisfiable);
}
