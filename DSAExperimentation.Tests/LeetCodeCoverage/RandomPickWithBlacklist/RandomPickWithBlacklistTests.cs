using DSAExperimentation.LeetCode.RandomPickWithBlacklist;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickWithBlacklist;

// Harness only. Both strategies are RandomPickWithBlacklistSolution's - this file
// replays LeetCode's published (n, blacklist, seed) scenarios against each IRandomPick
// instance, so a failure still names the strategy that broke. The pre-migration test
// only proved CreateBySetHashMapRemap's behaviour - CreateByRejectionSampling's baseline
// (previously untested scaffolding inlined in the benchmark) gets the same coverage
// here for the first time. Every scenario asserts the two properties LeetCode itself
// guarantees - every Pick() lands in the whitelist and nowhere else - and the
// "eventually covers every whitelisted number" scenario additionally asserts full
// coverage after enough calls, exactly as the pre-migration test did for its one
// strategy.
public sealed class RandomPickWithBlacklistTests
{
    public static TheoryData<int, int[], int, int, int[], bool> Examples =>
        new()
        {
            // Never returns a blacklisted number.
            { 7, [2, 3, 5], 1, 200, [0, 1, 4, 6], false },
            // Many calls eventually return every whitelisted number.
            { 5, [1, 3], 1, 200, [0, 2, 4], true },
            // Empty blacklist always returns within range.
            { 3, [], 1, 20, [0, 1, 2], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRejectionSampling_LeetCodeExamples_OnlyReturnsWhitelistedNumbers(
        int n, int[] blacklist, int seed, int calls, int[] whitelisted, bool expectFullCoverage) =>
        AssertOnlyReturnsWhitelistedNumbers(
            RandomPickWithBlacklistSolution.CreateByRejectionSampling(n, blacklist, seed),
            calls, whitelisted, expectFullCoverage);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySetHashMapRemap_LeetCodeExamples_OnlyReturnsWhitelistedNumbers(
        int n, int[] blacklist, int seed, int calls, int[] whitelisted, bool expectFullCoverage) =>
        AssertOnlyReturnsWhitelistedNumbers(
            RandomPickWithBlacklistSolution.CreateBySetHashMapRemap(n, blacklist, seed),
            calls, whitelisted, expectFullCoverage);

    private static void AssertOnlyReturnsWhitelistedNumbers(
        RandomPickWithBlacklistSolution.IRandomPick randomPick,
        int calls, int[] whitelisted, bool expectFullCoverage)
    {
        var whitelistedSet = new HashSet<int>(whitelisted);
        var seen = new HashSet<int>();

        for (var i = 0; i < calls; i++)
        {
            var picked = randomPick.Pick();
            Assert.Contains(picked, whitelistedSet);
            seen.Add(picked);
        }

        if (expectFullCoverage)
        {
            Assert.Equal(whitelistedSet.OrderBy(x => x), seen.OrderBy(x => x));
        }
    }
}
