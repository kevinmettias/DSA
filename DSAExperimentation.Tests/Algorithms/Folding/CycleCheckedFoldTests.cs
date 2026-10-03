using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// The graph tier's policy memoizes like the DAG tier's and also refuses to open a node already on
// the recursion path, which is how it detects a cycle.
public sealed partial class CycleCheckedFoldTests
{
    private const string Result = "folded";

    private static readonly TestNode Node = new("A");

    [Fact]
    public void Aborted_BeforeAnyCycle_IsFalse() => Assert.False(new CycleCheckedFold<TestNode, string>().Aborted);

    [Fact]
    public void TryOpen_FirstTime_Opens() => Assert.True(new CycleCheckedFold<TestNode, string>().TryOpen(Node));

    [Fact]
    public void TryOpen_NodeStillOnThePath_RefusesAndAborts()
    {
        var memo = new CycleCheckedFold<TestNode, string>();
        memo.TryOpen(Node);

        Assert.False(memo.TryOpen(Node));
        Assert.True(memo.Aborted);
    }

    // Closing takes the node off the path and memoizes it: a second arrival is a recall, not a
    // cycle - the shared-descendant case CheckedFold must not mistake for one.
    [Fact]
    public void Close_TakesTheNodeOffThePathAndRemembersItsResult()
    {
        var memo = new CycleCheckedFold<TestNode, string>();
        memo.TryOpen(Node);

        memo.Close(Node, Result);

        Assert.True(memo.TryRecall(Node, out var recalled));
        Assert.Equal(Result, recalled);
        Assert.True(memo.TryOpen(Node));
        Assert.False(memo.Aborted);
    }

    [Fact]
    public void TryRecall_BeforeClose_Misses() => Assert.False(new CycleCheckedFold<TestNode, string>().TryRecall(Node, out _));

    // FoldRecursion threads the policy by value, so a cycle found through one copy must abort
    // every copy - the reason the flag lives in a shared ledger rather than in the struct.
    [Fact]
    public void TryOpen_CycleFoundThroughACopy_AbortsTheOriginal()
    {
        var memo = new CycleCheckedFold<TestNode, string>();
        var copy = memo;
        copy.TryOpen(Node);

        copy.TryOpen(Node);

        Assert.True(memo.Aborted);
    }
}
