namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 990 - equations over the 26 lowercase variables
// with both sides drawn uniformly, one equation in five being an inequality. That
// mix keeps the equality components large enough to make reachability queries real
// work while leaving enough "!=" equations to charge for them.
internal static class EqualityEquationWorkloads
{
    private const int AlphabetSize = 26;
    private const int InequalityProbabilityDenominator = 5;
    private const string EqualsOperator = "==";
    private const string NotEqualsOperator = "!=";

    public static string[] BuildEquations(int equationCount, int seed)
    {
        var random = new Random(seed);
        var equations = new string[equationCount];

        for (var i = 0; i < equationCount; i++)
        {
            var first = (char)('a' + random.Next(AlphabetSize));
            var second = (char)('a' + random.Next(AlphabetSize));
            var op = random.Next(InequalityProbabilityDenominator) == 0 ? NotEqualsOperator : EqualsOperator;
            equations[i] = $"{first}{op}{second}";
        }

        return equations;
    }
}
