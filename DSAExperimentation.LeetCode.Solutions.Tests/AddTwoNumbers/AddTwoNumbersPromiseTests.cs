using DSAExperimentation.LeetCode.Solutions.AddTwoNumbers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AddTwoNumbers;

public sealed partial class AddTwoNumbersPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [2, 4, 3], [5, 6, 4], true },
            { [0], [0], true },
            { [1, 0], [5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNeitherNumberHasALeadingZero(int[] first, int[] second, bool expected)
    {
        var firstList = LeetCodeWireFormat.ToLinkedList(first);
        var secondList = LeetCodeWireFormat.ToLinkedList(second);

        Assert.Equal(expected, AddTwoNumbersPromise.IsKeptBy(firstList, secondList));
    }
}
