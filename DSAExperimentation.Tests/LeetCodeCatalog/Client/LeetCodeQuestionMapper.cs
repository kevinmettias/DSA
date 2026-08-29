using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Client;

// Translates LeetCodeQuestionDto (leetcode.com/graphql's own response shape) into
// this catalog's LeetCodeQuestion. The one genuinely uncertain step is
// ExtractExampleTestCases: LeetCode's public API gives raw example INPUTS
// (ExampleTestcases) but no structured expected-output field at all - the only
// place an expected output exists is prose inside the question's own HTML
// description (ContentHtml), so pairing an output to its input is a best-effort
// regex scrape, not a guaranteed-correct parse. A test case whose output can't be
// matched this way keeps RawExpectedOutput null rather than a guessed value -
// LeetCodeSolutionValidator already treats null as "nothing to compare," not a
// failure to fabricate around.
internal static class LeetCodeQuestionMapper
{
    // Matches one Example block's Output line inside LeetCode's own HTML content:
    // "<strong>Output:</strong> VALUE" up to either the next <strong> tag (an
    // Explanation line) or the closing </pre>. Singleline so a rare multi-line
    // output value is still captured whole.
    private static readonly Regex OutputPattern = new(
        @"<strong>Output:</strong>\s*(.*?)\s*(?:\n<strong>|</pre>)",
        RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static LeetCodeQuestion ToQuestion(LeetCodeQuestionDto dto, DateTimeOffset fetchedAtUtc)
    {
        // presumption: allow -- dto.Stats/dto.MetaData are always the nested JSON
        // strings leetcode.com's own GraphQL response carries for a real
        // question, in the shape LeetCodeStatsDto/LeetCodeMetaDataDto declare.
        var stats = JsonSerializer.Deserialize<LeetCodeStatsDto>(dto.Stats, SerializerOptions)!;
        var paramCount = JsonSerializer.Deserialize<LeetCodeMetaDataDto>(dto.MetaData, SerializerOptions)!.Parameters?.Count;

        return new LeetCodeQuestion
        {
            QuestionId = int.Parse(dto.QuestionId),
            FrontendId = int.Parse(dto.QuestionFrontendId),
            Title = dto.Title,
            TitleSlug = dto.TitleSlug,
            Difficulty = ParseDifficulty(dto.Difficulty),
            TopicTags = dto.TopicTags.Select(tag => tag.Name).ToList(),
            AcceptanceRatePercent = double.Parse(stats.AcRate.TrimEnd('%')),
            TotalAccepted = stats.TotalAcceptedRaw,
            TotalSubmissions = stats.TotalSubmissionRaw,
            Likes = dto.Likes,
            Dislikes = dto.Dislikes,
            IsPaidOnly = dto.IsPaidOnly,
            ContentHtml = dto.Content,
            Hints = dto.Hints,
            ExampleTestCases = ExtractExampleTestCases(dto, paramCount),
            FetchedAtUtc = fetchedAtUtc,
        };
    }

    private static LeetCodeDifficulty ParseDifficulty(string difficulty) => Enum.Parse<LeetCodeDifficulty>(difficulty);

    // paramCount is null for a "systemdesign" (class-based, multi-method) problem
    // - see LeetCodeMetaDataDto's own doc comment for why that shape is left
    // unparsed here rather than guessed at.
    private static List<LeetCodeTestCase> ExtractExampleTestCases(LeetCodeQuestionDto dto, int? paramCount)
    {
        if (paramCount is not { } count || count == 0)
        {
            return [];
        }

        var rawInputs = ChunkIntoRawInputs(dto.ExampleTestcases, count);
        var outputs = ScrapeOutputs(dto.Content);

        return rawInputs
            .Select((rawInput, index) => new LeetCodeTestCase(rawInput, OutputAt(outputs, index)))
            .ToList();
    }

    private static string? OutputAt(List<string> outputs, int index)
        => index < outputs.Count ? GetOutput(outputs, index) : null;

    private static string GetOutput(List<string> outputs, int index) => outputs[index];

    private static List<string> ChunkIntoRawInputs(string exampleTestcases, int paramCount)
    {
        var lines = exampleTestcases.Split('\n');

        return Enumerable.Range(0, lines.Length / paramCount)
            .Select(exampleIndex => string.Join('\n', lines.Skip(exampleIndex * paramCount).Take(paramCount)))
            .ToList();
    }

    private static List<string> ScrapeOutputs(string contentHtml)
        => OutputPattern.Matches(contentHtml)
            .Select(match => WebUtility.HtmlDecode(match.Groups[1].Value))
            .ToList();
}
