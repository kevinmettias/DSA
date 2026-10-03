using DSAExperimentation.LeetCode.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution's, the same methods
// FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolutionTests proves correct. Both
// run the identical per-edge Kruskal loop and differ only in how "are these two
// endpoints already connected?" is answered - a fresh BFS over the edges accepted so
// far, O(V+E) per question, against this repo's own DisjointSet, O(a(n)) amortized.
//
// A spanning chain guarantees connectivity; the extra random edges are what give
// Kruskal real ties and cycles to resolve. Building the edge list and sorting it by
// weight is input construction, so it is charged to [GlobalSetup] and handed to each
// strategy's prepared-input overload. LC 1489 caps a graph at 100 nodes and 200
// edges and lists each pair once, so the larger NodeCount is 100, a drawn pair that
// repeats an earlier one is skipped, and the extra edges stop at the 200th.
//
// Both arms return LeetCode's actual answer - the two index lists - rather than the
// count of classified edges the previous benchmark measured. See the solution class
// for the strategies themselves.
public class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks
{
    // 1489 is the LeetCode problem number, reused here as a fixed benchmark seed.
    private const int RandomSeed = 1489;
    private const int MaxEdgeWeight = 1_000;
    private const int ExtraEdgeMultiplier = 3;

    private const int MaxEdgeCount = 200;

    private WeightedEdgeList _graph = null!;

    [Params(40, 100)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = new List<int[]>();

        AddSpanningChainEdges(edges, random);
        AddExtraRandomEdges(edges, random);

        _graph = WeightedEdgeList.Build(NodeCount, [.. edges]);
    }

    private void AddSpanningChainEdges(List<int[]> edges, Random random)
    {
        for (var node = 1; node < NodeCount; node++)
        {
            edges.Add([node - 1, node, random.Next(1, MaxEdgeWeight)]);
        }
    }

    private void AddExtraRandomEdges(List<int[]> edges, Random random)
    {
        var chainPairs = edges.Select(edge => (edge[0], edge[1]));
        var listed = new HashSet<(int, int)>(chainPairs);

        for (var extra = 0; extra < NodeCount * ExtraEdgeMultiplier; extra++)
        {
            var a = random.Next(NodeCount);
            var b = random.Next(NodeCount);

            if (a != b)
            {
                // The weight is drawn whether or not the edge is kept, so every later draw stays
                // where it was.
                int[] edge = [Math.Min(a, b), Math.Max(a, b), random.Next(1, MaxEdgeWeight)];
                AddIfUnlisted(edges, listed, edge);
            }
        }
    }

    // Keeps an edge only while there is room under LC 1489's 200 and its pair is not listed yet.
    private static void AddIfUnlisted(List<int[]> edges, HashSet<(int, int)> listed, int[] edge)
    {
        var hasRoom = edges.Count < MaxEdgeCount;

        if (hasRoom && listed.Add((edge[0], edge[1])))
        {
            edges.Add(edge);
        }
    }

    [Benchmark(Baseline = true)]
    public (int[] Critical, int[] PseudoCritical) NaiveBfsConnectivity() =>
        FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution.ClassifyEdgesByBfsConnectivity(_graph);

    [Benchmark]
    public (int[] Critical, int[] PseudoCritical) DisjointSetKruskal() =>
        FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution.ClassifyEdgesByDisjointSet(_graph);
}
