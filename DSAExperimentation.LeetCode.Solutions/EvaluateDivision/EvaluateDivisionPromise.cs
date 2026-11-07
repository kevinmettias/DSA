namespace DSAExperimentation.LeetCode.Solutions.EvaluateDivision;

// LC 399's promise about its input: "the input is always valid ... there is no contradiction", so every path
// between two variables multiplies out to one ratio. LeetCode's judge never poses an input that breaks it.
// Generated cases do: a cycle a/b = 2, b/c = 3, c/d = 0.5, d/a = 0.333 makes a/d 3 one way round and 1/0.333
// the other, and each strategy answers with whichever path its walk meets first. A harness that runs them asks
// this first: it gives each variable a worth, 1 for the first of its component reached, and every equation
// must then hold between the worths it joins, to within rounding.
internal static class EvaluateDivisionPromise
{
    private const double RelativeTolerance = 1e-9;

    public static bool IsKeptBy(IList<IList<string>> equations, double[] values, IList<IList<string>> queries)
    {
        var ratiosByVariable = RatiosByVariable(equations, values);
        var worths = new Dictionary<string, double>();

        return ratiosByVariable.Keys.All(variable => IsConsistentFrom(variable, ratiosByVariable, worths));
    }

    // Each equation a / b = v read both ways: from a, b is v times smaller; from b, a is v times larger.
    private static Dictionary<string, List<(string Variable, double Ratio)>> RatiosByVariable(
        IList<IList<string>> equations, double[] values)
    {
        var ratiosByVariable = new Dictionary<string, List<(string Variable, double Ratio)>>();

        for (var index = 0; index < equations.Count; index++)
        {
            var (dividend, divisor) = (equations[index][0], equations[index][1]);
            RatiosOf(ratiosByVariable, dividend).Add((divisor, values[index]));
            RatiosOf(ratiosByVariable, divisor).Add((dividend, 1 / values[index]));
        }

        return ratiosByVariable;
    }

    private static List<(string Variable, double Ratio)> RatiosOf(
        Dictionary<string, List<(string Variable, double Ratio)>> ratiosByVariable, string variable)
    {
        if (!ratiosByVariable.TryGetValue(variable, out var ratios))
        {
            ratios = [];
            ratiosByVariable[variable] = ratios;
        }

        return ratios;
    }

    // Walks the component holding start, unless an earlier walk already gave it worths, and answers whether
    // every equation in it agrees with the worths the walk gives.
    private static bool IsConsistentFrom(
        string start, Dictionary<string, List<(string Variable, double Ratio)>> ratiosByVariable, Dictionary<string, double> worths)
    {
        if (!worths.TryAdd(start, 1))
        {
            return true;
        }

        var pending = new Queue<string>([start]);

        while (pending.TryDequeue(out var variable))
        {
            if (!IsEachNeighborConsistent(variable, ratiosByVariable[variable], worths, pending))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsEachNeighborConsistent(
        string variable, List<(string Variable, double Ratio)> ratios, Dictionary<string, double> worths, Queue<string> pending)
    {
        foreach (var (neighbor, ratio) in ratios)
        {
            var implied = worths[variable] / ratio;

            if (worths.TryAdd(neighbor, implied))
            {
                pending.Enqueue(neighbor);
            }
            else if (!IsClose(worths[neighbor], implied))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsClose(double known, double implied) =>
        Math.Abs(known - implied) <= RelativeTolerance * Math.Max(Math.Abs(known), Math.Abs(implied));
}
