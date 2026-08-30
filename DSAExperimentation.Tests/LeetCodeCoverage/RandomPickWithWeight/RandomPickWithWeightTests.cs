using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickWithWeight;

// LeetCode 528. Random Pick with Weight: the same prefix-sum-plus-BinarySearch
// technique RandomPointInNonOverlappingRectangleTests uses for area-weighted
// sampling, applied directly to index weights instead of rectangle areas - a
// cumulative-sum array of w, one uniform draw over the total, and this repo's own
// BinarySearch.UpperBound over an ArraySequence<int> to find which index's
// cumulative range the draw landed in.
public sealed partial class RandomPickWithWeightTests
{
    [Fact]
    public void PickIndex_SingleWeight_AlwaysReturnsThatIndex()
    {
        var solution = new Solution([1], seed: 1);

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(0, solution.PickIndex());
        }
    }

    [Fact]
    public void PickIndex_MultipleWeights_AlwaysReturnsAValidIndex()
    {
        var solution = new Solution([1, 3, 2], seed: 2);

        for (var i = 0; i < 200; i++)
        {
            Assert.Contains(solution.PickIndex(), new[] { 0, 1, 2 });
        }
    }

    [Fact]
    public void PickIndex_OneWeightFarLarger_LandsThereFarMoreOften()
    {
        var solution = new Solution([1, 999], seed: 3);

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

    private sealed class Solution
    {
        private readonly int[] _prefixSums;
        private readonly Random _random;

        public Solution(int[] w, int seed)
        {
            _random = new Random(seed);
            _prefixSums = new int[w.Length];

            var running = 0;
            for (var i = 0; i < w.Length; i++)
            {
                running += w[i];
                _prefixSums[i] = running;
            }
        }

        public int PickIndex()
        {
            var draw = _random.Next(_prefixSums[^1]);
            var sequence = new ArraySequence<int>(_prefixSums);
            return BinarySearch.UpperBound(sequence, draw);
        }
    }
}
