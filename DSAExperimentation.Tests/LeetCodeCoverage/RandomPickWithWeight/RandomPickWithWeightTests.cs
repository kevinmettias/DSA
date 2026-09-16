using DSAExperimentation.LeetCode.RandomPickWithWeight;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickWithWeight;

// Harness only. Both strategies are RandomPickWithWeightSolution's - this
// replays repeated PickIndex() calls against each IRandomPickWithWeight
// implementation built from LeetCode's published w. PickIndex's result is
// nondeterministic, so each example carries a seed (as the original hand-written
// test did) and the set of indices that are valid to return, plus a dedicated
// statistical case checking that a far-larger weight is favored accordingly.
public sealed partial class RandomPickWithWeightTests
{
    public static TheoryData<int[], int, int[], int> Examples =>
        new()
        {
            { [1], 1, [0], 20 },
            { [1, 3, 2], 2, [0, 1, 2], 200 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PickIndex_LeetCodeExamples_AlwaysReturnsAValidIndexByLinearScan(
        int[] weights, int seed, int[] validIndices, int trials)
        => AssertPicksAreValid(
            new RandomPickWithWeightSolution.RandomPickWithWeightByLinearScan(weights, new Random(seed)),
            validIndices,
            trials);

    [Theory]
    [MemberData(nameof(Examples))]
    public void PickIndex_LeetCodeExamples_AlwaysReturnsAValidIndexByBinarySearchUpperBound(
        int[] weights, int seed, int[] validIndices, int trials)
        => AssertPicksAreValid(
            new RandomPickWithWeightSolution.RandomPickWithWeightByBinarySearchUpperBound(weights, new Random(seed)),
            validIndices,
            trials);

    [Fact]
    public void PickIndex_OneWeightFarLarger_LandsThereFarMoreOftenByLinearScan()
        => AssertHeavilyWeightedIndexDominates(
            new RandomPickWithWeightSolution.RandomPickWithWeightByLinearScan([1, 999], new Random(3)));

    [Fact]
    public void PickIndex_OneWeightFarLarger_LandsThereFarMoreOftenByBinarySearchUpperBound()
        => AssertHeavilyWeightedIndexDominates(
            new RandomPickWithWeightSolution.RandomPickWithWeightByBinarySearchUpperBound([1, 999], new Random(3)));

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
}
