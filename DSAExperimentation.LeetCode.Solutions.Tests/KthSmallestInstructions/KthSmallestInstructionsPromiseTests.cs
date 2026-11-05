using DSAExperimentation.LeetCode.Solutions.KthSmallestInstructions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSmallestInstructions;

public sealed partial class KthSmallestInstructionsPromiseTests
{
    public static TheoryData<int[], long, bool> Inputs =>
        new()
        {
            { [2, 3], 1, true },
            { [2, 3], 10, true },
            { [15, 15], 155_117_520, true },
            { [2, 3], 11, false },
            { [14, 6], 98_765, false },
            { [2, 3], 0, false },
        };

    public static TheoryData<int[], long> PathCounts =>
        new()
        {
            { [2, 3], 10 },
            { [1, 1], 2 },
            { [15, 15], 155_117_520 },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARankAmongTheInstructions(int[] destination, long rank, bool expected)
        => Assert.Equal(expected, KthSmallestInstructionsPromise.IsKeptBy(destination, rank));

    [Theory]
    [MemberData(nameof(PathCounts))]
    public void PathCountTo_Destination_IsTheBinomialCoefficient(int[] destination, long expected)
        => Assert.Equal(expected, KthSmallestInstructionsPromise.PathCountTo(destination));
}
