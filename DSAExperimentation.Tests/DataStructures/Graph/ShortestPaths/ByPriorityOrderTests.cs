using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.ShortestPaths;

public sealed class ByPriorityOrderTests
{
    private static (TestNode Node, int Priority) Entry(string name, int priority) => (new TestNode(name), priority);

    [Fact]
    public void HasPriority_LowerPriority_Wins()
    {
        Assert.True(ByPriorityOrder<TestNode, int>.HasPriority(Entry("A", 1), Entry("B", 5)));
    }

    [Fact]
    public void HasPriority_HigherPriority_Loses()
    {
        Assert.False(ByPriorityOrder<TestNode, int>.HasPriority(Entry("A", 5), Entry("B", 1)));
    }

    [Fact]
    public void HasPriority_EqualPriorities_DoesNotDisplaceTheIncumbent()
    {
        // A strict comparison keeps the heap stable against equal keys.
        Assert.False(ByPriorityOrder<TestNode, int>.HasPriority(Entry("A", 3), Entry("B", 3)));
    }

    [Fact]
    public void HasPriority_IgnoresTheNodeAndComparesOnlyThePriority()
    {
        var shared = new TestNode("A");

        Assert.True(ByPriorityOrder<TestNode, int>.HasPriority((shared, 1), (shared, 2)));
    }

    [Fact]
    public void HasPriority_WorksForAnyComparableWeight()
    {
        Assert.True(ByPriorityOrder<TestNode, double>.HasPriority((new TestNode("A"), 0.5), (new TestNode("B"), 1.5)));
    }
}
