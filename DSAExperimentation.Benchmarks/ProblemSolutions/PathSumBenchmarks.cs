using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class PathSumBenchmarks
{
    // LeetCode 112's own example tree: the root-to-leaf path RootValue -> LeftValue
    // -> LeftLeftValue -> LeftLeftRightValue (5 -> 4 -> 11 -> 2) sums to TargetSum,
    // the only accepting path among the tree's four root-to-leaf paths.
    private const int TargetSum = 22;
    private const int RootValue = 5;
    private const int LeftValue = 4;
    private const int LeftLeftValue = 11;
    private const int LeftLeftLeftValue = 7;
    private const int LeftLeftRightValue = 2;
    private const int RightValue = 8;
    private const int RightLeftValue = 13;
    private const int RightRightValue = 4;
    private const int RightRightRightValue = 1;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = Tree();

    [Benchmark(Baseline = true)]
    public bool RecursivePathSum() => Has(_root, TargetSum);

    [Benchmark]
    public bool AllRootToLeafPathsSum() =>
        AllRootToLeafPaths.Find<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(_root)
            .Any(p => p.Sum(n => n.Value) == TargetSum);

    private static bool Has(BinaryTreeNode<int>? node, int target) =>
        node is not null && (node.Left is null && node.Right is null
            ? node.Value == target
            : Has(node.Left, target - node.Value) || Has(node.Right, target - node.Value));

    private static BinaryTreeNode<int> Tree() => new(RootValue)
    {
        Left = new(LeftValue)
        {
            Left = new(LeftLeftValue)
            {
                Left = new(LeftLeftLeftValue),
                Right = new(LeftLeftRightValue)
            }
        },
        Right = new(RightValue)
        {
            Left = new(RightLeftValue),
            Right = new(RightRightValue)
            {
                Right = new(RightRightRightValue)
            }
        }
    };
}
