using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MostCommonWordWorkloads (ARCHITECTURE 17.7). The reading depends on the tokens
// being ones an LC 819 paragraph can split into - lowercase letters only, at most 1,000 characters once
// joined by single spaces - on a banned list LC 819 accepts, and on the answer being unique, which LC
// 819 guarantees and random draws alone do not.
public sealed partial class MostCommonWordWorkloadsTests
{
    private const int SmallestTokenCount = 16;
    private const int LargestTokenCount = 166;
    private const int PoolSize = 20;
    private const int BannedCount = 5;
    private const int Seed = 819; // LC problem number

    // LC 819's own bounds on the paragraph and on a banned word.
    private const int MaxParagraphLength = 1_000;
    private const int MaxBannedWordLength = 10;

    [Fact]
    public void Build_EveryTokenAndBannedWord_IsLowercaseLettersOnly()
    {
        var (tokens, banned) = MostCommonWordWorkloads.Build(LargestTokenCount, PoolSize, BannedCount, Seed);

        Assert.All(tokens, token => Assert.True(token.All(char.IsAsciiLetterLower)));
        Assert.All(banned, word => Assert.True(word.All(char.IsAsciiLetterLower)));
        Assert.All(banned, word => Assert.InRange(word.Length, 1, MaxBannedWordLength));
    }

    [Fact]
    public void Build_LargestTokenCount_JoinsIntoAParagraphLeetCodeAccepts()
    {
        var (tokens, _) = MostCommonWordWorkloads.Build(LargestTokenCount, PoolSize, BannedCount, Seed);

        Assert.Equal(LargestTokenCount, tokens.Length);
        Assert.InRange(string.Join(' ', tokens).Length, 1, MaxParagraphLength);
    }

    [Theory]
    [InlineData(SmallestTokenCount)]
    [InlineData(LargestTokenCount)]
    public void Build_MostCommonAllowedWord_LeadsAlone(int tokenCount)
    {
        var (tokens, banned) = MostCommonWordWorkloads.Build(tokenCount, PoolSize, BannedCount, Seed);
        var allowedCounts = tokens
            .Where(token => !banned.Contains(token))
            .GroupBy(token => token)
            .Select(group => group.Count())
            .ToArray();
        var topCount = allowedCounts.Max();

        Assert.Single(allowedCounts, count => count == topCount);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (tokens, banned) = MostCommonWordWorkloads.Build(LargestTokenCount, PoolSize, BannedCount, Seed);
        var (repeatTokens, repeatBanned) = MostCommonWordWorkloads.Build(LargestTokenCount, PoolSize, BannedCount, Seed);

        Assert.Equal(tokens, repeatTokens);
        Assert.Equal(banned, repeatBanned);
    }
}
