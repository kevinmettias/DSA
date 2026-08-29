namespace DSAExperimentation.Tests.LeetCodeCoverage.Candy;

public sealed class CandyTests
{
    [Theory]
    [InlineData(new[] { 1,0,2 }, 5)]
    [InlineData(new[] { 1,2,2 }, 4)]
    [InlineData(new[] { 1,3,4,5,2 }, 11)]
    public void Candy_TwoPassSlopeConstraints_ReturnsMinimumCandies(int[] ratings, int expected) => Assert.Equal(expected, Candy(ratings));

    private static int Candy(int[] ratings)
    {
        var candies = Enumerable.Repeat(1, ratings.Length).ToArray();
        for (var i = 1; i < ratings.Length; i++) if (ratings[i] > ratings[i - 1]) candies[i] = candies[i - 1] + 1;
        for (var i = ratings.Length - 2; i >= 0; i--) if (ratings[i] > ratings[i + 1]) candies[i] = Math.Max(candies[i], candies[i + 1] + 1);
        return candies.Sum();
    }
}
