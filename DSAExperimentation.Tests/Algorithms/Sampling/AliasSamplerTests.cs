using DSAExperimentation.Algorithms.Sampling;

namespace DSAExperimentation.Tests.Algorithms.Sampling;

// Exact, so every (column, ticket) pair is enumerated: index i must come up exactly weight_i times
// the column count, which is its weight's share of all the pairs.
public sealed partial class AliasSamplerTests
{
    public static TheoryData<int[]> WeightSets => new() { new[] { 3, 0, 1, 6 }, new[] { 1, 1, 1, 1 }, new[] { 7 }, new[] { 1, 2, 3, 4, 5, 6, 7 } };

    [Theory]
    [MemberData(nameof(WeightSets))]
    public void IndexFor_EveryColumnAndTicket_GivesEachIndexItsWeightsShare(int[] weights)
    {
        var sampler = new AliasSampler(weights);
        var counts = new int[weights.Length];

        for (var column = 0; column < sampler.Columns; column++)
        {
            for (var ticket = 0; ticket < sampler.Total; ticket++)
            {
                counts[sampler.IndexFor(column, ticket)]++;
            }
        }

        Assert.Equal(weights.Select(weight => weight * weights.Length), counts);
    }

    [Fact]
    public void Total_IsTheSumOfTheWeights() => Assert.Equal(10, new AliasSampler([3, 0, 1, 6]).Total);

    [Fact]
    public void Columns_IsOnePerWeight() => Assert.Equal(4, new AliasSampler([3, 0, 1, 6]).Columns);

    [Fact]
    public void AliasSampler_NegativeWeight_Throws() => Assert.Throws<ArgumentException>(() => new AliasSampler([2, -1]));

    [Fact]
    public void AliasSampler_NoPositiveWeight_Throws() => Assert.Throws<ArgumentException>(() => new AliasSampler([0, 0]));
}
