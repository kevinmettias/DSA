using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// The tree tier's policy answers every question with the same constant: nothing is recalled,
// everything opens, and closing a node remembers nothing.
public sealed partial class UnmemoizedFoldTests
{
    private const string AnyResult = "result";

    private static readonly TestNode Node = new("A");

    [Fact]
    public void Aborted_IsAlwaysFalse() => Assert.False(default(UnmemoizedFold<TestNode, string>).Aborted);

    [Fact]
    public void TryRecall_AlwaysMisses() => Assert.False(default(UnmemoizedFold<TestNode, string>).TryRecall(Node, out _));

    // Opening the same node twice is not a cycle here: the tree tier's topology already rules
    // cycles out, so there is nothing to check.
    [Fact]
    public void TryOpen_SameNodeTwice_OpensBothTimes()
    {
        var memo = default(UnmemoizedFold<TestNode, string>);

        Assert.True(memo.TryOpen(Node));
        Assert.True(memo.TryOpen(Node));
    }

    [Fact]
    public void Close_RemembersNothing()
    {
        var memo = default(UnmemoizedFold<TestNode, string>);

        memo.Close(Node, AnyResult);

        Assert.False(memo.TryRecall(Node, out _));
    }
}
