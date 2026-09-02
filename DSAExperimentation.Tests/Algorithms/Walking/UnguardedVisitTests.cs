using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

public sealed class UnguardedVisitTests
{
    [Fact]
    public void ShouldVisit_AlwaysReturnsTrue()
    {
        var guard = new UnguardedVisit<TestNode>();

        Assert.True(guard.ShouldVisit(new TestNode("A")));
    }

    [Fact]
    public void ShouldVisit_SameNodeTwice_StillReturnsTrue()
    {
        // The tree-only guard keeps no state: ITreeTopology already promises unique
        // ancestry, so a repeat can only mean the caller broke that promise.
        var guard = new UnguardedVisit<TestNode>();
        var node = new TestNode("A");

        Assert.True(guard.ShouldVisit(node));
        Assert.True(guard.ShouldVisit(node));
    }

    [Fact]
    public void ShouldVisit_IsUnaffectedByCopyingTheGuard()
    {
        var guard = new UnguardedVisit<TestNode>();
        var copy = guard;
        var node = new TestNode("A");

        Assert.True(guard.ShouldVisit(node));
        Assert.True(copy.ShouldVisit(node));
    }
}
