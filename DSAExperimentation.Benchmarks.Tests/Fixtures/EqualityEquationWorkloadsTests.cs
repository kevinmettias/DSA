using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for EqualityEquationWorkloads (ARCHITECTURE 17.7). The reading depends on the
// equations being well-formed LC 990 rows - two lowercase variables joined by "==" or "!=" - with
// enough inequalities in the mix that both strategies have to charge for them.
public sealed partial class EqualityEquationWorkloadsTests
{
    private const int EquationCount = 64;
    private const int Seed = 990; // LC problem number
    private const int EquationLength = 4; // "x==y" or "x!=y"
    private const int OperatorStart = 1;
    private const int OperatorLength = 2;
    private const char FirstVariable = 'a';
    private const char LastVariable = 'z';
    private const string EqualsOperator = "==";
    private const string NotEqualsOperator = "!=";
    private const int FewestInequalities = 1;
    private const int MinorityShareDivisor = 2; // the inequalities stay under half the mix

    [Fact]
    public void BuildEquations_EquationCount_ReturnsOneEquationPerPosition() =>
        Assert.Equal(EquationCount, EqualityEquationWorkloads.BuildEquations(EquationCount, Seed).Length);

    [Fact]
    public void BuildEquations_EveryEquation_JoinsTwoLowercaseVariablesWithAnOperator()
    {
        var equations = EqualityEquationWorkloads.BuildEquations(EquationCount, Seed);

        Assert.All(equations, equation => Assert.Equal(EquationLength, equation.Length));
        Assert.All(equations, equation => Assert.InRange(equation[0], FirstVariable, LastVariable));
        Assert.All(equations, equation => Assert.InRange(equation[^1], FirstVariable, LastVariable));
        Assert.All(equations, equation => Assert.True(IsInequality(equation) || Operator(equation) == EqualsOperator));
    }

    // The operator draw is probabilistic - one equation in five is an inequality - so what is
    // asserted is the bounded shape the reading depends on: a genuine mixture in which the
    // inequalities stay the minority, rather than a run of rows that all state the same thing.
    [Fact]
    public void BuildEquations_InequalityCount_StaysAMinorityOfTheMix()
    {
        var equations = EqualityEquationWorkloads.BuildEquations(EquationCount, Seed);
        var inequalities = equations.Count(IsInequality);

        Assert.InRange(inequalities, FewestInequalities, EquationCount / MinorityShareDivisor);
    }

    [Fact]
    public void BuildEquations_SameSeed_ReturnsTheSameEquations() =>
        Assert.Equal(
            EqualityEquationWorkloads.BuildEquations(EquationCount, Seed),
            EqualityEquationWorkloads.BuildEquations(EquationCount, Seed));

    private static string Operator(string equation) => equation.Substring(OperatorStart, OperatorLength);

    private static bool IsInequality(string equation) => Operator(equation) == NotEqualsOperator;
}
