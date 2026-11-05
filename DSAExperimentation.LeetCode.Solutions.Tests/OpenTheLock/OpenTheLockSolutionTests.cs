using DSAExperimentation.LeetCode.Solutions.OpenTheLock;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OpenTheLock;

// Harness only. The lock is Domain/Devices/Locks' FourDigitLock and both search
// strategies are OpenTheLockSolution's - this file just pins them to LeetCode's
// published examples, including the two unopenable cases the graph strategy has to
// answer without a target node existing at all.
public sealed partial class OpenTheLockSolutionTests
{
    // The deadends are this problem's filter: the graph keeps every other combination.
    [Fact]
    public void BuildLockGraph_KeepsEveryCombinationButTheDeadends()
    {
        var graph = OpenTheLockSolution.BuildLockGraph(["8888", "0009"]);

        Assert.Equal(9_998, graph.Count);
        Assert.False(graph.TryGetNode("8888", out _));
    }

    [Fact]
    public void BuildLockGraph_JoinsNoCombinationToADeadend()
    {
        var graph = OpenTheLockSolution.BuildLockGraph(["0009"]);

        Assert.True(graph.TryGetNode("0000", out var start));
        Assert.Equal(7, start.Successors.Count);
        Assert.DoesNotContain(start.Successors, successor => successor.State == "0009");
    }
}
