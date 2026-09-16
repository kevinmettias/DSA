using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

public sealed partial class TrackedVisitGuardTests
{
    [Fact]
    public void ShouldVisit_FirstSighting_ReturnsTrue()
    {
        var guard = new TrackedVisitGuard<TestNode>([]);

        Assert.True(guard.ShouldVisit(new TestNode("A")));
    }

    [Fact]
    public void ShouldVisit_SameNodeAgain_ReturnsFalse()
    {
        var guard = new TrackedVisitGuard<TestNode>([]);
        var node = new TestNode("A");

        Assert.True(guard.ShouldVisit(node));
        Assert.False(guard.ShouldVisit(node));
    }

    [Fact]
    public void ShouldVisit_DistinguishesByIdentityNotByName()
    {
        var guard = new TrackedVisitGuard<TestNode>([]);

        Assert.True(guard.ShouldVisit(new TestNode("A")));
        Assert.True(guard.ShouldVisit(new TestNode("A")));
    }

    [Fact]
    public void ShouldVisit_HonoursNodesAlreadyInTheSuppliedSet()
    {
        var seen = new TestNode("A");
        var guard = new TrackedVisitGuard<TestNode>([seen]);

        Assert.False(guard.ShouldVisit(seen));
    }

    [Fact]
    public void ShouldVisit_SharesOneSetAcrossEveryReferenceToTheGuard()
    {
        // The guard is a reference type precisely so the visited set is shared
        // rather than silently copied - that sharing is what stops a walk looping.
        var visited = new HashSet<TestNode>();
        var guard = new TrackedVisitGuard<TestNode>(visited);
        var alias = guard;
        var node = new TestNode("A");

        Assert.True(guard.ShouldVisit(node));
        Assert.False(alias.ShouldVisit(node));
        Assert.Contains(node, visited);
    }
}
