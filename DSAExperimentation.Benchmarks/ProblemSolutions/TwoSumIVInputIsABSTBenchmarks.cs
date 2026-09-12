using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TwoSumIVInputIsABST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumIVInputIsABSTSolution's, the same methods
// TwoSumIVInputIsABSTTests proves correct. Target is deliberately unreachable
// (every node value non-negative, target negative) - the same TwoSumBenchmarks
// convention - so both strategies are forced through their full worst-case walk
// instead of an early exit on the first invocation making the nested-loop
// baseline look artificially competitive.
[MemoryDiagnoser]
public class TwoSumIVInputIsABSTBenchmarks
{
    private const int Target = -1;
    private const int RandomSeed = 653;

    [Params(200, 5_000)]
    public int NodeCount;

    private BinarySearchTree<int> _tree = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(0, NodeCount).ToArray();

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            _tree.Insert(value);
        }
    }

    [Benchmark(Baseline = true)]
    public bool CollectThenNestedPairScan() =>
        TwoSumIVInputIsABSTSolution.FindTargetByNestedPairScan(_tree.Root, Target);

    [Benchmark]
    public bool DepthFirstSetLookup() =>
        TwoSumIVInputIsABSTSolution.FindTargetByDepthFirstSetLookup(_tree.Root, Target);
}
