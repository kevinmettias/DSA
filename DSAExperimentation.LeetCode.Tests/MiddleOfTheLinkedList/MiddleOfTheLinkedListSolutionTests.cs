using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MiddleOfTheLinkedList;

namespace DSAExperimentation.LeetCode.Tests.MiddleOfTheLinkedList;

// Harness only. Both strategies are MiddleOfTheLinkedListSolution's - this file
// states LeetCode's examples once as the list's values plus the value of the node
// the answer must land on, and asserts each strategy against them.
public sealed partial class MiddleOfTheLinkedListSolutionTests
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
            MiddleValue(
                MiddleOfTheLinkedListSolution.MiddleNodeBySlowFastTwoPointer(LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeByCountThenWalk_LeetCodeExamples_ReturnsMiddleNode(int[] values, int expected) =>
        Assert.Equal(
            expected,
            MiddleValue(
                MiddleOfTheLinkedListSolution.MiddleNodeByCountThenWalk(LeetCodeWireFormat.ToLinkedList(values))));

    // The answer is a node, not a value, so the tail beyond it is part of what was
    // returned: asserting the remaining values pins that the strategies hand back
    // the real node rather than a detached copy carrying the right value.
    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeBySlowFastTwoPointer_LeetCodeExamples_ReturnsNodeStillLinkedToItsTail(int[] values, int _)
    {
        var middle = MiddleOfTheLinkedListSolution.MiddleNodeBySlowFastTwoPointer(
            LeetCodeWireFormat.ToLinkedList(values));

        Assert.Equal(values[(values.Length / MidpointDivisor)..], LeetCodeWireFormat.FromLinkedList(middle));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MiddleNodeByCountThenWalk_LeetCodeExamples_ReturnsNodeStillLinkedToItsTail(int[] values, int _)
    {
        var middle = MiddleOfTheLinkedListSolution.MiddleNodeByCountThenWalk(
            LeetCodeWireFormat.ToLinkedList(values));

        Assert.Equal(values[(values.Length / MidpointDivisor)..], LeetCodeWireFormat.FromLinkedList(middle));
    }

    // LC 876 answers null only for an empty list, and every row above states at least
    // one value, so both strategies are handed a list with a middle node.
    private static int MiddleValue(SinglyLinkedListNode<int>? middle) =>
        middle?.Value ?? throw new InvalidOperationException(
            "Every example above is a non-empty list, and the solution returns null only for an empty one.");
}
