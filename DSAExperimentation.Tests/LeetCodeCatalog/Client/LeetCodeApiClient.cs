using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// The one place this catalog actually calls leetcode.com's public (unauthenticated,
// undocumented but widely relied upon) GraphQL API. Deliberately never called from
// an ordinary xUnit [Fact] that runs as part of `dotnet test` - a normal test run
// must stay deterministic and offline, so this is only ever invoked from the
// explicitly Skip-tagged sync entry point (LeetCodeCatalogSyncTests.cs), run by a
// developer on demand to refresh LeetCodeQuestionCache's checked-in fixtures. The
// fixed values one call is built from - origin, endpoint, headers and the
// questionData operation itself - live in LeetCodeGraphQlApi, a type holding nothing
// but them.
internal sealed class LeetCodeApiClient : IDisposable
{
    private static readonly JsonSerializerOptions ResponseSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;

    public LeetCodeApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestVersion = HttpVersion.Version11;
        _httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(LeetCodeGraphQlApi.BrowserUserAgent);
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(LeetCodeGraphQlApi.JsonMediaType);
        _httpClient.DefaultRequestHeaders.Referrer = new Uri(LeetCodeGraphQlApi.RefererUrl);
        _httpClient.DefaultRequestHeaders.Add(
            LeetCodeGraphQlApi.OriginHeaderName, LeetCodeGraphQlApi.LeetCodeOrigin);
    }

    public async Task<LeetCodeQuestion> FetchQuestionAsync(string titleSlug, CancellationToken cancellationToken = default)
    {
        var request = new LeetCodeGraphQlRequestDto
        {
            OperationName = LeetCodeGraphQlApi.QuestionDataOperationName,
            Variables = new Dictionary<string, string>
            {
                [LeetCodeGraphQlApi.TitleSlugVariableName] = titleSlug,
            },
            Query = LeetCodeGraphQlApi.QuestionDataQuery,
        };

        using var response = await _httpClient.PostAsJsonAsync(
            LeetCodeGraphQlApi.GraphQlEndpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LeetCodeGraphQlResponseDto>(
            ResponseSerializerOptions, cancellationToken);
        var dto = payload?.Data?.Question
            ?? throw new InvalidOperationException($"leetcode.com returned no question data for '{titleSlug}'.");

        return LeetCodeQuestionMapper.ToQuestion(dto, DateTimeOffset.UtcNow);
    }

    public void Dispose() => _httpClient.Dispose();
}
