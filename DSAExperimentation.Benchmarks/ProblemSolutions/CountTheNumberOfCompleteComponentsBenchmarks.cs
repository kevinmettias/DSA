using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheNumberOfCompleteComponents;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheNumberOfCompleteComponentsSolution's, the
// same methods CountTheNumberOfCompleteComponentsTests proves correct. The
// workload is NodeCount / CliqueSize disjoint cliques, every one of them already
// complete, so no early mismatch cuts the pairwise scan short - the O(k^2)
// membership tests inside each fixed-size clique, not the O(V+E) edge sweep both
// arms share, are what the gap comes from. Building the edge array is charged to
// [GlobalSetup]; it is already LeetCode's own input shape, so both arms take it
// directly and neither strategy needs a hoisted overload.
[MemoryDiagnoser]
public class CountTheNumberOfCompleteComponentsBenchmarks
{
    private const int CliqueSize = 25;

    // NodeCount / CliqueSize disjoint cliques, each already complete.
    [Params(250, 2_500)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var edges = new List<int[]>();

        for (var first = 0; first < NodeCount; first += CliqueSize)
        {
            for (var i = first; i < first + CliqueSize; i++)
            {
                for (var j = i + 1; j < first + CliqueSize; j++)
                {
                    edges.Add([i, j]);
                }
            }
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public int AdjacencySetPairwiseScan() =>
        CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByAdjacencySetScan(NodeCount, _edges);

    [Benchmark]
    public int DisjointSetTally() =>
        CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByDisjointSetTally(NodeCount, _edges);
}
