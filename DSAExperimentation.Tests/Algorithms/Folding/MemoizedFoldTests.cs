using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// The DAG tier's policy remembers each closed node's result and checks nothing else.
public sealed partial class MemoizedFoldTests
{
    private const string Result = "folded";

    private static readonly TestNode Node = new("A");

    [Fact]
    public void Aborted_IsAlwaysFalse() => Assert.False(new MemoizedFold<TestNode, string>([]).Aborted);

    [Fact]
    public void TryRecall_BeforeClose_Misses() => Assert.False(new MemoizedFold<TestNode, string>([]).TryRecall(Node, out _));

    [Fact]
    public void Close_ThenTryRecall_ReturnsTheClosedResult()
    {
        var memo = new MemoizedFold<TestNode, string>([]);

        memo.Close(Node, Result);

        Assert.True(memo.TryRecall(Node, out var recalled));
        Assert.Equal(Result, recalled);
    }

    // IDagTopology rules cycles out, so opening a node already open is not checked.
    [Fact]
    public void TryOpen_SameNodeTwice_OpensBothTimes()
    {
        var memo = new MemoizedFold<TestNode, string>([]);

        Assert.True(memo.TryOpen(Node));
        Assert.True(memo.TryOpen(Node));
    }

    // FoldRecursion threads the policy by value, so every copy must read and write one memo.
    [Fact]
    public void Close_OnACopy_IsRecalledThroughTheOriginal()
    {
        var memo = new MemoizedFold<TestNode, string>([]);
        var copy = memo;

        copy.Close(Node, Result);

        Assert.True(memo.TryRecall(Node, out _));
    }
}
