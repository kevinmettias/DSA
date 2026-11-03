using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogJudgeTests
{
    private const string Arm = "CountByLoop";

    private static readonly LeetCodeCase[] Cases =
    [
        new() { Inputs = ["[1,2]"], ExpectedOutput = "2" },
        new() { Inputs = ["[1,2,3]"], ExpectedOutput = "3" },
    ];

    [Fact]
    public void WrongAnswers_EveryCaseAnsweredAsRecorded_IsEmpty() =>
        Assert.Empty(CatalogJudge.WrongAnswers(Arm, LeetCodeAnswerOrder.AsWritten, Cases, index => JsonValue.Create(index + 2)));

    [Fact]
    public void WrongAnswers_OneCaseAnsweredWrongly_NamesItsInputAndBothAnswers() =>
        Assert.Equal(
            ["CountByLoop on [1,2,3]: the catalog records 3, and it answered 4."],
            CatalogJudge.WrongAnswers(Arm, LeetCodeAnswerOrder.AsWritten, Cases, index => JsonValue.Create(index * 2 + 2)));

    [Fact]
    public void IsAccepted_AnyOrderSequenceReordered_Accepts() =>
        Assert.True(CatalogJudge.IsAccepted(NodeOf("[1,0]"), NodeOf("[0,1]"), LeetCodeAnswerOrder.AnyOrder));

    [Fact]
    public void IsAccepted_AsWrittenSequenceReordered_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("[1,0]"), NodeOf("[0,1]"), LeetCodeAnswerOrder.AsWritten));

    // A row's own items keep their order even when the rows may come back in any order.
    [Fact]
    public void IsAccepted_AnyOrderRowReversed_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("[[2,1]]"), NodeOf("[[1,2]]"), LeetCodeAnswerOrder.AnyOrder));

    // A number has no order to free, whatever the question says.
    [Fact]
    public void IsAccepted_AnyOrderNumber_ComparesItAsWritten() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("3"), NodeOf("4"), LeetCodeAnswerOrder.AnyOrder));

    // A case with no recorded output is still timed, but no answer to it is vouched for.
    [Fact]
    public void WrongAnswers_CaseWithNoRecordedOutput_IsNotJudged() =>
        Assert.Empty(CatalogJudge.WrongAnswers(Arm, LeetCodeAnswerOrder.AsWritten, [new() { Inputs = ["[1]"] }], _ => JsonValue.Create(-1)));

    // Every text a test writes is a JSON value, never a bare null.
    private static JsonNode NodeOf(string text) => JsonNode.Parse(text) ?? throw new InvalidOperationException($"{text} is a bare null.");
}
