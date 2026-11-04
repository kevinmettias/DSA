using DSAExperimentation.LeetCode.Solutions.SmallestMissingGeneticValueInEachSubtree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestMissingGeneticValueInEachSubtree;

public sealed partial class SmallestMissingGeneticValueInEachSubtreePromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [-1, 0, 0, 2], [1, 2, 3, 4], true },
            { [-1, 0, 1, 0, 3, 3], [5, 4, 6, 2, 1, 3], true },
            { [-1, 0, 0], [1, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheValuesAreDistinct(int[] parents, int[] nums, bool expected)
        => Assert.Equal(expected, SmallestMissingGeneticValueInEachSubtreePromise.IsKeptBy(parents, nums));
}
