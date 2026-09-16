namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// Every fixed value one call to leetcode.com's public GraphQL API is built from: the
// origin and the endpoint derived from it, the headers that get a request past the
// site's edge, and the questionData operation's own name, variable name and query
// text. Held apart from LeetCodeApiClient, which spends them, because they are a
// subject of their own - somebody tuning one of these reads values, not the request
// that sends them.
internal static class LeetCodeGraphQlApi
{
    public const string LeetCodeOrigin = "https://leetcode.com";
    public const string GraphQlEndpoint = $"{LeetCodeOrigin}/graphql";
    public const string RefererUrl = $"{LeetCodeOrigin}/";
    public const string JsonMediaType = "application/json";
    public const string OriginHeaderName = "Origin";

    // A real browser User-Agent, not a custom one identifying this tool: without
    // it, leetcode.com's edge (Cloudflare or similar) resets the TLS connection
    // before any HTTP response - confirmed by comparing curl (works with no
    // special headers at all) against .NET's default HttpClient (fails with a
    // forcibly-closed-connection IOException at the TLS layer, a known symptom
    // of TLS-fingerprint-based bot filtering) against the exact same endpoint.
    public const string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    public const string QuestionDataOperationName = "questionData";
    public const string TitleSlugVariableName = "titleSlug";

    public const string QuestionDataQuery = """
        query questionData($titleSlug: String!) {
          question(titleSlug: $titleSlug) {
            questionId
            questionFrontendId
            title
            titleSlug
            content
            difficulty
            likes
            dislikes
            isPaidOnly
            exampleTestcases
            metaData
            stats
            hints
            topicTags { name }
          }
        }
        """;
}
