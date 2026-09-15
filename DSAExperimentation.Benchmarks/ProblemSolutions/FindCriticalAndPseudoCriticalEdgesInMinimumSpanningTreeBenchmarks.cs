using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution's, the same methods
// FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeTests proves correct. Both
// run the identical per-edge Kruskal loop and differ only in how "are these two
// endpoints already connected?" is answered - a fresh BFS over the edges accepted so
// far, O(V+E) per question, against this repo's own DisjointSet, O(a(n)) amortized.
//
// A spanning chain guarantees connectivity; the extra random edges are what give
// Kruskal real ties and cycles to resolve. Building the edge list and sorting it by
// weight is input construction, so it is charged to [GlobalSetup] and handed to each
// strategy's prepared-input overload.
//
// Both arms return LeetCode's actual answer - the two index lists - rather than the
// count of classified edges the previous benchmark measured; the harness takes the
// lengths. See the solution class for the strategies themselves.
[MemoryDiagnoser]
public class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeBenchmarks
{
    // 1489 is the LeetCode problem number, reused here as a fixed benchmark seed.
    private const int RandomSeed = 1489;
    private const int MaxEdgeWeight = 1_000;
    private const int ExtraEdgeMultiplier = 3;

    private WeightedEdgeList _graph = null!;

    [Params(40, 150)]
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
        for (var extra = 0; extra < NodeCount * ExtraEdgeMultiplier; extra++)
        {
            var a = random.Next(NodeCount);
            var b = random.Next(NodeCount);

            if (a != b)
            {
                edges.Add([Math.Min(a, b), Math.Max(a, b), random.Next(1, MaxEdgeWeight)]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveBfsConnectivity()
    {
        var (critical, pseudoCritical) =
            FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution
                .ClassifyEdgesByBfsConnectivity(_graph);

        return critical.Length + pseudoCritical.Length;
    }

    [Benchmark]
    public int DisjointSetKruskal()
    {
        var (critical, pseudoCritical) =
            FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution
                .ClassifyEdgesByDisjointSet(_graph);

        return critical.Length + pseudoCritical.Length;
    }
}
