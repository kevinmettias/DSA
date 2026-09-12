using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RedundantConnectionII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RedundantConnectionIISolution's, the same methods
// RedundantConnectionIITests proves correct - the textbook brute force retries
// validity from scratch for every candidate edge removal (an in-degree pass plus
// an uncompressed find-root cycle pass per candidate, O(n) each, O(n^2) overall)
// against this repo's own DisjointSet-based approach, which finds the
// in-degree-2 node's two candidate edges up front and settles the answer in at
// most two O(n * alpha(n)) DisjointSet passes total.
[MemoryDiagnoser]
public class RedundantConnectionIIBenchmarks
{
    // The chain's extra cross edge targets node 2, which is what gives it two parents.
    private const int DoublyParentedNode = 2;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        // A straight-line chain 1->2->...->n plus one extra edge n->2, giving node 2
        // two parents - edges targeting node 2 sit at the very first and very last
        // array positions, so every candidate removal in between needs a near-full
        // scan before either check can rule it out. Removing the extra (last) edge
        // is the only fix, forcing both strategies through real work rather than an
        // early-exit on the first candidate tried.
        var edges = new int[NodeCount][];
        for (var i = 0; i < NodeCount - 1; i++)
        {
            var parent = i + 1;
            edges[i] = [parent, parent + 1];
        }

        edges[NodeCount - 1] = [NodeCount, DoublyParentedNode];
        _edges = edges;
    }

    [Benchmark(Baseline = true)]
    public int[] FindRedundantEdgeByRemovalScan() =>
        RedundantConnectionIISolution.FindRedundantEdgeByRemovalScan(_edges);

    [Benchmark]
    public int[] FindRedundantEdgeByDisjointSet() =>
        RedundantConnectionIISolution.FindRedundantEdgeByDisjointSet(_edges);
}
