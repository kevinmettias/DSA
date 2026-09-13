using DSAExperimentation.Tests.LeetCodeCatalog.Client;

namespace DSAExperimentation.Tests.LeetCodeCatalog;

// NOT part of the normal `dotnet test` run - Skip keeps it out of CI/every-run
// discovery entirely (a normal run must stay deterministic and offline, per
// LeetCodeApiClient's own doc comment). Run on demand to refresh
// Fixtures/*.json: temporarily delete the Skip argument (or run with
// `dotnet test --filter "FullyQualifiedName~RefreshFixtures"` after doing so),
// run once, then restore the Skip.
//
// The set it refreshes is THE FIXTURES THAT ALREADY EXIST, not a list written out
// here. It used to be a hardcoded thirteen slugs from when this catalog was
// built, which silently stayed at thirteen while coverage grew past eleven
// hundred - so the catalog tracked full metadata for roughly one percent of the
// problems it was supposed to describe and nothing said so. Deriving the set from
// the cache cannot drift that way: adding a problem's fixture is what enrols it,
// and every later refresh picks it up.
public sealed partial class LeetCodeCatalogSyncTests
{
    // A courtesy pause between requests to a third party's public API, not a
    // synchronization guess - see the note at its use site.
    private static readonly TimeSpan BetweenRequests = TimeSpan.FromSeconds(1);

    [Fact(Skip = "Hits the live leetcode.com API - run manually to refresh Fixtures/*.json, then restore Skip.")]
    public async Task RefreshFixtures_EveryCachedQuestion_RewritesFixtureFiles()
    {
        var titleSlugs = LeetCodeQuestionCache.ListCachedTitleSlugs();

        Assert.NotEmpty(titleSlugs);

        using var client = new LeetCodeApiClient();

        foreach (var titleSlug in titleSlugs)
        {
            var question = await client.FetchQuestionAsync(titleSlug);
            LeetCodeQuestionCache.Save(question);

            // flakiness: allow -- a deliberate rate limiter, not a synchronization
            // guess: this pauses between requests to leetcode.com's own public API
            // out of courtesy to a third party's servers, not to wait for
            // anything this test could instead observe directly.
            await Task.Delay(BetweenRequests);
        }

        Assert.Equal(titleSlugs.Count, LeetCodeQuestionCache.ListCachedTitleSlugs().Count);
    }
}
