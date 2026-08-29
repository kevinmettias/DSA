using DSAExperimentation.Tests.LeetCodeCatalog.Client;

namespace DSAExperimentation.Tests.LeetCodeCatalog;

// NOT part of the normal `dotnet test` run - Skip keeps it out of CI/every-run
// discovery entirely (a normal run must stay deterministic and offline, per
// LeetCodeApiClient's own doc comment). Run on demand to (re)populate
// Fixtures/*.json: temporarily delete the Skip argument (or run with
// `dotnet test --filter "FullyQualifiedName~RefreshFixtures"` after doing so),
// run once, then restore the Skip.
public sealed partial class LeetCodeCatalogSyncTests
{
    private static readonly string[] CoveredTitleSlugs =
    [
        "two-sum",
        "valid-parentheses",
        "min-stack",
        "merge-two-sorted-lists",
        "network-delay-time",
        "course-schedule",
        "redundant-connection",
        "kth-largest-element-in-an-array",
        "lowest-common-ancestor-of-a-binary-search-tree",
        "merge-intervals",
        "subsets",
        "climbing-stairs",
        "implement-trie-prefix-tree",
    ];

    [Fact(Skip = "Hits the live leetcode.com API - run manually to refresh Fixtures/*.json, then restore Skip.")]
    public async Task RefreshFixtures_EveryCoveredQuestion_WritesFixtureFiles()
    {
        using var client = new LeetCodeApiClient();

        foreach (var titleSlug in CoveredTitleSlugs)
        {
            var question = await client.FetchQuestionAsync(titleSlug);
            LeetCodeQuestionCache.Save(question);

            // flakiness: allow -- a deliberate rate limiter, not a synchronization
            // guess: this pauses between requests to leetcode.com's own public API
            // out of courtesy to a third party's servers, not to wait for
            // anything this test could instead observe directly.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        Assert.Equal(CoveredTitleSlugs.Length, LeetCodeQuestionCache.ListCachedTitleSlugs().Count);
    }
}
