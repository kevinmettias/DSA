namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// Every fixed value one call to leetcode.com's public GraphQL API is built from: the origin and
// the endpoint derived from it, the headers that get a request past the site's edge, and the
// questionData operation's name, variable and query text. Held apart from LeetCodeApiClient,
// which spends them, because somebody tuning one of these reads values, not the request.
internal static class LeetCodeGraphQlApi
{
    public const string LeetCodeOrigin = "https://leetcode.com";
    public const string GraphQlEndpoint = $"{LeetCodeOrigin}/graphql";
    public const string RefererUrl = $"{LeetCodeOrigin}/";
    public const string JsonMediaType = "application/json";
    public const string OriginHeaderName = "Origin";

    // A real browser User-Agent, not one naming this tool: without it leetcode.com's edge resets
    // the TLS connection before any HTTP response. Measured when the first catalog was built by
    // comparing curl (works with no special headers) against .NET's default HttpClient (fails with
    // a forcibly closed connection), a known symptom of TLS-fingerprint bot filtering; HTTP/1.1,
    // Referer and Origin were needed with it.
    public const string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    public const string QuestionDataOperationName = "questionData";
    public const string TitleSlugVariableName = "titleSlug";

    // The language whose starter code the catalog keeps, as codeSnippets names it.
    public const string CSharpLanguageSlug = "csharp";

    public const string QuestionDataQuery = """
        query questionData($titleSlug: String!) {
          question(titleSlug: $titleSlug) {
            questionId
            questionFrontendId
            title
            titleSlug
            content
            difficulty
            categoryTitle
            likes
            dislikes
            isPaidOnly
            acRate
            stats
            hints
            metaData
            exampleTestcaseList
            similarQuestions
            topicTags { name slug }
            codeSnippets { langSlug code }
          }
        }
        """;
}
