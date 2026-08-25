// Aliased to a non-colliding name, not a plain `using DSAExperimentation.Collections.DisjointSet;`
// with a bare `DisjointSet` reference: DisjointSet is this repo's first non-generic multi-file
// Collections type, so unlike `Heap<T,TOrder>` a bare `DisjointSet` reference has no type-argument
// list to disambiguate it from the enclosing namespace of the same name. Namespace-member lookup
// here finds this file's own namespace (`...Collections.DisjointSet`) as a nested member of
// `...Collections` before using-directives are ever consulted, so even a same-named `using` alias
// doesn't help - only a differently-named alias does.
using DisjointSetOperations = DSAExperimentation.Collections.DisjointSet.DisjointSet;

namespace DSAExperimentation.Tests.Collections.DisjointSet;

public sealed partial class DisjointSetTests
{
    [Fact]
    public void Find_UnrelatedIds_ReturnsDistinctRepresentatives()
    {
        var set = new DisjointSetOperations(3);

        Assert.NotEqual(set.Find(0), set.Find(1));
        Assert.NotEqual(set.Find(0), set.Find(2));
    }

    [Fact]
    public void Union_TwoIds_MakesThemConnected()
    {
        var set = new DisjointSetOperations(3);

        set.Union(0, 1);
        var unioned = set.Connected(0, 1);
        var untouched = set.Connected(0, 2);

        Assert.True(unioned);
        Assert.False(untouched);
    }

    [Fact]
    public void Union_ChainOfPairs_MergesAllIntoOneSet()
    {
        var set = new DisjointSetOperations(5);

        set.Union(0, 1);
        set.Union(1, 2);
        set.Union(2, 3);
        var chained = set.Connected(0, 3);
        var untouched = set.Connected(0, 4);

        Assert.True(chained);
        Assert.False(untouched);
    }

    [Fact]
    public void Union_AlreadyConnectedIds_IsANoOp()
    {
        var set = new DisjointSetOperations(3);
        set.Union(0, 1);
        var representativeBefore = set.Find(0);

        set.Union(1, 0);
        var stillConnected = set.Connected(0, 1);

        Assert.Equal(representativeBefore, set.Find(0));
        Assert.True(stillConnected);
    }

    [Fact]
    public void Find_AfterUnion_IsStableAcrossRepeatedCalls()
    {
        var set = new DisjointSetOperations(4);
        set.Union(0, 1);
        set.Union(2, 3);
        set.Union(1, 2);

        var representative = set.Find(0);

        Assert.Equal(representative, set.Find(1));
        Assert.Equal(representative, set.Find(2));
        Assert.Equal(representative, set.Find(3));
    }

    [Fact]
    public void Connected_SameId_IsAlwaysTrue()
    {
        var set = new DisjointSetOperations(3);

        var connected = set.Connected(1, 1);

        Assert.True(connected);
    }

    [Fact]
    public void Count_ReflectsConstructorSize()
    {
        var set = new DisjointSetOperations(7);

        Assert.Equal(7, set.Count);
    }

    [Fact]
    public void Union_ManyElements_ProducesExactlyTheExpectedPartition()
    {
        var set = new DisjointSetOperations(10);

        for (var id = 0; id < 5; id++)
        {
            set.Union(0, id);
        }

        for (var id = 5; id < 10; id++)
        {
            set.Union(5, id);
        }

        for (var id = 0; id < 5; id++)
        {
            var withinFirstGroup = set.Connected(0, id);
            var acrossGroups = set.Connected(0, id + 5);

            Assert.True(withinFirstGroup);
            Assert.False(acrossGroups);
        }
    }
}
