using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RangeSumOfBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeSumOfBSTSolution's, the same methods
// RangeSumOfBSTSolutionTests proves correct. Low/High are deliberately narrow and near the
// low end of the value domain, so pruning discards most of a large tree instead of
// merely skipping a few leaves. Tree construction is charged to [GlobalSetup]. The
// node values are 1..Length and Low is 1, as LC 938's values and bounds start at 1.
public class RangeSumOfBSTBenchmarks
{
    private const int Low = 1;
    private const int High = 20;
    private const int RandomSeed = 938;

    private BinaryTreeNode<int>? _root;

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(Low, Length).ToArray();
        var random = new Random(RandomSeed);

        // Fisher-Yates shuffle before insertion, so BinarySearchTree.Insert builds
        // an expected-O(log n)-height tree instead of the O(n)-height degenerate
        // chain ascending input would force.
        for (var i = values.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
        }

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        _root = tree.Root;
    }

    [Benchmark(Baseline = true)]
    public int FullTreeScan() => RangeSumOfBSTSolution.RangeSumByFullScan(_root, Low, High);

    [Benchmark]
    public int SearchTreePrunedWalk() =>
        RangeSumOfBSTSolution.RangeSumBySearchTreePruning(_root, Low, High);
}
