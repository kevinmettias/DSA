using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PathSumWorkloads (ARCHITECTURE 17.7). LC 112 and LC 113's benchmarks rely on
// three claims the fixture's comment makes: the tree is complete, every value and the target stay
// inside LeetCode's [-1000, 1000], and exactly the last matchingPaths leaves in left-to-right order
// end a root-to-leaf path summing to the target. The last claim is what keeps a search that stops at
// its first match from finishing early, so it is checked against every leaf, not just the planted
// ones. The node count is LeetCode's 5,000-node cap, where the value bound is tightest.
public sealed partial class PathSumWorkloadsTests
{
    private const int NodeCount = 5_000;
    private const int MatchingPaths = 8;
    private const int Seed = 112; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    [Fact]
    public void Build_NodeCount_ReturnsACompleteTreeOfThatSize()
    {
        var levelOrder = LeetCodeWireFormat.FromBinaryTree(Build().Root);

        Assert.Equal(NodeCount, levelOrder.Length);
        Assert.DoesNotContain(null, levelOrder);
    }

    [Fact]
    public void Build_EveryValueAndTheTarget_StayWithinLeetCodesRange()
    {
        var (root, targetSum) = Build();

        Assert.All(LeetCodeWireFormat.FromBinaryTree(root).OfType<int>(), value => Assert.InRange(value, MinValue, MaxValue));
        Assert.InRange(targetSum, MinValue, MaxValue);
    }

    [Fact]
    public void Build_MatchingPaths_AreExactlyTheLastLeavesLeftToRight()
    {
        var (root, targetSum) = Build();
        var pathSums = LeafPathSumsLeftToRight(root);
        var matching = Enumerable.Range(0, pathSums.Count).Where(leaf => pathSums[leaf] == targetSum);

        Assert.Equal(Enumerable.Range(pathSums.Count - MatchingPaths, MatchingPaths), matching);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var first = Build();
        var second = Build();

        Assert.Equal(first.TargetSum, second.TargetSum);
        Assert.Equal(LeetCodeWireFormat.FromBinaryTree(first.Root), LeetCodeWireFormat.FromBinaryTree(second.Root));
    }

    private static (BinaryTreeNode<int> Root, int TargetSum) Build() =>
        PathSumWorkloads.Build(NodeCount, MatchingPaths, new Random(Seed));

    private static List<int> LeafPathSumsLeftToRight(BinaryTreeNode<int> root)
    {
        var sums = new List<int>();
        AppendLeafPathSums(root, 0, sums);

        return sums;
    }

    private static void AppendLeafPathSums(BinaryTreeNode<int>? node, int sumAbove, List<int> sums)
    {
        if (node is null)
        {
            return;
        }

        var sum = sumAbove + node.Value;

        if (node.Left is null && node.Right is null)
        {
            sums.Add(sum);
            return;
        }

        AppendLeafPathSums(node.Left, sum, sums);
        AppendLeafPathSums(node.Right, sum, sums);
    }
}
