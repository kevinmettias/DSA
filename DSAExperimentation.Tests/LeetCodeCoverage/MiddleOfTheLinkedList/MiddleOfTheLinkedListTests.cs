using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MiddleOfTheLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MiddleOfTheLinkedList;

// Harness only. Both strategies are MiddleOfTheLinkedListSolution's - this file
// states LeetCode's examples once as the list's values plus the value of the node
// the answer must land on, and asserts each strategy against them.
public sealed partial class MiddleOfTheLinkedListTests
{
    private const int MidpointDivisor = 2;

    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 3 },
            { [1, 2, 3, 4, 5, 6], 4 },
            { [1], 1 },
            { [1, 2], 2 },
            { [7, 7, 7, 8, 9, 10, 11], 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeBySlowFastTwoPointer_LeetCodeExamples_ReturnsMiddleNode(int[] values, int expected) =>
        Assert.Equal(
            expected,
            MiddleValue(MiddleOfTheLinkedListSolution.MiddleNodeBySlowFastTwoPointer(BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeByCountThenWalk_LeetCodeExamples_ReturnsMiddleNode(int[] values, int expected) =>
        Assert.Equal(
            expected,
            MiddleValue(MiddleOfTheLinkedListSolution.MiddleNodeByCountThenWalk(BuildList(values))));

    // The answer is a node, not a value, so the tail beyond it is part of what was
    // returned: asserting the remaining values pins that the strategies hand back
    // the real node rather than a detached copy carrying the right value.
    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeBySlowFastTwoPointer_LeetCodeExamples_ReturnsNodeStillLinkedToItsTail(int[] values, int _)
    {
        var middle = MiddleOfTheLinkedListSolution.MiddleNodeBySlowFastTwoPointer(BuildList(values));

        Assert.Equal(values[(values.Length / MidpointDivisor)..], ToArray(middle));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeByCountThenWalk_LeetCodeExamples_ReturnsNodeStillLinkedToItsTail(int[] values, int _)
    {
        var middle = MiddleOfTheLinkedListSolution.MiddleNodeByCountThenWalk(BuildList(values));

        Assert.Equal(values[(values.Length / MidpointDivisor)..], ToArray(middle));
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        foreach (var value in values[1..])
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return head;
    }

    // LC 876 answers null only for an empty list, and BuildList reads values[0] to build
    // its head, so every row above hands both strategies a list with a middle node.
    private static int MiddleValue(SinglyLinkedListNode<int>? middle) =>
        middle?.Value ?? throw new InvalidOperationException(
            "Every example above is a non-empty list, and the solution returns null only for an empty one.");

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
