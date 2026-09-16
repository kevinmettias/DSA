using DSAExperimentation.LeetCode.OpenTheLock;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OpenTheLock;

// Harness only. The lock graph itself is Domain.Locks.LockGraph and both search
// strategies are OpenTheLockSolution's - this file just pins them to LeetCode's
// published examples, including the two unopenable cases the graph strategy has to
// answer without a target node existing at all.
public sealed class OpenTheLockTests
{
    public static TheoryData<string[], string, int> Examples =>
        new()
        {
            { ["0201", "0101", "0102", "1212", "2002"], "0202", 6 },
            { ["8888"], "0009", 1 },
            { ["8887", "8889", "8878", "8898", "8788", "8988", "7888", "9888"], "8888", -1 },
            { ["0000"], "8888", -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTurnsByMutationQueue_LeetCodeExamples_ReturnsShortestUnblockedTurnCount(
        string[] deadends, string target, int expected)
    {
        var actual = OpenTheLockSolution.MinTurnsByMutationQueue(deadends, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTurnsByReduceGraph_LeetCodeExamples_ReturnsShortestUnblockedTurnCount(
        string[] deadends, string target, int expected)
    {
        var actual = OpenTheLockSolution.MinTurnsByReduceGraph(deadends, target);

        Assert.Equal(expected, actual);
    }
}
