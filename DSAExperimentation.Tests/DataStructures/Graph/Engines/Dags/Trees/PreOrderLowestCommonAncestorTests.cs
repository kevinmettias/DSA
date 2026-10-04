using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

// Expected ancestors come from a hand derivation on a seven-node sample and, on a seeded random
// tree, from climbing the parent array itself: the first of second's ancestors (itself
// included) that is also one of first's.
public sealed partial class PreOrderLowestCommonAncestorTests
{
    // 0 -> [1, 2], 1 -> [3, 4], 2 -> [5], 3 -> [6].
    private static readonly int[] SampleParents = [-1, 0, 0, 1, 1, 2, 3];

    // Past LeetCode's 5 * 10^4, and deep enough to overflow a recursive search.
    private const int ChainLength = 100_000;

    private const int RandomTreeSize = 40;
    private const int RandomTreeSeed = 3515;

    [Fact]
    public void Constructor_SingleNodeTour_AnswersItsOnlyNode()
    {
        var ancestors = BuildAncestors([-1]);

        Assert.Equal(0, ancestors.Find(0, 0));
    }

    [Fact]
    public void Find_SameNodeTwice_ReturnsThatNode()
    {
        var ancestors = BuildAncestors(SampleParents);

        Assert.Equal(4, ancestors.Find(4, 4));
    }

    // 6 sits below 3 below 1, and 4 below 1: their paths up meet at 1.
    [Fact]
    public void Find_NodesInSiblingSubtrees_ReturnsWhereTheirPathsMeet()
    {
        var ancestors = BuildAncestors(SampleParents);

        Assert.Equal(1, ancestors.Find(6, 4));
        Assert.Equal(1, ancestors.Find(4, 6));
    }

    // 6 is under 1 and 5 under 2, so only the root holds both.
    [Fact]
    public void Find_NodesUnderDifferentChildrenOfTheRoot_ReturnsTheRoot()
    {
        var ancestors = BuildAncestors(SampleParents);

        Assert.Equal(0, ancestors.Find(6, 5));
    }

    // 1 is on 6's path up, in either argument order.
    [Fact]
    public void Find_OneNodeIsTheOthersAncestor_ReturnsTheAncestor()
    {
        var ancestors = BuildAncestors(SampleParents);

        Assert.Equal(1, ancestors.Find(1, 6));
        Assert.Equal(1, ancestors.Find(6, 1));
    }

    [Fact]
    public void Find_EveryPairOfARandomTree_MatchesAClimbUpTheParentArray()
    {
        var parents = RandomParents();
        var ancestors = BuildAncestors(parents);
        var pairs = Enumerable.Range(0, parents.Length)
            .SelectMany(first => Enumerable.Range(0, parents.Length), (first, second) => (first, second));

        Assert.All(pairs, pair => Assert.Equal(
            AncestorByClimbing(parents, pair.first, pair.second),
            ancestors.Find(pair.first, pair.second)));
    }

    // The two ends of a chain meet at the shallower one, however deep the other sits.
    [Fact]
    public void Find_ChainPastLeetCodesDepth_ReturnsTheShallowerNode()
    {
        var tour = PreOrderTour.Build(ParentArrayTree.Chain(ChainLength), ChainLength);
        var ancestors = new PreOrderLowestCommonAncestor(tour);

        Assert.Equal(ChainLength / 2, ancestors.Find(ChainLength - 1, ChainLength / 2));
    }

    private static PreOrderLowestCommonAncestor BuildAncestors(int[] parents)
    {
        var nodes = ParentArrayTree.Build(parents);

        return new PreOrderLowestCommonAncestor(PreOrderTour.Build(nodes[0], nodes.Length));
    }

    // Each node after the root hangs off a uniformly drawn earlier one.
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

    // Every climb ends at the root, whose negative parent stops it, and the root is an ancestor
    // of everything, so the second climb always finds a match.
    private static int AncestorByClimbing(int[] parents, int first, int second)
    {
        var firstsAncestors = new HashSet<int>();

        for (var node = first; node >= 0; node = parents[node])
        {
            firstsAncestors.Add(node);
        }

        var meeting = second;

        while (!firstsAncestors.Contains(meeting))
        {
            meeting = parents[meeting];
        }

        return meeting;
    }
}
