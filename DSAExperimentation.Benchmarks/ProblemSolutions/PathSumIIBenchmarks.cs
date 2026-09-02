using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PathSumIIBenchmarks
{
    private const int TargetPathSum = 22;

    private const int RootValue = 5;
    private const int LeftValue = 4;
    private const int LeftLeftValue = 11;
    private const int LeftLeftLeftValue = 7;
    private const int LeftLeftRightValue = 2;
    private const int RightValue = 8;
    private const int RightLeftValue = 13;
    private const int RightRightValue = 4;
    private const int RightRightLeftValue = 5;
    private const int RightRightRightValue = 1;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = new BinaryTreeNode<int>(RootValue)
    {
        Left = new BinaryTreeNode<int>(LeftValue)
        {
            Left = new BinaryTreeNode<int>(LeftLeftValue)
            {
                Left = new BinaryTreeNode<int>(LeftLeftLeftValue),
                Right = new BinaryTreeNode<int>(LeftLeftRightValue)
            }
        },
        Right = new BinaryTreeNode<int>(RightValue)
        {
            Left = new BinaryTreeNode<int>(RightLeftValue),
            Right = new BinaryTreeNode<int>(RightRightValue)
            {
                Left = new BinaryTreeNode<int>(RightRightLeftValue),
                Right = new BinaryTreeNode<int>(RightRightRightValue)
            }
        }
    };

    [Benchmark(Baseline = true)]
    public int RecursiveCollect()
    {
        var count = 0;

        void Search(BinaryTreeNode<int>? node, int sum)
        {
            if (node is null)
            {
                return;
            }

            sum += node.Value;

            if (node.Left is null && node.Right is null && sum == TargetPathSum)
            {
                count++;
            }

            Search(node.Left, sum);
            Search(node.Right, sum);
        }

        Search(_root, 0);
        return count;
    }

    [Benchmark]
    public int AllRootToLeafPathsCollect() =>
        AllRootToLeafPaths.Find<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(_root)
            .Count(p => p.Sum(n => n.Value) == TargetPathSum);
}
