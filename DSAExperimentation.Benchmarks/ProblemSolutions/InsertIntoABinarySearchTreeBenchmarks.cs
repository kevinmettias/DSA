using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InsertIntoABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only. Both arms are InsertIntoABinarySearchTreeSolution's, the same
// methods InsertIntoABinarySearchTreeTests proves correct. Each arm builds its
// own tree from the same shuffled insertion order every call (the original
// convention, preserved here), so the comparison stays "full construction plus
// one insert" for both strategies rather than only the incremental insert cost.
[MemoryDiagnoser]
public class InsertIntoABinarySearchTreeBenchmarks
{
    private const int ValueStride = 2;

    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _insertionOrder = null!;
    private int _newValue;

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

    // Projects to the resulting root's value (BinaryTreeNode<int> itself is
    // internal, and a [Benchmark] method must be public) purely so
    // BenchmarkDotNet has a public return type to consume - the call being
    // measured is still the one-line solution call.
    [Benchmark(Baseline = true)]
    public int? CollectSortInsertRebuild() =>
        InsertIntoABinarySearchTreeSolution.InsertByCollectSortRebuild(_insertionOrder, _newValue)?.Value;

    [Benchmark]
    public int? BinarySearchTreeInsert() =>
        InsertIntoABinarySearchTreeSolution.InsertByBstInsert(_insertionOrder, _newValue)?.Value;
}
