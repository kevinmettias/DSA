using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

public sealed partial class DatasetCaseConverterTests
{
    private static readonly LeetCodeQuestion TwoSum = QuestionWith(
        [new("nums", "integer[]"), new("target", "integer")],
        "integer[]",
        ["2 <= nums.length <= 10^4", "-10^9 <= nums[i] <= 10^9"],
        [new LeetCodeCase { Inputs = ["[2,7,11,15]", "9"], ExpectedOutput = "[0,1]" }]);

    public static TheoryData<string, string?, string> Dropped =>
        new()
        {
            { "nums = [3,3], target = 6", "Error: list index out of range", nameof(DatasetDropReason.ReferenceFailed) },
            { "nums = [3,3], target = 6", "Execution timed out", nameof(DatasetDropReason.ReferenceFailed) },
            { "nums = [3,3], target = 6", null, nameof(DatasetDropReason.ReferenceFailed) },
            { "[3,3]", "[0, 1]", nameof(DatasetDropReason.InputNotSplit) },
            { "values = [3,3], target = 6", "[0, 1]", nameof(DatasetDropReason.NamesDiffer) },
            { "nums = {3, 3}, target = 6", "[0, 1]", nameof(DatasetDropReason.NotTranslatable) },
            { "nums = [-1,-2], target = 5", "None", nameof(DatasetDropReason.NoAnswer) },
            { "nums = [3, 1.5], target = 6", "[0, 1]", nameof(DatasetDropReason.NotOfDeclaredType) },
            { "nums = [3], target = 6", "[0, 0]", nameof(DatasetDropReason.OutOfBounds) },
            { "nums = [2, 7, 11, 15], target = 9", "[0, 1]", nameof(DatasetDropReason.Duplicate) },
        };

    [Theory]
    [MemberData(nameof(Dropped))]
    public void Convert_CaseThatPosesADifferentQuestion_DropsItForItsReason(string input, string? output, string reason)
    {
        var conversion = new DatasetCaseConverter(TwoSum).Convert([new DatasetCaseDto { Input = input, Output = output }]);

        Assert.Empty(conversion.Cases);
        Assert.Equal((reason, 1), (conversion.Dropped.Keys.Single().ToString(), conversion.Dropped.Values.Single()));
    }

    [Fact]
    public void Convert_PythonLiterals_KeepsTheCaseInLeetCodeNotationInSignatureOrder()
    {
        var conversion = new DatasetCaseConverter(TwoSum).Convert([new DatasetCaseDto { Input = "target = 6, nums = [3, 3]", Output = "[0, 1]" }]);

        var kept = Assert.Single(conversion.Cases);
        Assert.Equal(["[3,3]", "6"], kept.Inputs);
        Assert.Equal("[0,1]", kept.ExpectedOutput);
        Assert.Equal(LeetCodeCaseSource.Dataset, kept.Source);
    }

    [Fact]
    public void Convert_SameInputsTwice_KeepsTheFirst()
    {
        var repeated = new DatasetCaseDto { Input = "nums = [1,2], target = 3", Output = "[0, 1]" };

        Assert.Single(new DatasetCaseConverter(TwoSum).Convert([repeated, repeated]).Cases);
    }

    [Fact]
    public void Convert_NoneForATreeAnswer_KeepsItAsTheEmptyTree()
    {
        var question = QuestionWith([new("root", "TreeNode")], "TreeNode", [], []);

        var kept = new DatasetCaseConverter(question).Convert([new DatasetCaseDto { Input = "root = [1]", Output = "None" }]).Cases;

        Assert.Equal("[]", Assert.Single(kept).ExpectedOutput);
    }

    [Fact]
    public void Convert_NoneForAnInPlaceAnswer_KeepsTheInputsWithNoAnswer()
    {
        var question = QuestionWith([new("nums", "integer[]"), new("k", "integer")], "void", [], []);

        var kept = new DatasetCaseConverter(question).Convert([new DatasetCaseDto { Input = "nums = [1,2,3], k = 1", Output = "None" }]).Cases;

        Assert.False(Assert.Single(kept).HasExpectedOutput);
    }

    [Fact]
    public void Convert_StringAnswersPrintedAsText_EncodesEachAsAString()
    {
        var question = QuestionWith([new("piles", "integer[]")], "string", [], []);

        var kept = new DatasetCaseConverter(question).Convert(
            [
                new DatasetCaseDto { Input = "piles = [1,2]", Output = "Alice" },
                new DatasetCaseDto { Input = "piles = [3]", Output = string.Empty },
            ]).Cases;

        Assert.Equal(["\"Alice\"", "\"\""], kept.Select(testCase => testCase.ExpectedOutput));
    }

    [Fact]
    public void Convert_NoneForAStringAnswer_DropsItAsNoAnswer()
    {
        var question = QuestionWith([new("piles", "integer[]")], "string", [], []);

        var conversion = new DatasetCaseConverter(question).Convert([new DatasetCaseDto { Input = "piles = [1,2]", Output = "None" }]);

        Assert.Equal(1, conversion.Dropped[DatasetDropReason.NoAnswer]);
    }

    private static LeetCodeQuestion QuestionWith(
        IReadOnlyList<LeetCodeParameter> parameters,
        string returnType,
        IReadOnlyList<string> constraints,
        IReadOnlyList<LeetCodeCase> examples)
        => new()
        {
            QuestionId = 1,
            FrontendId = 1,
            Title = "Fixture",
            TitleSlug = "fixture",
            Difficulty = LeetCodeDifficulty.Easy,
            Category = "Algorithms",
            IsPaidOnly = false,
            Topics = [],
            SimilarQuestions = [],
            Hints = [],
            Constraints = constraints,
            Signature = new LeetCodeSignature
            {
                FunctionName = "fixture",
                Parameters = parameters,
                ReturnType = returnType,
                ConstructorParameters = [],
                Methods = [],
            },
            CSharpStarterCode = null,
            Examples = examples,
            ContentHtml = string.Empty,
        };
}
