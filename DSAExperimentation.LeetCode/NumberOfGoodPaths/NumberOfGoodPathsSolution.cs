using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.NumberOfGoodPaths;

// LeetCode 2421. Number of Good Paths: count the paths in a tree whose two
// endpoints share the same value and whose every intermediate node is no larger
// than that value. Every single node is trivially one such path, so the count
// starts at n.
//
// The two strategies differ in how they decide whether a pair's connecting path
// stays under its shared value: walk that path and look, or arrange never to have
// to look - process the edges in increasing order of their higher-valued endpoint,
// so at the moment two components are joined nothing already inside either of them
// exceeds the value being considered.
internal static class NumberOfGoodPathsSolution
{
    // The textbook answer: build an adjacency list, then for each of the O(n^2)
    // pairs sharing a value, walk the tree between them (O(n)) refusing to step
    // onto anything larger than that value - if the walk arrives, the unique tree
    // path between them is good. Deliberately written without this repo's
    // DisjointSet, out of a BCL List/Stack/bool[] and nothing else; it is the arm
    // the composed strategy below has to justify itself against.
    public static int CountGoodPathsByPairwisePathWalk(int[] vals, int[][] edges)
    {
        var adjacency = BuildAdjacency(vals.Length, edges);
        var goodPaths = vals.Length;

        for (var first = 0; first < vals.Length; first++)
        {
            for (var second = first + 1; second < vals.Length; second++)
            {
                if (vals[first] == vals[second] && IsGoodPath(adjacency, vals, first, second))
                {
                    goodPaths++;
                }
            }
        }

        return goodPaths;
    }

    private static List<int>[] BuildAdjacency(int nodeCount, int[][] edges)
    {
        var adjacency = new List<int>[nodeCount];

        for (var node = 0; node < nodeCount; node++)
        {
            adjacency[node] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    // Depth-first from one endpoint, never stepping onto a node whose value
    // exceeds the endpoints' shared value. In a tree there is exactly one path
    // between the two, so arriving at all means that path is good.
    private static bool IsGoodPath(List<int>[] adjacency, int[] vals, int start, int end)
    {
        var maxAllowed = vals[start];
        var walk = new PathWalk(adjacency, vals, new Stack<int>(), new bool[adjacency.Length]);
        walk.Stack.Push(start);
        walk.Visited[start] = true;

        while (walk.Stack.Count > 0)
        {
            var node = walk.Stack.Pop();

            if (node == end)
            {
                return true;
            }

            PushOpenNeighbors(walk, node, maxAllowed);
        }

        return false;
    }

    private static void PushOpenNeighbors(PathWalk walk, int node, int maxAllowed)
    {
        foreach (var neighbor in walk.Adjacency[node])
        {
            if (!walk.Visited[neighbor] && walk.Vals[neighbor] <= maxAllowed)
            {
                walk.Visited[neighbor] = true;
                walk.Stack.Push(neighbor);
            }
        }
    }

    // The scratch state one pair's depth-first walk threads through: the tree it
    // walks, the values it compares against, and the frontier and visited marks it
    // keeps. Bundled the same way OpenTheLockSolution's TurnWalk bundles its BFS
    // state, and out of BCL types only, since this is the baseline arm.
    private readonly record struct PathWalk(
        List<int>[] Adjacency, int[] Vals, Stack<int> Stack, bool[] Visited);

    // This repo's own DisjointSet (Union-Find), sweeping the edges in increasing
    // order of their higher-valued endpoint. Every component carries the largest
    // value inside it and how many of its nodes attain that value; when two
    // components about to be joined agree on that maximum, every max-value node on
    // one side pairs with every max-value node on the other, and each such pair's
    // connecting path is good because no earlier-processed edge could have brought
    // a larger value into either side.
    public static int CountGoodPathsByDisjointSetSweep(int[] vals, int[][] edges)
    {
        var components = new ValueComponents(
            new DisjointSet(vals.Length), (int[])vals.Clone(), new int[vals.Length]);
        Array.Fill(components.MaxCount, 1);

        var goodPaths = vals.Length;

        foreach (var edge in edges.OrderBy(edge => Math.Max(vals[edge[0]], vals[edge[1]])))
        {
            goodPaths += Join(components, edge[0], edge[1]);
        }

        return goodPaths;
    }

    // Join the two endpoints' components and report how many good paths the join
    // newly connects - the product of both sides' max-value counts when the two
    // maxima agree, and none otherwise, since a smaller side contributes no
    // endpoint that could match the larger one.
    private static int Join(ValueComponents components, int first, int second)
    {
        var firstRoot = components.Nodes.Find(first);
        var secondRoot = components.Nodes.Find(second);

        if (firstRoot == secondRoot)
        {
            return 0;
        }

        var firstMax = components.MaxValue[firstRoot];
        var secondMax = components.MaxValue[secondRoot];
        var newPaths = firstMax == secondMax
            ? components.MaxCount[firstRoot] * components.MaxCount[secondRoot]
            : 0;
        var mergedCount = MergedCount(components, firstRoot, secondRoot);

        components.Nodes.Union(first, second);
        var mergedRoot = components.Nodes.Find(first);
        components.MaxValue[mergedRoot] = Math.Max(firstMax, secondMax);
        components.MaxCount[mergedRoot] = mergedCount;

        return newPaths;
    }

    // How many nodes attain the merged component's maximum: both sides' counts when
    // they tie, otherwise only the larger side's, since the smaller side's nodes are
    // all strictly below the surviving maximum.
    private static int MergedCount(ValueComponents components, int firstRoot, int secondRoot)
    {
        var firstMax = components.MaxValue[firstRoot];
        var secondMax = components.MaxValue[secondRoot];

        if (firstMax == secondMax)
        {
            return components.MaxCount[firstRoot] + components.MaxCount[secondRoot];
        }

        return firstMax > secondMax ? components.MaxCount[firstRoot] : components.MaxCount[secondRoot];
    }

    // The per-component bookkeeping threaded alongside Find/Union, indexed by
    // component root: the largest value inside that component and how many of its
    // nodes attain it. Bundled the same way OpenTheLockSolution's TurnWalk bundles
    // its BFS state.
    private readonly record struct ValueComponents(DisjointSet Nodes, int[] MaxValue, int[] MaxCount);
}
