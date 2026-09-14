using DSAExperimentation.LeetCode.MaximizeTheMinimumPoweredCity;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeTheMinimumPoweredCity;

// Harness only. Both strategies are MaximizeTheMinimumPoweredCitySolution's - the
// descending linear scan that used to live only in the benchmark's baseline arm, and
// the BinarySearch.LowerBound walk over the infeasibility sequence the test used to
// inline. The greedy feasibility sweep and the IRandomAccessSequence<bool> witness
// both moved down beside the solution.
public sealed class MaximizeTheMinimumPoweredCityTests
{
    public static TheoryData<int[], int, int, long> Examples =>
        new()
        {
            // LC examples 1 and 2.
            { [1, 2, 4, 5, 0], 1, 2, 5 },
            { [4, 4, 4, 4], 0, 3, 4 },

            // One city that reaches nothing else, so every extra station goes to it.
            { [3], 0, 5, 8 },

            // Nothing anywhere and nothing to build: the weakest city stays at zero.
            { [0], 0, 0, 0 },

            // Two cities that each cover the other, so both extra stations count twice.
            { [0, 0], 1, 2, 2 },

            // A radius wider than the whole array: every station powers every city.
            { [1, 1, 1], 5, 3, 6 },

            // The greedy placement matters: the single extra station can lift the two
            // ends or the middle, never both, so the answer stays at 1.
            { [1, 0, 0, 0, 1], 1, 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPowerByDescendingLinearScan_LeetCodeExamples_ReturnsMaximizedMinimumPower(
        int[] stations, int r, int k, long expected) =>
        Assert.Equal(
            expected, MaximizeTheMinimumPoweredCitySolution.MaxPowerByDescendingLinearScan(stations, r, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPowerBySequenceLowerBound_LeetCodeExamples_ReturnsMaximizedMinimumPower(
        int[] stations, int r, int k, long expected) =>
        Assert.Equal(
            expected, MaximizeTheMinimumPoweredCitySolution.MaxPowerBySequenceLowerBound(stations, r, k));
}
