using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The walk's bookkeeping on its own: two suffixes taken in turn share the shallowest depth passed
// between them, never more than the first one's own depth.
public sealed partial class SortedLeafWalkTests
{
    private const int FirstSuffix = 5;
    private const int SecondSuffix = 7;
    private const int FirstDepth = 4;
    private const int SecondDepth = 6;
    private const int ShallowerDepth = 2;
    private const int DeeperDepth = 5;

    [Fact]
    public void Take_AfterPassingAShallowerNode_GivesThePairThatDepth()
    {
        var walk = new SortedLeafWalk(2);
        walk.Take(FirstSuffix, FirstDepth);
        walk.Pass(ShallowerDepth);
        walk.Take(SecondSuffix, SecondDepth);

        Assert.Equal(
            [[FirstSuffix, SecondSuffix], [ShallowerDepth]],
            new[] { walk.Suffixes, walk.NeighbourPrefixes });
    }

    // The second suffix lies below the first's node, so the first is a prefix of it.
    [Fact]
    public void Pass_OnlyNodesBelowTheLastSuffix_LeavesThePairItsDepth()
    {
        var walk = new SortedLeafWalk(2);
        walk.Take(FirstSuffix, FirstDepth);
        walk.Pass(DeeperDepth);
        walk.Take(SecondSuffix, SecondDepth);

        Assert.Equal([FirstDepth], walk.NeighbourPrefixes);
    }
}
