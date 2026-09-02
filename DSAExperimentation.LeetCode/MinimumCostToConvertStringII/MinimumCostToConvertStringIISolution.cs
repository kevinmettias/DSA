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
        string source, string target, string[] original, string[] changed, int[] cost)
    {
        var index = BuildIndex(original, changed);
        var distances = BuildDistanceMatrix(index, original, changed, cost);

        return MinimumCostByBruteForceFloydWarshall(source, target, index, distances);
    }

    public static long MinimumCostByBruteForceFloydWarshall(
        string source, string target, Dictionary<string, int> index, long[,] distances)
    {
        var lengths = DistinctLengths(index.Keys);
        var dp = NewDpArray(source.Length);

        for (var i = 0; i < source.Length; i++)
        {
            if (dp[i] >= Unreachable)
            {
                continue;
            }

            if (source[i] == target[i])
            {
                Relax(dp, i + 1, dp[i]);
            }

            foreach (var length in lengths)
            {
                if (i + length > source.Length)
                {
                    continue;
                }

                if (!index.TryGetValue(source.Substring(i, length), out var fromIndex) ||
                    !index.TryGetValue(target.Substring(i, length), out var toIndex))
                {
                    continue;
                }

                var edgeCost = distances[fromIndex, toIndex];

                if (edgeCost < Unreachable)
                {
                    Relax(dp, i + length, dp[i] + edgeCost);
                }
            }
        }

        return dp[source.Length] < Unreachable ? dp[source.Length] : LeetCodeAnswer.None;
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
        var size = index.Count;
        var distances = new long[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                distances[i, j] = i == j ? 0 : Unreachable;
            }
        }

        for (var i = 0; i < original.Length; i++)
        {
            var (from, to) = (index[original[i]], index[changed[i]]);
            distances[from, to] = Math.Min(distances[from, to], cost[i]);
        }

        for (var k = 0; k < size; k++)
        {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    if (distances[i, k] + distances[k, j] < distances[i, j])
                    {
                        distances[i, j] = distances[i, k] + distances[k, j];
                    }
                }
            }
        }

        return distances;
    }

    // Algorithms.ShortestPaths.AllPairsShortestPaths' Floyd-Warshall over the
    // SubstringNetwork's distinct-string graph - the same composition
    // MinimumCostToConvertStringISolution uses for its 26-letter graph,
    // generalized to whatever distinct strings the rules mention.
    public static long MinimumCostByAllPairsShortestPaths(
        string source, string target, string[] original, string[] changed, int[] cost) =>
        MinimumCostByAllPairsShortestPaths(source, target, SubstringNetwork.Build(original, changed, cost));

    public static long MinimumCostByAllPairsShortestPaths(string source, string target, SubstringNetwork network)
    {
        AllPairsShortestPaths.TryComputeDistances<StringNode, StringTopology, ListEdges<StringNode, int>, int>(
            network.NodesByValue.Values, out var distances);

        var lengths = DistinctLengths(network.NodesByValue.Keys);
        var dp = NewDpArray(source.Length);

        for (var i = 0; i < source.Length; i++)
        {
            if (dp[i] >= Unreachable)
            {
                continue;
            }

            if (source[i] == target[i])
            {
                Relax(dp, i + 1, dp[i]);
            }

            foreach (var length in lengths)
            {
                if (i + length > source.Length)
                {
                    continue;
                }

                if (!network.NodesByValue.TryGetValue(source.Substring(i, length), out var fromNode) ||
                    !network.NodesByValue.TryGetValue(target.Substring(i, length), out var toNode))
                {
                    continue;
                }

                if (distances.TryGetValue((fromNode, toNode), out var edgeCost))
                {
                    Relax(dp, i + length, dp[i] + edgeCost);
                }
            }
        }

        return dp[source.Length] < Unreachable ? dp[source.Length] : LeetCodeAnswer.None;
    }

    // dp[i] is the minimum cost to convert source[0..i) into target[0..i);
    // shared bookkeeping between both arms above - neither strategy differs
    // on how the DP walk is shaped, only on what computes an edge's cost.
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
}
