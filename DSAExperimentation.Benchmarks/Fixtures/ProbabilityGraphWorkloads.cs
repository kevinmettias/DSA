namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1514 - every node i > 0 gets one edge to an
// earlier node j < i, guaranteeing node n-1 is connected to node 0; extra edges per
// node add the branching density the exhaustive-path baseline needs in order to
// actually explore an exponential number of distinct paths. Every edge is stated
// low-to-high (NetworkRecoveryWorkloads' own forward-only convention), which keeps the
// generated edge list independent of how the solution orients it - LC 1514's edges are
// undirected.
//
// Probabilities land in (0.5, 0.99], high enough that long paths stay competitive with
// short ones so the search cannot be won by a trivially dominant single edge.
internal static class ProbabilityGraphWorkloads
{
    private const double MinEdgeProbability = 0.5;
    private const double EdgeProbabilityRange = 0.49;

    public static (int[][] Edges, double[] SuccessProbabilities) Build(
        int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var generated = new List<GeneratedEdge>();

        for (var i = 1; i < nodeCount; i++)
        {
            var from = random.Next(i);
            AddEdge(generated, from, i, random);
        }

        for (var i = 0; i < nodeCount - 1; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var to = random.Next(i + 1, nodeCount);
                AddEdge(generated, i, to, random);
            }
        }

        return ToLeetCodeShape(generated);
    }

    private static void AddEdge(List<GeneratedEdge> generated, int from, int to, Random random)
    {
        var probability = MinEdgeProbability + (random.NextDouble() * EdgeProbabilityRange);
        var edge = new GeneratedEdge(from, to, probability);

        generated.Add(edge);
    }

    // LC 1514 states the two halves of an edge separately - edges[i] and succProb[i] -
    // so the generated pairs are split apart only at the boundary.
    private static (int[][] Edges, double[] SuccessProbabilities) ToLeetCodeShape(
        List<GeneratedEdge> generated)
    {
        var edges = new int[generated.Count][];
        var probabilities = new double[generated.Count];

        for (var i = 0; i < generated.Count; i++)
        {
            edges[i] = [generated[i].From, generated[i].To];
            probabilities[i] = generated[i].Probability;
        }

        return (edges, probabilities);
    }

    private readonly record struct GeneratedEdge(int From, int To, double Probability);
}
