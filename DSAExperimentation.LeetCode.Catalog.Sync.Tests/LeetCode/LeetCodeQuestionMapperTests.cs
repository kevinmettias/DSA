using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode.GraphQl;
using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.LeetCode;

public sealed partial class LeetCodeQuestionMapperTests
{
    private const string FunctionSignatureJson = """
        {"name": "hasCycle", "params": [{"name": "head", "type": "ListNode"}, {"name": "pos", "type": "integer"}],
         "return": {"type": "boolean"}, "manual": true}
        """;

    private const string InPlaceSignatureJson = """
        {"name": "rotate", "params": [{"name": "nums", "type": "integer[]"}, {"name": "k", "type": "integer"}],
         "return": {"type": "void"}, "output": {"paramindex": 0}}
        """;

    private const string DesignSignatureJson = """
        {"classname": "MinStack", "constructor": {"params": []},
         "methods": [{"name": "push", "params": [{"type": "integer", "name": "value"}], "return": {"type": "void"}},
                     {"name": "getMin", "params": [], "return": {"type": "integer"}}],
         "return": {"type": "boolean"}, "systemdesign": true}
        """;

    [Fact]
    public void SignatureOf_FunctionWithAJudgeOnlyInput_KeepsEveryJudgeInputAndTheCustomJudgeFlag()
    {
        var signature = LeetCodeQuestionMapper.SignatureOf(FunctionSignatureJson);

        Assert.Equal("hasCycle", signature.FunctionName);
        Assert.Equal([new LeetCodeParameter("head", "ListNode"), new LeetCodeParameter("pos", "integer")], signature.Parameters);
        Assert.Equal("boolean", signature.ReturnType);
        Assert.True(signature.HasCustomJudge);
        Assert.False(signature.IsDesign);
    }

    [Fact]
    public void SignatureOf_InPlaceFunction_NamesTheInputTheJudgeReadsBack()
        => Assert.Equal(0, LeetCodeQuestionMapper.SignatureOf(InPlaceSignatureJson).OutputParameterIndex);

    [Fact]
    public void SignatureOf_DesignClass_ReadsTheClassAndNoFunction()
    {
        var signature = LeetCodeQuestionMapper.SignatureOf(DesignSignatureJson);

        Assert.True(signature.IsDesign);
        Assert.Equal("MinStack", signature.ClassName);
        Assert.Null(signature.FunctionName);
        Assert.Null(signature.ReturnType);
    }

    [Fact]
    public void SignatureOf_DesignClass_ReadsEachMethodsNameAndReturnType()
    {
        var methods = LeetCodeQuestionMapper.SignatureOf(DesignSignatureJson).Methods;

        Assert.Equal([("push", "void"), ("getMin", "integer")], methods.Select(method => (method.Name, method.ReturnType)));
    }

    [Fact]
    public void ExamplesOf_OneOutputPerExample_PairsThemByPosition()
    {
        var examples = LeetCodeQuestionMapper.ExamplesOf(["[2,7,11,15]\n9", "[3,3]\n6"], ["[0,1]", "[1,0]"]);

        Assert.Equal(["[2,7,11,15]", "9"], examples[0].Inputs);
        Assert.Equal(["[0,1]", "[1,0]"], examples.Select(example => example.ExpectedOutput));
    }

    [Fact]
    public void ExamplesOf_OutputCountDiffersFromExampleCount_PairsNone()
    {
        var examples = LeetCodeQuestionMapper.ExamplesOf(["[2,7,11,15]\n9", "[3,3]\n6"], ["[0,1]"]);

        Assert.All(examples, example => Assert.Null(example.ExpectedOutput));
    }

    [Fact]
    public void ExamplesOf_OutputStatedInProse_KeepsThatExampleWithNoOutput()
    {
        var examples = LeetCodeQuestionMapper.ExamplesOf(["[3,2,0,-4]\n1", "[1]\n-1"], ["tail connects to node index 1", "false"]);

        Assert.Equal([null, "false"], examples.Select(example => example.ExpectedOutput));
    }

    [Fact]
    public void ToQuestion_ResponseForTwoSum_ReadsIdentityFilingAndStatement()
    {
        var question = LeetCodeQuestionMapper.ToQuestion(TwoSumResponse());

        Assert.Equal((1, 1, "two-sum", LeetCodeDifficulty.Easy, "Algorithms"), (question.QuestionId, question.FrontendId, question.TitleSlug, question.Difficulty, question.Category));
        Assert.Equal([new LeetCodeTopic("Array", "array")], question.Topics);
        Assert.Equal([new LeetCodeRelatedQuestion("3Sum", "3sum", LeetCodeDifficulty.Medium)], question.SimilarQuestions);
        Assert.Equal(["2 <= nums.length <= 10^4"], question.Constraints);
        Assert.Equal("public class Solution {}", question.CSharpStarterCode);
    }

    [Fact]
    public void ToQuestion_ResponseForTwoSum_PairsItsExampleWithTheStatedOutput()
    {
        var example = Assert.Single(LeetCodeQuestionMapper.ToQuestion(TwoSumResponse()).Examples);

        Assert.Equal(["[2,7,11,15]", "9"], example.Inputs);
        Assert.Equal("[0,1]", example.ExpectedOutput);
    }

    [Fact]
    public void ToStatistics_ResponseForTwoSum_ReadsTheRateTotalsAndVotes()
    {
        var statistics = LeetCodeQuestionMapper.ToStatistics(TwoSumResponse());

        Assert.Equal(new LeetCodeStatistics(58.08, 24_000_000, 41_300_000, 70_000, 2_600), statistics);
    }

    // A trimmed copy of what leetcode.com returns for Two Sum, its nested documents included.
    private static LeetCodeQuestionDto TwoSumResponse()
        => new()
        {
            QuestionId = "1",
            QuestionFrontendId = "1",
            Title = "Two Sum",
            TitleSlug = "two-sum",
            Content = """
                <pre>
                <strong>Input:</strong> nums = [2,7,11,15], target = 9
                <strong>Output:</strong> [0,1]
                </pre>
                <p><strong>Constraints:</strong></p>
                <ul>
                	<li><code>2 &lt;= nums.length &lt;= 10<sup>4</sup></code></li>
                </ul>
                """,
            Difficulty = "Easy",
            CategoryTitle = "Algorithms",
            Likes = 70_000,
            Dislikes = 2_600,
            IsPaidOnly = false,
            AcRate = 58.08,
            Stats = """{"totalAcceptedRaw": 24000000, "totalSubmissionRaw": 41300000, "acRate": "58.1%"}""",
            Hints = [],
            MetaData = FunctionSignatureJson,
            ExampleTestcaseList = ["[2,7,11,15]\n9"],
            SimilarQuestions = """[{"title": "3Sum", "titleSlug": "3sum", "difficulty": "Medium", "translatedTitle": null}]""",
            TopicTags = [new LeetCodeTopicTagDto { Name = "Array", Slug = "array" }],
            CodeSnippets = [new LeetCodeCodeSnippetDto { LangSlug = "csharp", Code = "public class Solution {}" }],
        };
}
