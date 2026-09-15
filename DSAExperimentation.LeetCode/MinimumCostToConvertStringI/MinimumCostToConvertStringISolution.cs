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
        SourceText source, TargetText target, (char[] Original, char[] Changed, int[] Cost) rules)
    {
        var distances = BuildDistanceMatrix(rules.Original, rules.Changed, rules.Cost);

        return MinimumCostByBruteForceFloydWarshall(source, target, distances);
    }

    public static long MinimumCostByBruteForceFloydWarshall(
        SourceText source, TargetText target, long[,] distances)
    {
        var total = 0L;

        for (var i = 0; i < source.Text.Length; i++)
        {
            var distance = distances[source.Text[i] - 'a', target.Text[i] - 'a'];

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
        var distances = BuildIdentityMatrix();

        ApplyDirectConversions(distances, (original, changed, cost));
        CloseUnderTransitiveChains(distances);

        return distances;
    }

    // The starting matrix: every letter reaches itself at no cost and every other
    // letter not at all, until a stated rule or a chain of them proves otherwise.
    private static long[,] BuildIdentityMatrix()
    {
        var distances = new long[Alphabet, Alphabet];

        for (var i = 0; i < Alphabet; i++)
        {
            for (var j = 0; j < Alphabet; j++)
            {
                distances[i, j] = i == j ? 0 : Unreachable;
            }
        }

        return distances;
    }

    // Every stated rule is one direct edge, and a conversion stated more than once
    // keeps its cheapest cost.
    private static void ApplyDirectConversions(
        long[,] distances, (char[] Original, char[] Changed, int[] Cost) rules)
    {
        for (var i = 0; i < rules.Original.Length; i++)
        {
            var (from, to) = (rules.Original[i] - 'a', rules.Changed[i] - 'a');
            distances[from, to] = Math.Min(distances[from, to], rules.Cost[i]);
        }
    }

    // Floyd-Warshall: each letter takes a turn as an intermediate hop, so after its
    // pass no i -> j entry is worse than routing that one letter.
    private static void CloseUnderTransitiveChains(long[,] distances)
    {
        for (var through = 0; through < Alphabet; through++)
        {
            for (var from = 0; from < Alphabet; from++)
            {
                RelaxThrough(distances, from, through);
            }
        }
    }

    // One row of a pass: does going from -> through -> j beat the best from -> j so far?
    private static void RelaxThrough(long[,] distances, int from, int through)
    {
        for (var to = 0; to < Alphabet; to++)
        {
            var viaIntermediate = distances[from, through] + distances[through, to];

            if (viaIntermediate < distances[from, to])
            {
                distances[from, to] = viaIntermediate;
            }
        }
    }

    // Algorithms.ShortestPaths.AllPairsShortestPaths' Floyd-Warshall over the
    // 26-node LetterNetwork - the same composition
    // NumberOfPossibleSetsOfClosingBranchesSolution's
    // CountClosingSetsByAllPairsShortestPaths uses for BranchNetwork.
    // The three parallel arrays are one rule set - the (original, changed, cost)
    // triples the problem states - so they are one argument rather than three whose
    // order only the signature remembers.
    public static long MinimumCostByAllPairsShortestPaths(
        SourceText source, TargetText target, (char[] Original, char[] Changed, int[] Cost) rules)
    {
        var network = LetterNetwork.Build(rules.Original, rules.Changed, rules.Cost);

        return MinimumCostByAllPairsShortestPaths(source, target, network);
    }

    public static long MinimumCostByAllPairsShortestPaths(
        SourceText source, TargetText target, LetterNetwork network)
    {
        AllPairsShortestPaths.TryComputeDistances<LetterNode, LetterTopology, ListEdges<LetterNode, int>, int>(
            network.Nodes, out var distances);

        var total = 0L;

        for (var i = 0; i < source.Text.Length; i++)
        {
            var from = network.Nodes[source.Text[i] - 'a'];
            var to = network.Nodes[target.Text[i] - 'a'];

            if (!distances.TryGetValue((from, to), out var distance))
            {
                return LeetCodeAnswer.None;
            }

            total += distance;
        }

        return total;
    }

    // The two ends of every per-position conversion, named for the roles they play here
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser. The conversion graph is
    // directed - `distances[source[i] - 'a', target[i] - 'a']` reads one entry and not
    // its mirror - so the source letter each position starts from and the target letter
    // it must become are not interchangeable.
    internal readonly record struct SourceText(string Text);

    internal readonly record struct TargetText(string Text);
}
