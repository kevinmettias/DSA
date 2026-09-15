using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindElementsInAContaminatedBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindElementsInAContaminatedBinaryTreeSolution's, the
// same factories FindElementsInAContaminatedBinaryTreeTests proves correct. Each
// arm recovers the tree once and then answers the same fixed batch of Find queries,
// so the measurement is the recovery walk plus the per-query lookup cost the
// recovered container implies - a List scanned linearly vs. this repo's own
// Set<int>.
//
// The tree is a complete binary tree built in [GlobalSetup] from Fixtures'
// BinaryTrees.Balanced, so node i's recovered value is exactly i - the same
// array-index-as-value shape a binary heap has. Half the sampled targets therefore
// miss, which is the worst case for the linear scan and the case the hashed lookup
// exists for.
[MemoryDiagnoser]
public class FindElementsInAContaminatedBinaryTreeBenchmarks
{
    private const int TargetSampleCount = 200;
    private const int TargetRangeMultiplier = 2;
    private const int TargetSeed = 1;

    private BinaryTreeNode<int> _root = null!;

    private int[] _targets = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _root = BinaryTrees.Balanced(NodeCount);

        var random = new Random(TargetSeed);
        _targets = Enumerable.Range(0, TargetSampleCount)
            .Select(_ => random.Next(0, NodeCount * TargetRangeMultiplier))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListRecoverThenLinearScan() =>
        CountFound(FindElementsInAContaminatedBinaryTreeSolution.CreateByListScan(_root));

    [Benchmark]
    public int TopDownRecoverThenSetLookup() =>
        CountFound(FindElementsInAContaminatedBinaryTreeSolution.CreateByTopDownSet(_root));

    private int CountFound(IFindElements elements)
    {
        var found = 0;

        foreach (var target in _targets)
        {
            if (elements.Find(target))
            {
                found++;
            }
        }

        return found;
    }
}
