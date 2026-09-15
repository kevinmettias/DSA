using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

// LeetCode 2959. Number of Possible Sets of Closing Branches: n branches (n <= 10)
// connected by weighted roads. A set of branches may be closed if every pair of
// branches that stays open can still reach each other, using only other open
// branches, within maxDistance. Count how many sets of branches may be closed.
//
// Closing a set of branches and keeping the complement open is a bijection, so
// counting valid closing sets is the same as counting valid open sets - every
// subset of the n branches is tried directly as "the open set". n <= 10 makes
// both the 2^n subset enumeration and an O(n^3) all-pairs distance pass per
// subset cheap.
internal static class NumberOfPossibleSetsOfClosingBranchesSolution
{
    // The sentinel for a pair with no road between them, scaled below
    // long.MaxValue so adding two of them during a Floyd-Warshall relaxation
    // cannot overflow.
    private const long Unreachable = long.MaxValue / 4;

    // Textbook baseline: a plain BCL long[,] distance matrix and a hand-rolled
    // Floyd-Warshall per subset - deliberately without this repo's own
    // AllPairsShortestPaths, the arm the composed strategy below has to justify
    // itself against.
    public static long CountClosingSetsByBruteForceFloydWarshall(int n, int[][] roads, int maxDistance)
    {
        var baseDistances = BuildDistanceMatrix(n, roads);
        return CountClosingSetsByBruteForceFloydWarshall(baseDistances, maxDistance);
    }

    public static long CountClosingSetsByBruteForceFloydWarshall(long[,] baseDistances, int maxDistance)
    {
        var n = baseDistances.GetLength(0);
        var validSets = 0L;

        for (var openMask = 0; openMask < (1 << n); openMask++)
        {
            if (IsValidOpenSetByMatrix(baseDistances, n, openMask, maxDistance))
            {
                validSets++;
            }
        }

        return validSets;
    }

    // Public so a benchmark's [GlobalSetup] can build the same prepared matrix
    // this overload's own hoisting builds internally, rather than reimplementing
    // it - the same role LockGraph.Build plays for OpenTheLock's benchmark.
    public static long[,] BuildDistanceMatrix(int n, int[][] roads)
    {
        var distances = new long[n, n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                distances[i, j] = i == j ? 0 : Unreachable;
            }
        }

        foreach (var road in roads)
        {
            var (from, to, weight) = (road[0], road[1], (long)road[2]);
            distances[from, to] = Math.Min(distances[from, to], weight);
            distances[to, from] = Math.Min(distances[to, from], weight);
        }

        return distances;
    }

    private static bool IsValidOpenSetByMatrix(long[,] baseDistances, int n, int openMask, int maxDistance)
    {
        var openBranches = SelectOpenBranches(n, openMask);

        if (openBranches.Count <= 1)
        {
            return true;
        }

        var distances = RestrictAndRefine(baseDistances, openBranches);

        return EveryPairWithinLimit(distances, openBranches.Count, maxDistance);
    }

    private static long[,] RestrictAndRefine(long[,] baseDistances, List<int> openBranches)
    {
        var distances = RestrictToOpenBranches(baseDistances, openBranches);
        RefineShortestPaths(distances);
        return distances;
    }

    // Dropping the closed branches as endpoints: the restricted matrix holds only
    // the open set's own pairwise base distances.
    private static long[,] RestrictToOpenBranches(long[,] baseDistances, List<int> openBranches)
    {
        var size = openBranches.Count;
        var distances = new long[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                distances[i, j] = baseDistances[openBranches[i], openBranches[j]];
            }
        }

        return distances;
    }

    // Floyd-Warshall over the restricted matrix: relaxing through vertex k only
    // ever uses a k drawn from the open set, which is what confines routing to
    // open branches.
    private static void RefineShortestPaths(long[,] distances)
    {
        var size = distances.GetLength(0);

        for (var k = 0; k < size; k++)
        {
            RelaxThroughIntermediate(distances, k, size);
        }
    }

    private static void RelaxThroughIntermediate(long[,] distances, int k, int size)
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

    private static bool EveryPairWithinLimit(long[,] distances, int size, int maxDistance)
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (i != j && distances[i, j] > maxDistance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Reduce.Graph has no all-pairs shape, so this composes
    // Algorithms.ShortestPaths.AllPairsShortestPaths directly instead: for each
    // candidate open set, restricting the vertex list handed to Floyd-Warshall
    // both drops the closed branches as endpoints and - because Refine only ever
    // uses vertices from that same list as an intermediate - as a routing hop,
    // which is exactly "may only route through other open branches".
    public static long CountClosingSetsByAllPairsShortestPaths(int n, int[][] roads, int maxDistance)
    {
        var network = BranchNetwork.Build(n, roads);
        return CountClosingSetsByAllPairsShortestPaths(network, maxDistance);
    }

    public static long CountClosingSetsByAllPairsShortestPaths(BranchNetwork network, int maxDistance)
    {
        var n = network.Nodes.Length;
        var validSets = 0L;

        for (var openMask = 0; openMask < (1 << n); openMask++)
        {
            if (IsValidOpenSetByGraph(network, openMask, maxDistance))
            {
                validSets++;
            }
        }

        return validSets;
    }

    private static bool IsValidOpenSetByGraph(BranchNetwork network, int openMask, int maxDistance)
    {
        var openNodes = SelectOpenNodes(network, openMask);
        if (openNodes.Count <= 1)
        {
            return true;
        }

        AllPairsShortestPaths.TryComputeDistances<BranchNode, BranchTopology, ListEdges<BranchNode, int>, int>(
            openNodes, out var distances);

        return AllPairsWithinLimit(distances, openNodes, maxDistance);
    }

    // A pair further apart than maxDistance - or with no route at all among the
    // open set - makes the set invalid, and the missing-key arm is that same
    // refusal: the distance dictionary holds an unreached sentinel rather than
    // dropping the pair.
    private static bool AllPairsWithinLimit(
        Dictionary<(BranchNode From, BranchNode To), int> distances, List<BranchNode> openNodes, int maxDistance)
    {
        foreach (var from in openNodes)
        {
            foreach (var to in openNodes)
            {
                if (from == to)
                {
                    continue;
                }

                if (!distances.TryGetValue((from, to), out var distance) || distance > maxDistance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static List<int> SelectOpenBranches(int n, int openMask)
    {
        var openBranches = new List<int>();

        for (var i = 0; i < n; i++)
        {
            if ((openMask & (1 << i)) != 0)
            {
                openBranches.Add(i);
            }
        }

        return openBranches;
    }

    private static List<BranchNode> SelectOpenNodes(BranchNetwork network, int openMask)
    {
        var openNodes = new List<BranchNode>();

        for (var i = 0; i < network.Nodes.Length; i++)
        {
            if ((openMask & (1 << i)) != 0)
            {
                openNodes.Add(network.Nodes[i]);
            }
        }

        return openNodes;
    }
}
