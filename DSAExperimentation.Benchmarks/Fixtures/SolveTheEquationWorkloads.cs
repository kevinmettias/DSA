using System.Text;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 640 - an equation of termsPerSide terms a side whose
// solution, whenever it is a single one, is the integer PlantedSolution, as LC 640 promises
// every single solution is. Terms are drawn in pairs, one term on each side under one shared
// sign, that are equal at x = PlantedSolution: an x term faces the constant it equals there,
// either way round, or two equal constants face each other. The x terms still land on both
// sides, so their coefficients net out to anything, zero included - and a zero net leaves
// every x a solution, which LC 640 allows. Every coefficient is in [1, 99], so a term with its
// sign is at most four characters and 125 terms a side stay inside LC 640's 1,000-character
// equation.
internal static class SolveTheEquationWorkloads
{
    // At x = 1 an x term equals its own coefficient, which keeps the facing constant in range.
    public const int PlantedSolution = 1;

    // Exclusive upper bound on a term's coefficient.
    private const int CoefficientUpperBound = 100;

    private const int PairSignChoiceCount = 2;
    private const int PairKindCount = 3;
    private const int VariableOnLeftKind = 0;
    private const int VariableOnRightKind = 1;
    private const char Plus = '+';
    private const char Minus = '-';
    private const char Variable = 'x';
    private const char EqualsSign = '=';

    public static string Build(int termsPerSide, int seed)
    {
        var random = new Random(seed);
        var left = new StringBuilder();
        var right = new StringBuilder();

        for (var term = 0; term < termsPerSide; term++)
        {
            AppendSign(left, right, term, random);
            AppendBalancedPair(left, right, random);
        }

        return left.Append(EqualsSign).Append(right).ToString();
    }

    // The first term of a side carries no sign; every later pair shares a '+' or '-' drawn for it.
    private static void AppendSign(StringBuilder left, StringBuilder right, int term, Random random)
    {
        if (term == 0)
        {
            return;
        }

        var isMinus = random.Next(PairSignChoiceCount) == 0;
        var sign = isMinus ? Minus : Plus;
        left.Append(sign);
        right.Append(sign);
    }

    // A pair's kind puts its x term on the left, on the right, or nowhere - two equal constants.
    private static void AppendBalancedPair(StringBuilder left, StringBuilder right, Random random)
    {
        var kind = random.Next(PairKindCount);
        var coefficient = random.Next(1, CoefficientUpperBound);
        left.Append(coefficient);
        right.Append(coefficient);

        if (kind == VariableOnLeftKind)
        {
            left.Append(Variable);
        }

        if (kind == VariableOnRightKind)
        {
            right.Append(Variable);
        }
    }
}
