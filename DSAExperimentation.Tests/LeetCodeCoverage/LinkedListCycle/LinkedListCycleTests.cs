using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListCycle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListCycle;

// Harness only. Both strategies are LinkedListCycleSolution's - this file builds
// LeetCode's published examples as linked lists (pos is the 0-based index the
// tail's Next rejoins, or -1 for no cycle) and checks whether each strategy
// reports a cycle.
public sealed class LinkedListCycleTests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [3, 2, 0, -4], 1, true },
            { [1, 2], 0, true },
            { [1], -1, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByVisitedSet_LeetCodeExamples_ReturnsWhetherTheListCycles(
        int[] values, int pos, bool expected) =>
        Assert.Equal(expected, LinkedListCycleSolution.HasCycleByVisitedSet(BuildList(values, pos)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByFloydCycleDetection_LeetCodeExamples_ReturnsWhetherTheListCycles(
        int[] values, int pos, bool expected) =>
        Assert.Equal(expected, LinkedListCycleSolution.HasCycleByFloydCycleDetection(BuildList(values, pos)));

    private static SinglyLinkedListNode<int>? BuildList(int[] values, int pos)
    {
        if (values.Length == 0)
        {
            return null;
        }

        var nodes = new SinglyLinkedListNode<int>[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(values[i]);
        }

        for (var i = 0; i < values.Length - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
        }

        if (pos >= 0)
        {
            nodes[^1].Next = nodes[pos];
        }

        return nodes[0];
    }
}
