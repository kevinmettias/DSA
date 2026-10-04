using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

// Expected values come from two places that share nothing with the tour: a hand derivation
// on a seven-node sample, and, on a seeded random tree, a climb up the parent array itself -
// u is below v exactly when climbing from u reaches v.
public sealed partial class PreOrderTourTests
{
    // 0 -> [1, 2], 1 -> [3, 4], 2 -> [5], 3 -> [6]: ParentArrayTree lists each node's children
    // in id order, so a pre-order taking the last child first visits 0 2 5 1 4 3 6.
    private static readonly int[] SampleParents = [-1, 0, 0, 1, 1, 2, 3];
    private static readonly int[] SamplePreOrder = [0, 2, 5, 1, 4, 3, 6];

    // A chain this long is past LeetCode's 5 * 10^4 and deep enough to overflow a recursive walk.
    private const int ChainLength = 100_000;

    private const int RandomTreeSize = 60;
    private const int RandomTreeSeed = 3841;

    [Fact]
    public void Build_SampleTree_TakesPositionsInPreOrderLastChildFirst()
    {
        var tour = BuildTour(SampleParents);

        Assert.Equal(SamplePreOrder, Enumerable.Range(0, tour.NodeCount).Select(tour.NodeAt));
    }

    [Fact]
    public void Build_NodeCountLargerThanTheTree_ThrowsArgumentException()
    {
        var nodes = ParentArrayTree.Build(SampleParents);

        Assert.Throws<ArgumentException>(() => PreOrderTour.Build(nodes[0], nodes.Length + 1));
    }

    // A recursive walk would need one frame per level here.
    [Fact]
    public void Build_ChainPastLeetCodesDepth_LaysEveryNodeOutWithoutRecursing()
    {
        var tour = PreOrderTour.Build(ParentArrayTree.Chain(ChainLength), ChainLength);

        Assert.Equal(ChainLength, tour.NodeCount);
        Assert.Equal((ChainLength / 2, ChainLength - 1), tour.SubtreeOf(ChainLength / 2));
    }

    [Fact]
    public void NodeCount_IsTheNumberOfNodesLaidOut()
    {
        Assert.Equal(SampleParents.Length, BuildTour(SampleParents).NodeCount);
    }

    [Fact]
    public void PositionOf_SampleTree_IsWhereEachNodeFallsInPreOrder()
    {
        var tour = BuildTour(SampleParents);

        // Read off SamplePreOrder: node 1 is fourth, node 3 sixth.
        int[] expected = [0, 3, 1, 5, 4, 2, 6];

        Assert.Equal(expected, Enumerable.Range(0, SampleParents.Length).Select(tour.PositionOf));
    }

    [Fact]
    public void NodeAt_UndoesPositionOf()
    {
        var tour = BuildTour(RandomParents());

        Assert.All(Enumerable.Range(0, tour.NodeCount), node => Assert.Equal(node, tour.NodeAt(tour.PositionOf(node))));
    }

    [Fact]
    public void ParentOf_MatchesTheParentArray_AndIsNegativeForTheRoot()
    {
        var parents = RandomParents();
        var tour = BuildTour(parents);

        Assert.True(tour.ParentOf(0) < 0);
        Assert.All(Enumerable.Range(1, parents.Length - 1), node => Assert.Equal(parents[node], tour.ParentOf(node)));
    }

    // Each range read off the sample's pre-order: node 1's subtree is 1 4 3 6, at positions 3..6.
    [Fact]
    public void SubtreeOf_SampleTree_IsEachNodesRunOfPositions()
    {
        var tour = BuildTour(SampleParents);
        (int, int)[] expected = [(0, 6), (3, 6), (1, 2), (5, 6), (4, 4), (2, 2), (6, 6)];

        Assert.Equal(expected, Enumerable.Range(0, SampleParents.Length).Select(tour.SubtreeOf));
    }

    [Fact]
    public void SubtreeOf_RandomTree_HoldsExactlyTheNodesWhoseClimbReachesIt()
    {
        var parents = RandomParents();
        var tour = BuildTour(parents);

        foreach (var root in Enumerable.Range(0, parents.Length))
        {
            var (first, last) = tour.SubtreeOf(root);
            var laidOut = Enumerable.Range(first, last - first + 1).Select(tour.NodeAt).Order();
            var below = Enumerable.Range(0, parents.Length).Where(node => IsReachedClimbingFrom(parents, node, root));

            Assert.Equal(below, laidOut);
        }
    }

    [Fact]
    public void SubtreeOf_SingleNode_IsJustItsOwnPosition()
    {
        var tour = BuildTour([-1]);

        Assert.Equal((0, 0), tour.SubtreeOf(0));
    }

    private static PreOrderTour BuildTour(int[] parents)
    {
        var nodes = ParentArrayTree.Build(parents);

        return PreOrderTour.Build(nodes[0], nodes.Length);
    }

    // Each node after the root hangs off a uniformly drawn earlier one, so the array is a tree
    // rooted at 0 with branching and depth both left to the draw.
    private static int[] RandomParents()
    {
        var random = new Random(RandomTreeSeed);
        var parents = new int[RandomTreeSize];
        parents[0] = -1;

        for (var node = 1; node < RandomTreeSize; node++)
        {
            parents[node] = random.Next(node);
        }

        return parents;
    }

    // Ends at the root, whose negative parent stops the climb.
    private static bool IsReachedClimbingFrom(int[] parents, int node, int ancestor)
    {
        for (var current = node; current >= 0; current = parents[current])
        {
            if (current == ancestor)
            {
                return true;
            }
        }

        return false;
    }
}
