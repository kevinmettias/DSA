using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListCycle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListCycle;

// Harness only. Both strategies are LinkedListCycleSolution's - this file builds
// LeetCode's published examples as linked lists (pos is the 0-based index the
// tail's Next rejoins, or -1 for no cycle) and checks whether each strategy
// reports a cycle.
public sealed class LinkedListCycleTests
{
    public static TheoryData<CycleExample> Examples =>
        new()
        {
            { new CycleExample(Values: [3, 2, 0, -4], Pos: 1, Expected: true) },
            { new CycleExample(Values: [1, 2], Pos: 0, Expected: true) },
            { new CycleExample(Values: [1], Pos: -1, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByVisitedSet_LeetCodeExamples_ReturnsWhetherTheListCycles(CycleExample example)
    {
        var head = BuildList(example.Values, example.Pos);

        Assert.Equal(example.Expected, LinkedListCycleSolution.HasCycleByVisitedSet(head));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasCycleByFloydCycleDetection_LeetCodeExamples_ReturnsWhetherTheListCycles(CycleExample example)
    {
        var head = BuildList(example.Values, example.Pos);

        Assert.Equal(example.Expected, LinkedListCycleSolution.HasCycleByFloydCycleDetection(head));
    }

    // One LeetCode example: the list's values, the index the tail rejoins (-1 for no
    // cycle), and whether the list cycles. The `bool` is the expected answer rather than
    // a mode, so the row names it instead of leaving a bare `true` in a position to
    // decode - and `Pos` sits next to `Values`, which is the list it indexes.
    public readonly record struct CycleExample(int[] Values, int Pos, bool Expected);

    private static SinglyLinkedListNode<int>? BuildList(int[] values, int pos)
    {
        if (values.Length == 0)
        {
            return null;
        }

        var nodes = NewNodes(values);
        LinkInOrder(nodes);

        if (pos >= 0)
        {
            nodes[^1].Next = nodes[pos];
        }

        return nodes[0];
    }

    private static SinglyLinkedListNode<int>[] NewNodes(int[] values)
    {
        var nodes = new SinglyLinkedListNode<int>[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(values[i]);
        }

        return nodes;
    }

    private static void LinkInOrder(SinglyLinkedListNode<int>[] nodes)
    {
        for (var i = 0; i < nodes.Length - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
        }
    }
}
