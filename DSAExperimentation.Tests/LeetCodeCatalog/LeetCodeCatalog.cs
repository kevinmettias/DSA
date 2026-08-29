namespace DSAExperimentation.Tests.LeetCodeCatalog;

// The "track and query each question" surface: a read-only view over every
// cached LeetCodeQuestion. Deliberately just LINQ-friendly enumeration plus one
// by-slug lookup, not a bespoke query language - IEnumerable<LeetCodeQuestion>
// already composes with Where/OrderBy/GroupBy for "all Hard graph questions" or
// "questions sorted by acceptance rate" style queries without this type needing
// to anticipate each one.
internal static class LeetCodeCatalog
{
    public static IReadOnlyList<LeetCodeQuestion> LoadAll()
        => LeetCodeQuestionCache.ListCachedTitleSlugs()
            .Select(LoadOrThrow)
            .OrderBy(question => question.QuestionId)
            .ToList();

    public static LeetCodeQuestion Load(string titleSlug) => LoadOrThrow(titleSlug);

    // presumption: allow -- Load is the throwing convenience beside
    // LeetCodeQuestionCache.TryLoad's own recoverable form, the same
    // "internal-trusted caller's own bug, not external-input uncertainty" shape
    // DynamicArray.Get/TryGet and DisjointSetForest.GetParent/TryGetParent already
    // use: a missing fixture here means a test asked for a slug nobody synced, not
    // state a caller couldn't have known about in advance.
    private static LeetCodeQuestion LoadOrThrow(string titleSlug)
        => LeetCodeQuestionCache.TryLoad(titleSlug, out var question) ? question : ThrowMissingFixture(titleSlug);

    private static LeetCodeQuestion ThrowMissingFixture(string titleSlug)
        => throw new KeyNotFoundException($"No cached LeetCode question fixture for '{titleSlug}'.");
}
