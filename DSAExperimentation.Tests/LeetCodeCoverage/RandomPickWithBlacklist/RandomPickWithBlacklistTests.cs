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
    public static TheoryData<BlacklistScenario> Examples =>
        new()
        {
            // Never returns a blacklisted number.
            { new BlacklistScenario(N: 7, Blacklist: [2, 3, 5], Seed: 1, Calls: 200, Whitelisted: [0, 1, 4, 6], Claim: CoverageClaim.StaysWithinWhitelist) },
            // Many calls eventually return every whitelisted number.
            { new BlacklistScenario(N: 5, Blacklist: [1, 3], Seed: 1, Calls: 200, Whitelisted: [0, 2, 4], Claim: CoverageClaim.CoversWholeWhitelist) },
            // Empty blacklist always returns within range.
            { new BlacklistScenario(N: 3, Blacklist: [], Seed: 1, Calls: 20, Whitelisted: [0, 1, 2], Claim: CoverageClaim.StaysWithinWhitelist) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRejectionSampling_LeetCodeExamples_OnlyReturnsWhitelistedNumbers(
        BlacklistScenario example)
    {
        var randomPick = RandomPickWithBlacklistSolution.CreateByRejectionSampling(
            example.N, example.Blacklist, example.Seed);

        AssertPickerRespectsWhitelist(randomPick, example.Calls, example.Whitelisted, example.Claim);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySetHashMapRemap_LeetCodeExamples_OnlyReturnsWhitelistedNumbers(
        BlacklistScenario example)
    {
        var randomPick = RandomPickWithBlacklistSolution.CreateBySetHashMapRemap(
            example.N, example.Blacklist, example.Seed);

        AssertPickerRespectsWhitelist(randomPick, example.Calls, example.Whitelisted, example.Claim);
    }

    private static void AssertPickerRespectsWhitelist(
        RandomPickWithBlacklistSolution.IRandomPick randomPick,
        int calls,
        int[] whitelisted,
        CoverageClaim claim)
    {
        var whitelistedSet = new HashSet<int>(whitelisted);
        var seen = new HashSet<int>();

        for (var i = 0; i < calls; i++)
        {
            var picked = randomPick.Pick();
            Assert.Contains(picked, whitelistedSet);
            seen.Add(picked);
        }

        if (claim == CoverageClaim.CoversWholeWhitelist)
        {
            Assert.Equal(whitelistedSet.OrderBy(x => x), seen.OrderBy(x => x));
        }
    }

    // How much a scenario demands of a picker: that it never leaves the whitelist, or
    // that it also visits all of it once given enough calls. A named choice rather than
    // a bool, so the row says which claim it is making instead of leaving the reader to
    // decode a bare `true`. Public because it names a field of a public row, which is
    // what lets `Examples` itself stay a public member.
    public enum CoverageClaim
    {
        StaysWithinWhitelist,
        CoversWholeWhitelist,
    }

    // One LeetCode example: the universe size, the excluded numbers, the seed the
    // generator is pinned to, how many picks to replay, the numbers those picks may
    // return, and which claim about them the scenario makes. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct BlacklistScenario(
        int N, int[] Blacklist, int Seed, int Calls, int[] Whitelisted, CoverageClaim Claim);
}
