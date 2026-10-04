using DSAExperimentation.LeetCode.RandomPickWithWeight;

namespace DSAExperimentation.LeetCode.Tests.RandomPickWithWeight;

// Harness only. Both strategies are RandomPickWithWeightSolution's - this
// replays repeated PickIndex() calls against each IRandomPickWithWeight
// implementation built from LeetCode's published w. PickIndex's result is
// nondeterministic, so each example carries a seed (as the original hand-written
// test did) and the set of indices that are valid to return - LeetCode accepts any
// sequence of valid picks - plus statistical cases checking that each weight is
// favored in proportion.
public sealed partial class RandomPickWithWeightSolutionTests
{
    public static TheoryData<int[], int, int[], int> Examples =>
        new()
        {
            // LeetCode example 1, its single pick repeated 20 times: index 0 is the only
            // option.
            { [1], 1, [0], 20 },

            // LeetCode example 2: five picks, each 0 or 1.
            { [1, 3], 4, [0, 1], 5 },

            // Weights of this file's own: 200 picks, each 0, 1 or 2.
            { [1, 3, 2], 2, [0, 1, 2], 200 },
        };

    // LeetCode example 2's weights [1, 3] put 3 of their 4 units on index 1, so 4,000
    // seeded picks land there about 3,000 times. One standard deviation is
    // sqrt(4000 * 3/4 * 1/4), about 27, so 2,850..3,150 is more than five of them on
    // each side.
    public static TheoryData<int[], int, PickShare> Shares =>
        new()
        {
            { [1, 3], 5, new PickShare(Index: 1, Picks: 4_000, FewestHits: 2_850, MostHits: 3_150) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomPickWithWeightByLinearScan_LeetCodeExamples_AlwaysReturnsAValidIndex(
        int[] weights, int seed, int[] validIndices, int trials)
        => AssertPicksAreValid(
            new RandomPickWithWeightSolution.RandomPickWithWeightByLinearScan(weights, new Random(seed)),
            validIndices,
            trials);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RandomPickWithWeightByBinarySearchUpperBound_LeetCodeExamples_AlwaysReturnsAValidIndex(
        int[] weights, int seed, int[] validIndices, int trials)
        => AssertPicksAreValid(
            new RandomPickWithWeightSolution.RandomPickWithWeightByBinarySearchUpperBound(weights, new Random(seed)),
            validIndices,
            trials);

    [Fact]
    public void RandomPickWithWeightByLinearScan_OneWeightFarLarger_LandsThereFarMoreOften()
        => AssertHeavilyWeightedIndexDominates(
            new RandomPickWithWeightSolution.RandomPickWithWeightByLinearScan([1, 999], new Random(3)));

    [Fact]
    public void RandomPickWithWeightByBinarySearchUpperBound_OneWeightFarLarger_LandsThereFarMoreOften()
        => AssertHeavilyWeightedIndexDominates(
            new RandomPickWithWeightSolution.RandomPickWithWeightByBinarySearchUpperBound([1, 999], new Random(3)));

    [Theory]
    [MemberData(nameof(Shares))]
    public void RandomPickWithWeightByLinearScan_LeetCodeExample2Weights_PicksEachIndexInProportion(
        int[] weights, int seed, PickShare share)
        => AssertHitsWithinShare(
            new RandomPickWithWeightSolution.RandomPickWithWeightByLinearScan(weights, new Random(seed)),
            share);

    [Theory]
    [MemberData(nameof(Shares))]
    public void RandomPickWithWeightByBinarySearchUpperBound_LeetCodeExample2Weights_PicksEachIndexInProportion(
        int[] weights, int seed, PickShare share)
        => AssertHitsWithinShare(
            new RandomPickWithWeightSolution.RandomPickWithWeightByBinarySearchUpperBound(weights, new Random(seed)),
            share);

    private static void AssertPicksAreValid(
        RandomPickWithWeightSolution.IRandomPickWithWeight solution, int[] validIndices, int trials)
    {
        for (var i = 0; i < trials; i++)
        {
            Assert.Contains(solution.PickIndex(), validIndices);
        }
    }

    private static void AssertHeavilyWeightedIndexDominates(
        RandomPickWithWeightSolution.IRandomPickWithWeight solution)
    {
        var heavyIndexHits = 0;

        for (var i = 0; i < 500; i++)
        {
            if (solution.PickIndex() == 1)
            {
                heavyIndexHits++;
            }
        }

        Assert.True(heavyIndexHits > 480);
    }

    private static void AssertHitsWithinShare(
        RandomPickWithWeightSolution.IRandomPickWithWeight solution, PickShare share)
    {
        var hits = Enumerable.Range(0, share.Picks).Count(_ => solution.PickIndex() == share.Index);

        Assert.InRange(hits, share.FewestHits, share.MostHits);
    }

    // How often one index should come up: over Picks draws, between FewestHits and
    // MostHits of them land on Index. Named fields, because four adjacent ints in a
    // data row would say nothing about which bound is which.
    public readonly record struct PickShare(int Index, int Picks, int FewestHits, int MostHits);
}
