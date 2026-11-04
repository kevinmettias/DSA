using DSAExperimentation.LeetCode.Solutions.RemoveDuplicatesFromSortedListII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveDuplicatesFromSortedListII;

public sealed partial class RemoveDuplicatesFromSortedListIIPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 2, 3, 3, 4, 4, 5], true },
            { [], true },
            { [-100, 100, -100, 100], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASortedList(int[] values, bool expected)
        => Assert.Equal(expected, RemoveDuplicatesFromSortedListIIPromise.IsKeptBy(LeetCodeWireFormat.ToLinkedList(values)));
}
