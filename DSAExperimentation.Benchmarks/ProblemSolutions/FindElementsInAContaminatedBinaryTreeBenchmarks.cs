using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindElementsInAContaminatedBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindElementsInAContaminatedBinaryTreeSolution's, the
// same factories FindElementsInAContaminatedBinaryTreeSolutionTests proves correct. Each
// arm recovers the tree once and then answers the same fixed batch of Find queries,
// so the measurement is the recovery walk plus the per-query lookup cost the
// recovered container implies - a List scanned linearly vs. this repo's own
// Set<int>.
//
// The tree is a complete binary tree built in [GlobalSetup] through Fixtures'
// BinaryTrees.Complete with every value -1, the contamination LC 1261 hands in, so
// node i's recovered value is exactly i - the same array-index-as-value shape a
// binary heap has. Half the sampled targets therefore miss, which is the worst case
// for the linear scan and the case the hashed lookup exists for. Recovery writes the
// values over the tree without reading the old ones, so a later invocation, handed
// the recovered tree, does the same work.
public class FindElementsInAContaminatedBinaryTreeBenchmarks
{
    private const int TargetSampleCount = 200;
    private const int TargetRangeMultiplier = 2;
    private const int TargetSeed = 1;

    // The value LC 1261 gives every node of the contaminated tree.
    private const int ContaminatedValue = -1;

    private BinaryTreeNode<int> _root = null!;

    private int[] _targets = [];

    // Every Find verdict, in target order; sized in setup so the queries allocate nothing.
    private bool[] _found = [];
    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var contaminated = Enumerable.Repeat(ContaminatedValue, NodeCount).ToArray();
        _root = BinaryTrees.Complete(contaminated);

        var random = new Random(TargetSeed);
        _targets = SeededDraws.Values(TargetSampleCount, 0, NodeCount * TargetRangeMultiplier, random);
        _found = new bool[_targets.Length];
    }

    [Benchmark(Baseline = true)]
    public bool[] ListRecoverThenLinearScan() =>
        FindAll(FindElementsInAContaminatedBinaryTreeSolution.CreateByListScan(_root));

    [Benchmark]
    public bool[] TopDownRecoverThenSetLookup() =>
        FindAll(FindElementsInAContaminatedBinaryTreeSolution.CreateByTopDownSet(_root));

    private bool[] FindAll(IFindElements elements)
    {
        for (var i = 0; i < _targets.Length; i++)
        {
            _found[i] = elements.Find(_targets[i]);
        }

        return _found;
    }
}
