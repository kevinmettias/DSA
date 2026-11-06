namespace DSAExperimentation.LeetCode.Tests;

public sealed partial class LeetCodeSearchTreesTests
{
    public static TheoryData<int[]> RisingSequences => [[], [1, 2, 4]];

    // A value repeated, and a value below the one before it.
    public static TheoryData<int[]> FallingOrRepeatingSequences => [[1, 2, 2], [3, 1]];

    [Fact]
    public void InOrderValues_LeetCodesTree_ReadsLeftNodeRight()
    {
        var root = LeetCodeWireFormat.ToBinaryTree([5, 3, 6, 2, 4, null, 7]);

        Assert.Equal([2, 3, 4, 5, 6, 7], LeetCodeSearchTrees.InOrderValues(root));
    }

    [Fact]
    public void InOrderValues_EmptyTree_IsEmpty() => Assert.Empty(LeetCodeSearchTrees.InOrderValues(null));

    [Theory]
    [MemberData(nameof(RisingSequences))]
    public void IsStrictlyRising_EachValueAboveTheOneBefore_IsTrue(int[] values) => Assert.True(LeetCodeSearchTrees.IsStrictlyRising(values));

    [Theory]
    [MemberData(nameof(FallingOrRepeatingSequences))]
    public void IsStrictlyRising_SomeValueNotAboveTheOneBefore_IsFalse(int[] values) => Assert.False(LeetCodeSearchTrees.IsStrictlyRising(values));
}
