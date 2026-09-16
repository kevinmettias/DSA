using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.ShortestPaths;

public sealed class ByPriorityOrderTests
{
    private static (TestNode Node, int Priority) Entry(string name, int priority) => (new TestNode(name), priority);

    [Fact]
    public void HasPriority_LowerPriority_Wins()
    {
        var candidate = Entry("A", 1);
        var incumbent = Entry("B", 5);
        var takesPriority = ByPriorityOrder<TestNode, int>.HasPriority(candidate, incumbent);

        Assert.True(takesPriority);
    }

    [Fact]
    public void HasPriority_HigherPriority_Loses()
    {
        var candidate = Entry("A", 5);
        var incumbent = Entry("B", 1);
        var takesPriority = ByPriorityOrder<TestNode, int>.HasPriority(candidate, incumbent);

        Assert.False(takesPriority);
    }

    [Fact]
    public void HasPriority_EqualPriorities_DoesNotDisplaceTheIncumbent()
    {
        // A strict comparison keeps the heap stable against equal keys.
        var candidate = Entry("A", 3);
        var incumbent = Entry("B", 3);
        var takesPriority = ByPriorityOrder<TestNode, int>.HasPriority(candidate, incumbent);

        Assert.False(takesPriority);
    }

    [Fact]
    public void HasPriority_IgnoresTheNodeAndComparesOnlyThePriority()
    {
        var shared = new TestNode("A");
        var candidate = (shared, 1);
        var incumbent = (shared, 2);
        var takesPriority = ByPriorityOrder<TestNode, int>.HasPriority(candidate, incumbent);

        Assert.True(takesPriority);
    }

    [Fact]
    public void HasPriority_WorksForAnyComparableWeight()
    {
        var candidate = (new TestNode("A"), 0.5);
        var incumbent = (new TestNode("B"), 1.5);
        var takesPriority = ByPriorityOrder<TestNode, double>.HasPriority(candidate, incumbent);

        Assert.True(takesPriority);
    }
}
