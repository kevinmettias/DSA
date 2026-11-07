using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Storage;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

namespace DSAExperimentation.LeetCode.Catalog.Sync;

// `questions [slug...]`: refetches every enrolled question, plus any slug named, from leetcode.com.
// A question's file is rewritten only when LeetCode changed the question; Statistics.json is
// rewritten as one snapshot dated today. A question that fails to fetch keeps its old file and its
// old figures, and QuestionsTally reports it.
internal static class QuestionsCommand
{
    public const string Verb = "questions";

    private const char ResponseFilePrefix = '@';

    // A courtesy pause between requests to a third party's public API.
    private static readonly TimeSpan BetweenRequests = TimeSpan.FromSeconds(1);

    public static async Task<int> RunAsync(
        IReadOnlyList<string> slugsToEnrol, TextWriter output, TextWriter error, CancellationToken cancellationToken)
    {
        var slugs = QuestionFiles.Slugs().Union(SlugsNamedBy(slugsToEnrol)).Order(StringComparer.Ordinal).ToList();
        var previous = StatisticsFile.Read().Questions
            .Where(entry => slugs.Contains(entry.Key))
            .ToDictionary(StringComparer.Ordinal);
        var tally = new QuestionsTally(slugs.Count, previous, output, error);

        using (var client = new LeetCodeApiClient())
        {
            foreach (var slug in slugs)
            {
                await RefreshAsync(client, slug, tally, cancellationToken);

                // flakiness: allow -- a deliberate rate limit on requests to a third party's
                // server, not a wait for anything this tool could observe instead.
                await Task.Delay(BetweenRequests, cancellationToken);
            }
        }

        new StatisticsFile(DateOnly.FromDateTime(DateTime.UtcNow), tally.Statistics).TryWrite();
        return tally.Finish();
    }

    // An operand is a slug, or @path naming a file of slugs one per line - enrolling a thousand
    // questions at once would not fit on a command line.
    private static IEnumerable<string> SlugsNamedBy(IReadOnlyList<string> operands)
        => operands.SelectMany(SlugsNamedByOne);

    private static async Task RefreshAsync(
        LeetCodeApiClient client, string slug, QuestionsTally tally, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await client.FetchQuestionAsync(slug, cancellationToken);
            Store(dto, slug, tally);
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or JsonException)
        {
            tally.Failed(slug, exception.Message);
        }
    }

    // Writes a fetched question's file when LeetCode changed it, and gives the tally its outcome
    // and figures. Mapping a malformed response throws JsonException, which RefreshAsync counts as
    // that question's failure.
    private static void Store(LeetCodeQuestionDto dto, string slug, QuestionsTally tally)
    {
        if (dto.TitleSlug != slug)
        {
            tally.Renamed(slug, dto.TitleSlug);
        }

        var mapped = LeetCodeQuestionMapper.ToQuestion(dto);
        var question = CorrectionFiles.WithCorrectedSignature(mapped);
        var statistics = LeetCodeQuestionMapper.ToStatistics(dto);

        if (QuestionFiles.TryWrite(question))
        {
            tally.Rewritten(slug, statistics);
            return;
        }

        tally.Unchanged(slug, statistics);
    }

    private static IEnumerable<string> SlugsNamedByOne(string operand)
    {
        if (!operand.StartsWith(ResponseFilePrefix))
        {
            return [operand];
        }

        return File.ReadLines(operand[1..]).Select(line => line.Trim()).Where(line => line.Length > 0);
    }
}
