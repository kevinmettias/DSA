using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SatisfiabilityOfEqualityEquations;

// LeetCode 990. Satisfiability of Equality Equations: DisjointSet over the 26
// lowercase-letter variable ids (union on every "==" equation), then a single scan
// of the "!=" equations checking IsConnected - the same union-then-scan shape
// AccountsMergeTests already uses, applied to a fixed 26-slot alphabet instead of
// account indices.
public sealed partial class SatisfiabilityOfEqualityEquationsTests
{
    [Fact]
    public void IsSatisfiable_ContradictingEquations_ReturnsFalse()
    {
        string[] equations = ["a==b", "b!=a"];

        Assert.False(IsSatisfiable(equations));
    }

    [Fact]
    public void IsSatisfiable_TransitiveEquality_ReturnsTrue()
    {
        string[] equations = ["a==b", "b==c", "a==c"];

        Assert.True(IsSatisfiable(equations));
    }

    [Fact]
    public void IsSatisfiable_InequalityAcrossUnrelatedComponents_ReturnsTrue()
    {
        string[] equations = ["c==c", "b==d", "x!=z"];

        Assert.True(IsSatisfiable(equations));
    }

    [Fact]
    public void IsSatisfiable_InequalityAfterTransitiveEquality_ReturnsFalse()
    {
        string[] equations = ["a==b", "b!=c", "c==a"];

        Assert.False(IsSatisfiable(equations));
    }

    private static bool IsSatisfiable(string[] equations)
    {
        var components = new DisjointSet(26);

        foreach (var equation in equations)
        {
            if (equation[1] == '=')
            {
                components.Union(equation[0] - 'a', equation[3] - 'a');
            }
        }

        foreach (var equation in equations)
        {
            if (equation[1] == '!' && components.IsConnected(equation[0] - 'a', equation[3] - 'a'))
            {
                return false;
            }
        }

        return true;
    }
}
