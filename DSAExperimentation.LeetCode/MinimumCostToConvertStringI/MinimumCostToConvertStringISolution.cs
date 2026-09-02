using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

// LeetCode 2976. Minimum Cost to Convert String I: source[i] must become
// target[i] at every position, one character at a time, via any chain of
// the given (original, changed, cost) rules. Each position is an
// independent shortest-path query on the 26-letter conversion graph, so the
// whole problem reduces to "sum one all-pairs lookup per position".
internal static class MinimumCostToConvertStringISolution
{
    private const int Alphabet = 26;
    private const long Unreachable = long.MaxValue / 4;

    // Textbook baseline: a plain BCL long[26,26] distance matrix and a
    // hand-rolled Floyd-Warshall - deliberately without this repo's own
    // AllPairsShortestPaths, the arm the composed strategy below has to
    // justify itself against. Same shape as
    // NumberOfPossibleSetsOfClosingBranchesSolution's brute-force arm.
    public static long MinimumCostByBruteForceFloydWarshall(
        string source, string target, char[] original, char[] changed, int[] cost) =>
        MinimumCostByBruteForceFloydWarshall(source, target, BuildDistanceMatrix(original, changed, cost));

    public static long MinimumCostByBruteForceFloydWarshall(string source, string target, long[,] distances)
    {
        var total = 0L;

        for (var i = 0; i < source.Length; i++)
        {
            var distance = distances[source[i] - 'a', target[i] - 'a'];

            if (distance >= Unreachable)
            {
                return LeetCodeAnswer.None;
            }

            total += distance;
        }

        return total;
    }

    // Public so a benchmark's [GlobalSetup] can build the same prepared
    // matrix this overload's own hoisting builds internally, rather than
    // reimplementing it - the same role BranchNetwork.Build plays for
    // NumberOfPossibleSetsOfClosingBranches's benchmark.
    public static long[,] BuildDistanceMatrix(char[] original, char[] changed, int[] cost)
    {
        var distances = new long[Alphabet, Alphabet];

        for (var i = 0; i < Alphabet; i++)
        {
            for (var j = 0; j < Alphabet; j++)
            {
                distances[i, j] = i == j ? 0 : Unreachable;
            }
        }

        for (var i = 0; i < original.Length; i++)
        {
            var (from, to) = (original[i] - 'a', changed[i] - 'a');
            distances[from, to] = Math.Min(distances[from, to], cost[i]);
        }

        for (var k = 0; k < Alphabet; k++)
        {
            for (var i = 0; i < Alphabet; i++)
            {
                for (var j = 0; j < Alphabet; j++)
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
    // 26-node LetterNetwork - the same composition
    // NumberOfPossibleSetsOfClosingBranchesSolution's
    // CountClosingSetsByAllPairsShortestPaths uses for BranchNetwork.
    public static long MinimumCostByAllPairsShortestPaths(
        string source, string target, char[] original, char[] changed, int[] cost) =>
        MinimumCostByAllPairsShortestPaths(source, target, LetterNetwork.Build(original, changed, cost));

    public static long MinimumCostByAllPairsShortestPaths(string source, string target, LetterNetwork network)
    {
        AllPairsShortestPaths.TryComputeDistances<LetterNode, LetterTopology, ListEdges<LetterNode, int>, int>(
            network.Nodes, out var distances);

        var total = 0L;

        for (var i = 0; i < source.Length; i++)
        {
            var from = network.Nodes[source[i] - 'a'];
            var to = network.Nodes[target[i] - 'a'];

            if (!distances.TryGetValue((from, to), out var distance))
            {
                return LeetCodeAnswer.None;
            }

            total += distance;
        }

        return total;
    }
}
