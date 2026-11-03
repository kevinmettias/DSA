using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// Translates LeetCodeQuestionDto - leetcode.com/graphql's own shape - into the catalog's
// LeetCodeQuestion and LeetCodeStatistics.
//
// The uncertain step is pairing each example with its output. The API gives the example INPUTS
// (exampleTestcaseList) but no output at all; the only place one exists is the statement's prose,
// read by StatementText.Outputs. They are paired by position only when the statement states
// exactly one output per example: any other count means some example has no stated output (or the
// statement shows one the list lacks), and position no longer says which output belongs to which
// input, so every example then keeps a null output rather than possibly the wrong one. An output
// stated in prose rather than in LeetCode's notation - Linked List Cycle II's "tail connects to
// node index 1", Remove Duplicates' "2, nums = [1,2,_]" - is no answer a harness can compare, so
// that example keeps a null output too.
internal static class LeetCodeQuestionMapper
{

    public static LeetCodeQuestion ToQuestion(LeetCodeQuestionDto dto)
        => new()
        {
            QuestionId = int.Parse(dto.QuestionId),
            FrontendId = int.Parse(dto.QuestionFrontendId),
            Title = dto.Title,
            TitleSlug = dto.TitleSlug,
            Difficulty = ParseDifficulty(dto.Difficulty),
            Category = dto.CategoryTitle,
            IsPaidOnly = dto.IsPaidOnly,
            Topics = dto.TopicTags.Select(tag => new LeetCodeTopic(tag.Name, tag.Slug)).ToList(),
            SimilarQuestions = SimilarQuestionsOf(dto.SimilarQuestions),
            Hints = dto.Hints,
            Constraints = StatementText.Constraints(dto.Content),
            Signature = SignatureOf(dto.MetaData),
            CSharpStarterCode = CSharpStarterCodeOf(dto.CodeSnippets),
            Examples = ExamplesOf(dto.ExampleTestcaseList, StatementText.Outputs(dto.Content)),
            ContentHtml = dto.Content,
        };

    private static List<LeetCodeRelatedQuestion> SimilarQuestionsOf(string similarQuestions)
        => Deserialize<List<LeetCodeSimilarQuestionDto>>(similarQuestions)
            .Select(similar => new LeetCodeRelatedQuestion(similar.Title, similar.TitleSlug, ParseDifficulty(similar.Difficulty)))
            .ToList();

    private static string? CSharpStarterCodeOf(List<LeetCodeCodeSnippetDto>? snippets)
        => snippets?.Find(snippet => snippet.LangSlug == LeetCodeGraphQlApi.CSharpLanguageSlug)?.Code;

    public static LeetCodeStatistics ToStatistics(LeetCodeQuestionDto dto)
    {
        var stats = Deserialize<LeetCodeStatsDto>(dto.Stats);
        return new LeetCodeStatistics(dto.AcRate, stats.TotalAcceptedRaw, stats.TotalSubmissionRaw, dto.Likes, dto.Dislikes);
    }

    // A function question fills the function half and a design question the class half; the other
    // half is left empty, so a reader never meets metaData's meaningless top-level "return" of a
    // design question.
    public static LeetCodeSignature SignatureOf(string metaData)
    {
        var dto = Deserialize<LeetCodeMetaDataDto>(metaData);
        var function = dto.ClassName is null ? dto : null;

        return new LeetCodeSignature
        {
            FunctionName = function?.Name,
            Parameters = ParametersOf(dto.Parameters),
            ReturnType = function?.Return?.Type,
            OutputParameterIndex = dto.Output?.ParameterIndex,
            HasCustomJudge = dto.Manual,
            ClassName = dto.ClassName,
            ConstructorParameters = ParametersOf(dto.Constructor?.Parameters),
            Methods = (dto.Methods ?? []).Select(MethodOf).ToList(),
        };
    }

    public static IReadOnlyList<LeetCodeCase> ExamplesOf(IReadOnlyList<string> exampleTestcases, IReadOnlyList<string> outputs)
    {
        var paired = outputs.Count == exampleTestcases.Count ? outputs : null;

        return exampleTestcases
            .Select((inputs, index) => new LeetCodeCase
            {
                Inputs = inputs.Split(LeetCodeNotation.InputSeparator),
                ExpectedOutput = NotationOrNone(paired?[index]),
                Source = LeetCodeCaseSource.Example,
            })
            .ToList();
    }

    private static string? NotationOrNone(string? output)
    {
        if (output is null)
        {
            return null;
        }

        try
        {
            using var _ = JsonDocument.Parse(output);
            return output;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // presumption: allow -- each of these strings is a nested JSON document leetcode.com's own
    // response carries for a real question, in the shape the DTO declares.
    private static TDocument Deserialize<TDocument>(string json) => JsonSerializer.Deserialize<TDocument>(json, WireJson.Options)!;

    private static LeetCodeMethod MethodOf(LeetCodeMethodDto method)
        => new(method.Name, ParametersOf(method.Parameters), method.Return?.Type ?? LeetCodeTypeNames.Void);

    private static LeetCodeDifficulty ParseDifficulty(string difficulty) => Enum.Parse<LeetCodeDifficulty>(difficulty);

    private static List<LeetCodeParameter> ParametersOf(List<LeetCodeParameterDto>? parameters)
        => (parameters ?? []).Select(parameter => new LeetCodeParameter(parameter.Name, parameter.Type)).ToList();
}
