using DSAExperimentation.Buffers;

namespace DSAExperimentation.Tests.Buffers;

public sealed partial class ContiguousGroupBufferTests
{
    [Fact]
    public void Add_ConsecutiveItemsWithSameKey_GroupsTogetherOnFlush()
    {
        var groups = AddTwoItemsThenFlush(("a1", "A"), ("a2", "A"));

        Assert.Single(groups);
        Assert.Equal("A", groups[0].Key);
        Assert.Equal(new[] { "a1", "a2" }, groups[0].Items);
    }

    [Fact]
    public void Add_KeyChange_FlushesPreviousGroupBeforeStartingNext()
    {
        var groups = AddTwoItemsThenFlush(("a1", "A"), ("b1", "B"));

        Assert.Equal(2, groups.Count);
        Assert.Equal("A", groups[0].Key);
        Assert.Equal(new[] { "a1" }, groups[0].Items);
        Assert.Equal("B", groups[1].Key);
        Assert.Equal(new[] { "b1" }, groups[1].Items);
    }

    private static List<(IReadOnlyList<string> Items, string Key)> AddTwoItemsThenFlush(
        (string Item, string Key) first, (string Item, string Key) second)
    {
        var buffer = new ContiguousGroupBuffer<string, string>();
        var groups = new List<(IReadOnlyList<string> Items, string Key)>();

        buffer.Add(first.Item, first.Key, (items, key) => groups.Add((items, key)));
        buffer.Add(second.Item, second.Key, (items, key) => groups.Add((items, key)));
        buffer.Flush((items, key) => groups.Add((items, key)));

        return groups;
    }

    [Fact]
    public void Flush_WithNoItemsAdded_DoesNotInvokeCallback()
    {
        var buffer = new ContiguousGroupBuffer<string, string>();
        var invoked = false;

        buffer.Flush((_, _) => invoked = true);

        Assert.False(invoked);
    }

    [Fact]
    public void Reset_ClearsPendingGroupSoSubsequentFlushDoesNotInvokeCallback()
    {
        var buffer = new ContiguousGroupBuffer<string, string>();
        var invoked = false;

        buffer.Add("a1", "A", (_, _) => { });
        buffer.Reset();
        buffer.Flush((_, _) => invoked = true);

        Assert.False(invoked);
    }
}
