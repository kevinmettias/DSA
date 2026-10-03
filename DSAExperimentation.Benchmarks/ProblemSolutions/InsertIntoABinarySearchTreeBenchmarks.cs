using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InsertIntoABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only. Both arms are InsertIntoABinarySearchTreeSolution's, the same
// methods InsertIntoABinarySearchTreeSolutionTests proves correct. Each arm builds its
// own tree from the same shuffled insertion order every call (the original
// convention, preserved here), so the comparison stays "full construction plus
// one insert" for both strategies rather than only the incremental insert cost.
public class InsertIntoABinarySearchTreeBenchmarks
{
    private const int ValueStride = 2;

    private int[] _insertionOrder = [];

    private int _newValue;
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).Select(v => v * ValueStride).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _insertionOrder = values;
        _newValue = 1; // odd, so it always lands as a brand-new key between two existing even keys
    }

    // Each arm returns the root of the tree it built, as object because
    // BinaryTreeNode<int> is internal and a [Benchmark] method must be public.
    [Benchmark(Baseline = true)]
    public object? CollectSortInsertRebuild() =>
        InsertIntoABinarySearchTreeSolution.InsertByCollectSortRebuild(_insertionOrder, _newValue);

    [Benchmark]
    public object? BinarySearchTreeInsert() =>
        InsertIntoABinarySearchTreeSolution.InsertByBstInsert(_insertionOrder, _newValue);
}
