using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PoorPigs;

// LeetCode 458. Poor Pigs: with rounds = minutesToTest / minutesToDie feeding rounds
// per pig, (rounds + 1)^pigs distinct outcomes are distinguishable with `pigs` pigs -
// monotonically increasing in pigs, so the minimum pig count is exactly this repo's
// own BinarySearch.LowerBound over an ArraySequence<long> of precomputed powers.
public sealed partial class PoorPigsTests
{
    [Theory]
    [InlineData(1000, 15, 60, 5)]
    [InlineData(4, 15, 15, 2)]
    [InlineData(4, 15, 30, 2)]
    [InlineData(1, 15, 15, 0)]
    public void PoorPigs_ClassicExamples_ReturnsMinimumPigCount(
        int buckets, int minutesToDie, int minutesToTest, int expected)
    {
        var actual = MinPoisonedPigs(buckets, minutesToDie, minutesToTest);
        Assert.Equal(expected, actual);
    }

    private static int MinPoisonedPigs(int buckets, int minutesToDie, int minutesToTest)
    {
        var basis = ((long)minutesToTest / minutesToDie) + 1;

        var powers = new List<long> { 1L };

        while (powers[^1] < buckets)
        {
            powers.Add(powers[^1] * basis);
        }

        var sequence = new ArraySequence<long>(powers.ToArray());

        return BinarySearch.LowerBound<long, ArraySequence<long>>(sequence, buckets);
    }
}
