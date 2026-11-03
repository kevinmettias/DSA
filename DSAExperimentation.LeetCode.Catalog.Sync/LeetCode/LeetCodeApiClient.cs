using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// The one place the catalog calls leetcode.com's public GraphQL API (unauthenticated,
// undocumented, widely relied upon). Only the sync tool constructs it; nothing `dotnet test` runs
// reaches it. The fixed values a call is built from live in LeetCodeGraphQlApi.
//
// A weekly job fetches about 1,100 questions unattended, so a transient failure - a 429 from the
// site's rate limiter, a 5xx, a dropped connection - is retried after a growing pause instead of
// failing the run; one that keeps failing surfaces to the caller, which reports it.
internal sealed class LeetCodeApiClient : IDisposable
{
    // The pauses before each retry: a first quick one for a dropped connection, then long enough
    // for a rate limiter's window to pass.
    private static readonly TimeSpan[] RetryPauses = [TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(60)];

    private readonly HttpClient _httpClient;

    public LeetCodeApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestVersion = HttpVersion.Version11;
        _httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(LeetCodeGraphQlApi.BrowserUserAgent);
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(LeetCodeGraphQlApi.JsonMediaType);
        _httpClient.DefaultRequestHeaders.Referrer = new Uri(LeetCodeGraphQlApi.RefererUrl);
        _httpClient.DefaultRequestHeaders.Add(LeetCodeGraphQlApi.OriginHeaderName, LeetCodeGraphQlApi.LeetCodeOrigin);
    }

    public async Task<LeetCodeQuestionDto> FetchQuestionAsync(string titleSlug, CancellationToken cancellationToken)
    {
        foreach (var pause in RetryPauses)
        {
            try
            {
                return await FetchOnceAsync(titleSlug, cancellationToken);
            }
            catch (HttpRequestException exception) when (IsTransient(exception))
            {
                // flakiness: allow -- a backoff before retrying a third party's server, not a
                // synchronization guess: nothing local could be observed instead.
                await Task.Delay(pause, cancellationToken);
            }
        }

        return await FetchOnceAsync(titleSlug, cancellationToken);
    }

    private async Task<LeetCodeQuestionDto> FetchOnceAsync(string titleSlug, CancellationToken cancellationToken)
    {
        var request = new LeetCodeGraphQlRequestDto
        {
            OperationName = LeetCodeGraphQlApi.QuestionDataOperationName,
            Variables = new Dictionary<string, string> { [LeetCodeGraphQlApi.TitleSlugVariableName] = titleSlug },
            Query = LeetCodeGraphQlApi.QuestionDataQuery,
        };

        using var response = await _httpClient.PostAsJsonAsync(LeetCodeGraphQlApi.GraphQlEndpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LeetCodeGraphQlResponseDto>(
            WireJson.Options, cancellationToken);

        return payload?.Data?.Question
            ?? throw new InvalidOperationException($"leetcode.com returned no question data for '{titleSlug}'.");
    }

    // No status code means the connection itself failed; 429 and 5xx are the server asking to be
    // tried again later. A 4xx otherwise is a request this tool built wrong, and retrying it would
    // only repeat the mistake.
    private static bool IsTransient(HttpRequestException exception)
        => exception.StatusCode is null or HttpStatusCode.TooManyRequests or >= HttpStatusCode.InternalServerError;

    public void Dispose() => _httpClient.Dispose();
}
