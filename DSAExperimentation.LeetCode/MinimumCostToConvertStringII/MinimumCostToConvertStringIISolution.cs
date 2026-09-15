using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

// LeetCode 2977. Minimum Cost to Convert String II: same shortest-path-per-
// position idea as LC 2976, generalized from single characters to whole
// substrings. original[i].length == changed[i].length is a problem
// constraint, so a chosen window's length never changes mid-conversion -
// only its content does - which is what lets a plain per-length DP walk
// stand in for the "disjoint or identical substrings" operation rule:
// covering position i with a length-L window and paying that window's
// shortest registered-string-to-registered-string cost is exactly one
// operation (or chain of operations) over source[i..i+L).
internal static class MinimumCostToConvertStringIISolution
{
    private const long Unreachable = long.MaxValue / 4;

    // Textbook baseline: a plain BCL Dictionary<string,int> index and a
    // hand-rolled Floyd-Warshall over a long[,] matrix - deliberately
    // without this repo's own AllPairsShortestPaths, the arm the composed
    // strategy below has to justify itself against. Same shape as
    // MinimumCostToConvertStringISolution's brute-force arm, generalized
    // from 26 fixed letters to however many distinct strings the rules
    // mention.
    public static long MinimumCostByBruteForceFloydWarshall(
        SourceText source, TargetText target, (string[] Original, string[] Changed, int[] Cost) rules)
    {
        var (original, changed, cost) = rules;
        var index = BuildIndex(original, changed);
        var distances = BuildDistanceMatrix(index, original, changed, cost);

        return MinimumCostByBruteForceFloydWarshall(source, target, index, distances);
    }

    public static long MinimumCostByBruteForceFloydWarshall(
        SourceText source, TargetText target, Dictionary<string, int> index, long[,] distances)
    {
        var pricing = new MatrixWindowPricing(index, distances);

        return MinimumCostOverWalk(source, target, index.Keys, pricing);
    }

    // Public so the LeetCode-shaped overload above and a benchmark's
    // [GlobalSetup] can share one string -> node-index assignment.
    public static Dictionary<string, int> BuildIndex(string[] original, string[] changed)
    {
        var index = new Dictionary<string, int>();

        foreach (var value in original)
        {
            AddIfMissing(index, value);
        }

        foreach (var value in changed)
        {
            AddIfMissing(index, value);
        }

        return index;
    }

    private static void AddIfMissing(Dictionary<string, int> index, string value)
    {
        if (!index.ContainsKey(value))
        {
            index[value] = index.Count;
        }
    }

    // Public so a benchmark's [GlobalSetup] can build the same prepared
    // matrix this overload's own hoisting builds internally, rather than
    // reimplementing it - the same role BuildDistanceMatrix plays for
    // MinimumCostToConvertStringI's benchmark.
    public static long[,] BuildDistanceMatrix(Dictionary<string, int> index, string[] original, string[] changed, int[] cost)
    {
        var distances = BuildIdentityMatrix(index.Count);

        ApplyDirectConversions(distances, index, (original, changed, cost));
        CloseUnderTransitiveChains(distances);

        return distances;
    }

    // The starting matrix: every string reaches itself at no cost and every other
    // string not at all, until a stated rule or a chain of them proves otherwise.
    private static long[,] BuildIdentityMatrix(int size)
    {
        var distances = new long[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                distances[i, j] = i == j ? 0 : Unreachable;
            }
        }

        return distances;
    }

    // Every stated rule is one direct edge between the two strings it names here,
    // and a conversion stated more than once keeps its cheapest cost.
    private static void ApplyDirectConversions(
        long[,] distances, Dictionary<string, int> index, (string[] Original, string[] Changed, int[] Cost) rules)
    {
        var (original, changed, cost) = rules;

        for (var i = 0; i < original.Length; i++)
        {
            var (from, to) = (index[original[i]], index[changed[i]]);
            distances[from, to] = Math.Min(distances[from, to], cost[i]);
        }
    }

    // Floyd-Warshall: each distinct string takes a turn as an intermediate hop, so
    // after its pass no from -> to entry is worse than routing that one string.
    private static void CloseUnderTransitiveChains(long[,] distances)
    {
        var size = distances.GetLength(0);

        for (var through = 0; through < size; through++)
        {
            for (var from = 0; from < size; from++)
            {
                RelaxThrough(distances, from, through);
            }
        }
    }

    // One row of a pass: does going from -> through -> to beat the best from -> to so far?
    private static void RelaxThrough(long[,] distances, int from, int through)
    {
        var size = distances.GetLength(0);

        for (var to = 0; to < size; to++)
        {
            var viaIntermediate = distances[from, through] + distances[through, to];

            if (viaIntermediate < distances[from, to])
            {
                distances[from, to] = viaIntermediate;
            }
        }
    }

    // Algorithms.ShortestPaths.AllPairsShortestPaths' Floyd-Warshall over the
    // SubstringNetwork's distinct-string graph - the same composition
    // MinimumCostToConvertStringISolution uses for its 26-letter graph,
    // generalized to whatever distinct strings the rules mention.
    public static long MinimumCostByAllPairsShortestPaths(
        SourceText source, TargetText target, (string[] Original, string[] Changed, int[] Cost) rules)
    {
        var network = SubstringNetwork.Build(rules.Original, rules.Changed, rules.Cost);

        return MinimumCostByAllPairsShortestPaths(source, target, network);
    }

    public static long MinimumCostByAllPairsShortestPaths(SourceText source, TargetText target, SubstringNetwork network)
    {
        AllPairsShortestPaths.TryComputeDistances<StringNode, StringTopology, ListEdges<StringNode, int>, int>(
            network.NodesByValue.Values, out var distances);

        var pricing = new NetworkWindowPricing(network, distances);

        return MinimumCostOverWalk(source, target, network.NodesByValue.Keys, pricing);
    }

    // dp[i] is the minimum cost to convert source[0..i) into target[0..i). Both arms
    // above walk that table identically - neither differs on how the walk is shaped,
    // only on what prices one window - so the walk is one method and the arm in play
    // is its `pricing` argument. `keys` is whichever set of strings that arm indexed
    // its edges by; the rule set's distinct strings either way.
    private static long MinimumCostOverWalk(
        SourceText source, TargetText target, IEnumerable<string> keys, IWindowPricing pricing)
    {
        var lengths = DistinctLengths(keys);
        var walk = new ConversionWalk(source.Text, target.Text, lengths, pricing);
        var dp = NewDpArray(walk.Source.Length);

        for (var i = 0; i < walk.Source.Length; i++)
        {
            RelaxFromPosition(dp, i, walk);
        }

        return MinimumCostAtEnd(dp);
    }

    // One position of the walk: a position whose character is already right costs
    // nothing, and any same-length window the strategy can price is one operation
    // (or a chain of operations) over source[i..i+L).
    private static void RelaxFromPosition(long[] dp, int i, ConversionWalk walk)
    {
        if (dp[i] >= Unreachable)
        {
            return;
        }

        if (walk.Source[i] == walk.Target[i])
        {
            Relax(dp, i + 1, dp[i]);
        }

        foreach (var length in walk.Lengths)
        {
            RelaxWindow(dp, i, length, walk);
        }
    }

    // One window at one position. A window that runs off the end cannot be chosen,
    // and neither can one `pricing` has no price for - the rules simply never spell
    // that conversion.
    private static void RelaxWindow(long[] dp, int i, int length, ConversionWalk walk)
    {
        if (i + length > walk.Source.Length)
        {
            return;
        }

        var fromWindow = walk.Source.Substring(i, length);
        var toWindow = walk.Target.Substring(i, length);
        var windows = new WindowPair(fromWindow, toWindow);
        var edgeCost = walk.Pricing.CostOf(windows);

        if (edgeCost < Unreachable)
        {
            Relax(dp, i + length, dp[i] + edgeCost);
        }
    }

    // The condition tests this same entry, so naming it evaluates nothing extra.
    private static long MinimumCostAtEnd(long[] dp)
    {
        var minimumCost = dp[^1];

        return minimumCost < Unreachable ? minimumCost : LeetCodeAnswer.None;
    }

    private static long[] NewDpArray(int n)
    {
        var dp = new long[n + 1];
        Array.Fill(dp, Unreachable);
        dp[0] = 0;

        return dp;
    }

    private static void Relax(long[] dp, int index, long candidate)
    {
        if (candidate < dp[index])
        {
            dp[index] = candidate;
        }
    }

    private static int[] DistinctLengths(IEnumerable<string> keys) =>
        keys.Select(key => key.Length).Distinct().ToArray();

    // One step of the DP walk holds two things: the ends of the window it spans -
    // named for the roles they play, since the conversion graph is directed and
    // source[i..i+L) is not interchangeable with target[i..i+L) - and the pricing
    // the arm in play gives that pair. Pricing is a strategy type, so this stays
    // grouped data the walk carries, with no behaviour of its own.
    private readonly record struct ConversionWalk(
        string Source, string Target, int[] Lengths, IWindowPricing Pricing);

    // The two windows one step of the walk spans, named for the roles they play
    // rather than left as two adjacent `string` positions a caller could hand over
    // the wrong way round with the compiler none the wiser. The conversion graph is
    // directed, so the window at a source position and the window at the same target
    // position are not interchangeable - the same argument LC 2976's SourceText and
    // TargetText make, one window at a time instead of one character.
    private readonly record struct WindowPair(string From, string To);

    // How an arm prices one window pair - the only thing the two arms differ on, and
    // what lets the walk be one method. Behavior handed to the walk, so it is a
    // strategy type with a named operation: the name, the parameter name and the
    // Unreachable contract are written here, where a bare Func<...> had nowhere to
    // put them.
    private interface IWindowPricing
    {
        long CostOf(WindowPair windows);
    }

    // The brute-force arm's pricing: probe its own string index twice and read the
    // hand-rolled matrix. Unreachable is the honest answer for two windows the stated
    // rules never connect, and the walk then skips exactly those.
    private sealed class MatrixWindowPricing(
        Dictionary<string, int> index, long[,] distances) : IWindowPricing
    {
        public long CostOf(WindowPair windows)
        {
            if (!index.TryGetValue(windows.From, out var fromIndex) ||
                !index.TryGetValue(windows.To, out var toIndex))
            {
                return Unreachable;
            }

            return distances[fromIndex, toIndex];
        }
    }

    // The composed arm's pricing: resolve both windows to their network nodes and read
    // the all-pairs table AllPairsShortestPaths produced. A pair the network never
    // connected has no table entry, which is Unreachable again.
    private sealed class NetworkWindowPricing(
        SubstringNetwork network, Dictionary<(StringNode From, StringNode To), int> distances) : IWindowPricing
    {
        public long CostOf(WindowPair windows)
        {
            if (!network.NodesByValue.TryGetValue(windows.From, out var fromNode) ||
                !network.NodesByValue.TryGetValue(windows.To, out var toNode))
            {
                return Unreachable;
            }

            var connected = distances.TryGetValue((fromNode, toNode), out var edgeCost);

            return connected ? edgeCost : Unreachable;
        }
    }
}
