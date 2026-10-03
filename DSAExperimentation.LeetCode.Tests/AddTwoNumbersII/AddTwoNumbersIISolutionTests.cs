using DSAExperimentation.LeetCode.AddTwoNumbersII;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.LeetCode.Tests.AddTwoNumbersII;

// Harness only. Both strategies are AddTwoNumbersIISolution's - this file just
// pins them to LeetCode's published examples, converting each row's operand/result
// arrays to/from the repo's own SinglyLinkedListNode<int>.
public sealed partial class AddTwoNumbersIISolutionTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            { [7, 2, 4, 3], [5, 6, 4], [7, 8, 0, 7] },
            { [9, 9, 9], [1], [1, 0, 0, 0] },
            { [8, 4, 6], [5], [8, 5, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByBigInteger_LeetCodeExamples_ReturnsDigitwiseSumInOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersIISolution.AddByBigInteger(
            LeetCodeWireFormat.ToLinkedList(first), LeetCodeWireFormat.ToLinkedList(second));
        var actual = LeetCodeWireFormat.FromLinkedList(sum);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AddByTwoStacks_LeetCodeExamples_ReturnsDigitwiseSumInOrder(
        int[] first, int[] second, int[] expected)
    {
        var sum = AddTwoNumbersIISolution.AddByTwoStacks(
            LeetCodeWireFormat.ToLinkedList(first), LeetCodeWireFormat.ToLinkedList(second));
        var actual = LeetCodeWireFormat.FromLinkedList(sum);

        Assert.Equal(expected, actual);
    }
}
