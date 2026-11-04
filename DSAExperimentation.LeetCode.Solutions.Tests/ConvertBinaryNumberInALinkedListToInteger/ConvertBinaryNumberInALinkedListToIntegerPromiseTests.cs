using DSAExperimentation.LeetCode.Solutions.ConvertBinaryNumberInALinkedListToInteger;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConvertBinaryNumberInALinkedListToInteger;

public sealed partial class ConvertBinaryNumberInALinkedListToIntegerPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 0, 1], true },
            { [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAtMostThirtyNodes(int[] values, bool expected)
    {
        var head = LeetCodeWireFormat.ToLinkedList(values);

        Assert.Equal(expected, ConvertBinaryNumberInALinkedListToIntegerPromise.IsKeptBy(head));
    }
}
