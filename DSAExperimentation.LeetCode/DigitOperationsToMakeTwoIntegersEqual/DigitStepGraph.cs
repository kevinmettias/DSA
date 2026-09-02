namespace DSAExperimentation.LeetCode.DigitOperationsToMakeTwoIntegersEqual;

// Every non-prime integer with the given digit count, wired to every
// reachable single-digit +-1 mutation that also lands on a non-prime value -
// LC 3377's own "n must never be prime" law and "same digit count throughout"
// law, both baked into which nodes exist and which edges connect them rather
// than checked by the search. The domain model, not an answer to any one
// query about it (EdgeGraph's own framing for LC 3123's identical shape).
internal sealed class DigitStepGraph
{
    private DigitStepGraph(Dictionary<int, DigitStepNode> nodes) => Nodes = nodes;

    public Dictionary<int, DigitStepNode> Nodes { get; }

    public static DigitStepGraph Build(int digitCount)
    {
        var (low, high) = Range(digitCount);
        var nodes = BuildNodes(low, high);

        WireDigitMutations(nodes);

        return new DigitStepGraph(nodes);
    }

    private static (int Low, int High) Range(int digitCount)
    {
        var low = (int)Math.Pow(10, digitCount - 1);

        return (low, (low * 10) - 1);
    }

    private static Dictionary<int, DigitStepNode> BuildNodes(int low, int high)
    {
        var nodes = new Dictionary<int, DigitStepNode>();

        for (var value = low; value <= high; value++)
        {
            if (!IsPrime(value))
            {
                nodes[value] = new DigitStepNode(value);
            }
        }

        return nodes;
    }

    private static void WireDigitMutations(Dictionary<int, DigitStepNode> nodes)
    {
        foreach (var (value, node) in nodes)
        {
            foreach (var neighborValue in DigitMutations(value))
            {
                if (nodes.TryGetValue(neighborValue, out var neighbor))
                {
                    node.Edges.Add((neighborValue, neighbor));
                }
            }
        }
    }

    // Every digit +-1 mutation that keeps the same digit count: the general
    // "not 9" / "not 0" rule LC 3377 states, plus the one extra restriction
    // that a leading digit may not fall from 1 to 0 (that would drop a digit
    // entirely, changing n's digit count). Primality is the caller's job -
    // BuildNodes filters it when wiring a graph, and
    // DigitOperationsToMakeTwoIntegersEqualSolution.MinOperationsByBruteForceDijkstra
    // filters it neighbor by neighbor.
    public static IEnumerable<int> DigitMutations(int value)
    {
        var digits = value.ToString();

        for (var i = 0; i < digits.Length; i++)
        {
            var digit = digits[i] - '0';

            if (digit < 9)
            {
                yield return WithDigit(digits, i, digit + 1);
            }

            if (digit > 0 && !(i == 0 && digit == 1))
            {
                yield return WithDigit(digits, i, digit - 1);
            }
        }
    }

    private static int WithDigit(string digits, int index, int digit)
    {
        var chars = digits.ToCharArray();
        chars[index] = (char)('0' + digit);

        return int.Parse(new string(chars));
    }

    // LC 3377's own primality law: values as small as this repo ever builds
    // (n, m < 10^4) make trial division up to sqrt(value) the natural check -
    // no sieve is worth the extra state for a graph this size.
    public static bool IsPrime(int value)
    {
        if (value < 2)
        {
            return false;
        }

        for (var divisor = 2; divisor * divisor <= value; divisor++)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }
}
