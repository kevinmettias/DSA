using DSAExperimentation.LeetCode.Solutions.QueueReconstructionByHeight;

namespace DSAExperimentation.LeetCode.Solutions.Tests.QueueReconstructionByHeight;

public sealed partial class QueueReconstructionByHeightPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[7, 0], [4, 4], [7, 1], [5, 0], [6, 1], [5, 2]], true },
            { [[6, 0], [5, 0], [4, 0], [3, 2], [2, 2], [1, 4]], true },
            { [[5, 2], [7, 0], [5, 0], [6, 1], [3, 2], [2, 2], [1, 4], [8, 1]], false },
            { [[5, 0], [5, 0]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAQueueFitsEveryone(int[][] people, bool expected)
        => Assert.Equal(expected, QueueReconstructionByHeightPromise.IsKeptBy(people));
}
