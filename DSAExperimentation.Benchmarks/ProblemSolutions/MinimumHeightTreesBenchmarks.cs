using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumHeightTrees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumHeightTreesSolution's, the same methods
// MinimumHeightTreesTests proves correct. Each arm is handed the prepared adjacency
// list its hoisted overload takes, so tree construction is charged to [GlobalSetup]
// rather than to the search being measured.
[MemoryDiagnoser]
public class MinimumHeightTreesBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private List<int>[] _adjacency = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            _adjacency[i] = [];
        }

        // A random recursive tree: each node (after the first) attaches to a
        // uniformly-chosen earlier node, giving a connected, cycle-free graph on
        // NodeCount vertices with NodeCount-1 edges.
        for (var i = 1; i < NodeCount; i++)
        {
            var parent = random.Next(i);
            _adjacency[i].Add(parent);
            _adjacency[parent].Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> HeightFromEveryNode() =>
        MinimumHeightTreesSolution.FindRootsByHeightFromEveryNode(_adjacency);

    [Benchmark]
    public List<int> LeafPeeling() =>
        MinimumHeightTreesSolution.FindRootsByLeafPeeling(_adjacency);
}
