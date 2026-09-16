using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for HouseRobberIIIWorkloads (ARCHITECTURE 17.7). The reading depends on LC 337's
// tree being full and genuinely data-dependent at every node, so the Rob/NotRobbed choice is not
// settled by the same branch throughout.
public sealed partial class HouseRobberIIIWorkloadsTests
{
    private const int Depth = 6;
    private const int Seed = 337; // LC problem number
    private const int MinNodeValue = 1;
    private const int MaxNodeValueExclusive = 100;

    // Filling every level is what makes it a full tree, and a binary tree of this height can only
    // hold that many nodes if every level is filled - the two facts together pin the shape.
    [Fact]
    public void RandomFullTree_Depth_ReturnsATreeWithEveryLevelFilled()
    {
        var root = HouseRobberIIIWorkloads.RandomFullTree(Depth, Seed);

        Assert.Equal((1 << (Depth + 1)) - 1, CountNodes(root));
        Assert.Equal(Depth, Height(root));
    }

    [Fact]
    public void RandomFullTree_EveryValue_StaysWithinTheDocumentedBand() =>
        Assert.All(
            InOrderValues(HouseRobberIIIWorkloads.RandomFullTree(Depth, Seed)),
            value => Assert.InRange(value, MinNodeValue, MaxNodeValueExclusive - 1));

    [Fact]
    public void RandomFullTree_SameSeed_ReturnsTheSameTree() =>
        Assert.Equal(
            InOrderValues(HouseRobberIIIWorkloads.RandomFullTree(Depth, Seed)),
            InOrderValues(HouseRobberIIIWorkloads.RandomFullTree(Depth, Seed)));

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? -1 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private static List<int> InOrderValues(BinaryTreeNode<int>? node)
    {
        var values = new List<int>();

        if (node is null)
        {
            return values;
        }

        values.AddRange(InOrderValues(node.Left));
        values.Add(node.Value);
        values.AddRange(InOrderValues(node.Right));

        return values;
    }
}
