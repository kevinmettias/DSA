using DSAExperimentation.LeetCode.Solutions.ModifyGraphEdgeWeights;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ModifyGraphEdgeWeights;

public sealed partial class ModifyGraphEdgeWeightsAnswerCheckTests
{
    // A 6-cycle whose six edges are all -1, from 0 to 3 at distance 12, recorded with the far side heavy.
    private static readonly int[][] Cycle = [[0, 1, -1], [1, 2, -1], [2, 3, -1], [3, 4, -1], [4, 5, -1], [5, 0, -1]];
    private static readonly int[][] RecordedCycle = [[0, 1, 1], [1, 2, 1], [2, 3, 10], [3, 4, 2_000_000_000], [4, 5, 2_000_000_000], [5, 0, 2_000_000_000]];

    public static TheoryData<int[][], int[][], bool> Answers =>
        new()
        {
            { RecordedCycle, RecordedCycle, true },
            { [[0, 1, 10], [1, 2, 6], [2, 3, 8], [3, 4, 10], [4, 5, 1], [5, 0, 1]], RecordedCycle, true },
            { [[5, 0, 1], [4, 5, 1], [3, 4, 10], [2, 3, 8], [1, 2, 6], [0, 1, 10]], RecordedCycle, true },
            { [[0, 1, 1], [1, 2, 1], [2, 3, 1], [3, 4, 1], [4, 5, 1], [5, 0, 1]], RecordedCycle, false },
            { [[0, 1, 0], [1, 2, 1], [2, 3, 11], [3, 4, 2_000_000_000], [4, 5, 2_000_000_000], [5, 0, 2_000_000_000]], RecordedCycle, false },
            { [[0, 1, 1], [1, 2, 1], [2, 3, 10], [3, 4, 2_000_000_000], [4, 5, 2_000_000_000]], RecordedCycle, false },
            { [], RecordedCycle, false },
            { [], [], true },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForWeightsThatPutTheDestinationAtTarget(int[][] answer, int[][] recorded, bool expected)
        => Assert.Equal(expected, ModifyGraphEdgeWeightsAnswerCheck.IsAccepted(6, Cycle, 0, 3, 12, answer, recorded));
}
