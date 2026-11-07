using System.Numerics;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

// The two implicit trees, held to what a Fenwick tree relies on them for. Node j covers the
// elements (QueryTreeParent(j), j]. Climbing the update tree from a position must reach exactly the
// nodes whose ranges hold it, and climbing the query tree from a position must read ranges that
// tile everything up to it, each element once. Every walk here also checks that it climbs, so a
// step that went the wrong way fails rather than looping.
public sealed partial class FenwickIndexTests
{
    // Not a power of two, so the update tree's walks leave the array at several heights.
    private const int ArrayLength = 37;

    public static TheoryData<int, int> PositionToUpdateTreeParent => new()
    {
        { 1, 2 },
        { 2, 4 },
        { 3, 4 },
        { 5, 6 },
        { 6, 8 },
        { 7, 8 },
        { 12, 16 },
        { 16, 32 },
    };

    public static TheoryData<int, int> PositionToQueryTreeParent => new()
    {
        { 1, 0 },
        { 2, 0 },
        { 3, 2 },
        { 5, 4 },
        { 6, 4 },
        { 7, 6 },
        { 12, 8 },
        { 16, 0 },
    };

    public static TheoryData<int> Positions => [.. Enumerable.Range(1, ArrayLength)];

    [Theory]
    [MemberData(nameof(PositionToUpdateTreeParent))]
    public void UpdateTreeParent_OfPosition_AddsItsLowestSetBit(int position, int expected) =>
        Assert.Equal(expected, FenwickIndex.UpdateTreeParent(position));

    [Theory]
    [MemberData(nameof(PositionToQueryTreeParent))]
    public void QueryTreeParent_OfPosition_ClearsItsLowestSetBit(int position, int expected) =>
        Assert.Equal(expected, FenwickIndex.QueryTreeParent(position));

    [Theory]
    [MemberData(nameof(Positions))]
    public void UpdateTreeParent_WalkFromAPosition_ReachesExactlyTheNodesCoveringIt(int position)
    {
        var covering = Enumerable.Range(1, ArrayLength)
            .Where(node => FenwickIndex.QueryTreeParent(node) < position && position <= node);

        Assert.Equal(covering, UpdateWalk(position));
    }

    // One node per set bit of the position, which is the O(log n) a prefix query promises.
    [Theory]
    [MemberData(nameof(Positions))]
    public void QueryTreeParent_WalkFromAPosition_ReadsEachElementUpToItOnce(int position)
    {
        var nodes = QueryWalk(position);
        var read = nodes
            .SelectMany(node => Enumerable.Range(
                FenwickIndex.QueryTreeParent(node) + 1, node - FenwickIndex.QueryTreeParent(node)))
            .Order();

        Assert.Equal(Enumerable.Range(1, position), read);
        Assert.Equal(BitOperations.PopCount((uint)position), nodes.Count);
    }

    // The nodes an update at a position reaches, in the order it reaches them.
    private static List<int> UpdateWalk(int position)
    {
        var nodes = new List<int>();

        for (var node = position; node <= ArrayLength;)
        {
            nodes.Add(node);
            var parent = FenwickIndex.UpdateTreeParent(node);
            Assert.True(parent > node, $"the update tree climbs from {node}, not to {parent}");
            node = parent;
        }

        return nodes;
    }

    // The nodes a prefix query through a position reads, in the order it reads them.
    private static List<int> QueryWalk(int position)
    {
        var nodes = new List<int>();

        for (var node = position; node > 0;)
        {
            nodes.Add(node);
            var parent = FenwickIndex.QueryTreeParent(node);
            Assert.True(parent < node, $"the query tree descends from {node}, not to {parent}");
            node = parent;
        }

        return nodes;
    }
}
