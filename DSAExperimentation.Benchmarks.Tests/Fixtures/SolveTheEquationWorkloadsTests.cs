using System.Text.RegularExpressions;
using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SolveTheEquationWorkloads (ARCHITECTURE 17.7). The reading depends on the
// equation being one LC 640 could pose: at most 1,000 characters, one '=', integers in [0, 100]
// without leading zeros, and - the guarantee the generator exists to plant - a single solution
// that is an integer. The solution is worked out here by a parse of its own, not by either arm:
// each side folds to a coefficient of x and a constant, and the two folds decide it.
public sealed partial class SolveTheEquationWorkloadsTests
{
    // Mirrors SolveTheEquationBenchmarks' own private RandomSeed, and its two sizes.
    private const int Seed = 11;
    private const int SmallestTermsPerSide = 12;
    private const int LargestTermsPerSide = 125;

    private const int MaxEquationLength = 1_000;
    private const int MaxOperand = 100;
    private const char EqualsSign = '=';
    private const string MinusSign = "-";

    // An equation is a left side and a right side.
    private const int SideCount = 2;

    private const string SideGrammar = "^[0-9]+x?([+-][0-9]+x?)*$";
    private const string SignGroup = "sign";
    private const string DigitsGroup = "digits";
    private const string VariableGroup = "variable";
    private const string TermPattern = "(?<sign>[+-]?)(?<digits>[0-9]+)(?<variable>x?)";

    public static TheoryData<int> TermCounts => new([SmallestTermsPerSide, LargestTermsPerSide]);

    [Theory]
    [MemberData(nameof(TermCounts))]
    public void Build_BenchmarkSizes_StayInsideTheEquationLengthCap(int termsPerSide) =>
        Assert.InRange(SolveTheEquationWorkloads.Build(termsPerSide, Seed).Length, 1, MaxEquationLength);

    [Theory]
    [MemberData(nameof(TermCounts))]
    public void Build_EverySide_IsSignedTermsOfInRangeIntegersWithoutLeadingZeros(int termsPerSide)
    {
        var sides = SolveTheEquationWorkloads.Build(termsPerSide, Seed).Split(EqualsSign);

        Assert.Equal(SideCount, sides.Length);
        Assert.All(sides, side => Assert.Matches(SideGrammar, side));
        Assert.All(sides.SelectMany(Terms), term => Assert.InRange(term.Operand, 0, MaxOperand));
        Assert.All(sides.SelectMany(Terms), term => Assert.Equal(term.Digits, term.Operand.ToString()));
    }

    // a * x = b, folded from both sides: a single solution is b / a, and it must divide evenly and
    // be the planted one; with a = 0 the planted value still satisfies the equation, so b = 0 too.
    [Theory]
    [MemberData(nameof(TermCounts))]
    public void Build_SingleSolution_IsThePlantedInteger(int termsPerSide)
    {
        var sides = SolveTheEquationWorkloads.Build(termsPerSide, Seed).Split(EqualsSign);
        var (leftCoefficient, leftConstant) = Fold(sides[0]);
        var (rightCoefficient, rightConstant) = Fold(sides[1]);
        var coefficient = leftCoefficient - rightCoefficient;
        var constant = rightConstant - leftConstant;

        Assert.Equal(coefficient * SolveTheEquationWorkloads.PlantedSolution, constant);
    }

    private static (int Coefficient, int Constant) Fold(string side)
    {
        var coefficient = 0;
        var constant = 0;

        foreach (var term in Terms(side))
        {
            var signed = term.IsNegative ? -term.Operand : term.Operand;
            coefficient += term.IsVariable ? signed : 0;
            constant += term.IsVariable ? 0 : signed;
        }

        return (coefficient, constant);
    }

    private static IEnumerable<Term> Terms(string side) =>
        Regex.Matches(side, TermPattern).Select(match => new Term(
            match.Groups[SignGroup].Value == MinusSign,
            match.Groups[DigitsGroup].Value,
            match.Groups[VariableGroup].Value.Length > 0));

    // One parsed term: its sign, its digits as written, and whether it carries x.
    private readonly record struct Term(bool IsNegative, string Digits, bool IsVariable)
    {
        public int Operand => int.Parse(Digits);
    }
}
