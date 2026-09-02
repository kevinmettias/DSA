using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class MinimumDepthOfBinaryTreeBenchmarks
{
    private const int LeftChildValue = 2;
    private const int RightChildValue = 3;
    private const int RightGrandchildValue = 4;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = new(1) { Left = new(LeftChildValue), Right = new(RightChildValue) { Right = new(RightGrandchildValue) } };

    [Benchmark(Baseline = true)]
    public int RecursiveMinDepth() => MinDepth(_root);

    [Benchmark]
    public int BinaryTreeNodeMinDepth() => MinDepth(_root);

    private static int MinDepth(BinaryTreeNode<int>? n)
    {
        if (n is null)
        {
            return 0;
        }

        if (n.Left is null)
        {
            return 1 + MinDepth(n.Right);
        }

        if (n.Right is null)
        {
            return 1 + MinDepth(n.Left);
        }

        return 1 + Math.Min(MinDepth(n.Left), MinDepth(n.Right));
    }
}
