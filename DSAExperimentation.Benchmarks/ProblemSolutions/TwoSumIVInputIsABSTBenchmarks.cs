using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Two Sum IV - Input is a BST (LC 653): collect every node's value into a List<int>
// via a plain traversal, then run TwoSumBenchmarks' own O(n^2) nested-pair scan
// baseline vs. this repo's own BinarySearchTree<int> (build) + Set<int> (a single
// DFS checking each node's complement against every value seen so far) - O(n) in one
// pass, no second array ever materialized. Target is deliberately unreachable (every
// node value non-negative, target negative), the same TwoSumBenchmarks convention,
// so both strategies are forced through their full worst-case walk instead of an
// early exit on the first invocation making the nested-loop baseline look
// artificially competitive.
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
    public bool CollectThenNestedPairScan()
    {
        var values = new List<int>();
        Collect(_tree.Root, values);

        for (var i = 0; i < values.Count; i++)
        {
            for (var j = i + 1; j < values.Count; j++)
            {
                if (values[i] + values[j] == Target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool DepthFirstSetLookup() => FindTarget(_tree.Root, Target, new Set<int>());

    private static void Collect(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        Collect(node.Left, values);
        Collect(node.Right, values);
    }

    private static bool FindTarget(BinaryTreeNode<int>? node, int k, Set<int> seen)
    {
        if (node is null)
        {
            return false;
        }

        if (seen.Has(k - node.Value))
        {
            return true;
        }

        seen.TryAdd(node.Value);
        return FindTarget(node.Left, k, seen) || FindTarget(node.Right, k, seen);
    }
}
