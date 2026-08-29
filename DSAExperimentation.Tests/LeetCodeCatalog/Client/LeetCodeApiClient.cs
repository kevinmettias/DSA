using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// The one place this catalog actually calls leetcode.com's public (unauthenticated,
// undocumented but widely relied upon) GraphQL API. Deliberately never called from
// an ordinary xUnit [Fact] that runs as part of `dotnet test` - a normal test run
// must stay deterministic and offline, so this is only ever invoked from the
// explicitly Skip-tagged sync entry point (LeetCodeCatalogSyncTests.cs), run by a
// developer on demand to refresh LeetCodeQuestionCache's checked-in fixtures.
internal sealed class LeetCodeApiClient : IDisposable
{
    private const string LeetCodeOrigin = "https://leetcode.com";
    private const string GraphQlEndpoint = $"{LeetCodeOrigin}/graphql";
    private const string RefererUrl = $"{LeetCodeOrigin}/";
    private const string JsonMediaType = "application/json";
    private const string OriginHeaderName = "Origin";

    // A real browser User-Agent, not a custom one identifying this tool: without
    // it, leetcode.com's edge (Cloudflare or similar) resets the TLS connection
    // before any HTTP response - confirmed by comparing curl (works with no
    // special headers at all) against .NET's default HttpClient (fails with a
    // forcibly-closed-connection IOException at the TLS layer, a known symptom
    // of TLS-fingerprint-based bot filtering) against the exact same endpoint.
    private const string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    private const string QuestionDataOperationName = "questionData";
    private const string TitleSlugVariableName = "titleSlug";

    private const string QuestionDataQuery = """
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

    private static readonly JsonSerializerOptions ResponseSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;

    public LeetCodeApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestVersion = HttpVersion.Version11;
        _httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(BrowserUserAgent);
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(JsonMediaType);
        _httpClient.DefaultRequestHeaders.Referrer = new Uri(RefererUrl);
        _httpClient.DefaultRequestHeaders.Add(OriginHeaderName, LeetCodeOrigin);
    }

    public async Task<LeetCodeQuestion> FetchQuestionAsync(string titleSlug, CancellationToken cancellationToken = default)
    {
        var request = new LeetCodeGraphQlRequestDto
        {
            OperationName = QuestionDataOperationName,
            Variables = new Dictionary<string, string> { [TitleSlugVariableName] = titleSlug },
            Query = QuestionDataQuery,
        };

        using var response = await _httpClient.PostAsJsonAsync(GraphQlEndpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LeetCodeGraphQlResponseDto>(
            ResponseSerializerOptions, cancellationToken);
        var dto = payload?.Data?.Question
            ?? throw new InvalidOperationException($"leetcode.com returned no question data for '{titleSlug}'.");

        return LeetCodeQuestionMapper.ToQuestion(dto, DateTimeOffset.UtcNow);
    }

    public void Dispose() => _httpClient.Dispose();
}
