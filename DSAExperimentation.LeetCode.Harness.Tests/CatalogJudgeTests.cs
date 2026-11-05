using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogJudgeTests
{
    private const string Arm = "CountByLoop";

    // LeetCode's usual tolerance for a real-valued answer.
    private const double Tolerance = 1e-5;

    // A question whose answer is compared as written, as the judge reads one off its attribute.
    private static readonly LeetCodeQuestionAttribute AsWritten = new("probe", LeetCodeAnswerOrder.AsWritten);

    private static readonly LeetCodeCase[] Cases =
    [
        new() { Inputs = ["[1,2]"], ExpectedOutput = "2" },
        new() { Inputs = ["[1,2,3]"], ExpectedOutput = "3" },
    ];

    // A question whose statement lets several answers stand, judged by its answer check.
    private static readonly LeetCodeQuestionAttribute Checked = new("probe", LeetCodeAnswerOrder.AsWritten) { AnswerCheck = typeof(SameParityProbeAnswerCheck) };

    [Fact]
    public void WrongAnswers_AnswersOtherThanTheRecordedOnesThatTheCheckTakes_IsEmpty()
    {
        var wrong = CatalogJudge.WrongAnswers(Arm, Checked, Cases, index => JsonValue.Create(index));

        Assert.Empty(wrong);
    }

    [Fact]
    public void WrongAnswers_AnswerTheCheckRefuses_NamesItsInputAndBothAnswers()
    {
        var wrong = CatalogJudge.WrongAnswers(Arm, Checked, Cases, index => JsonValue.Create(index * 2 + 1));

        Assert.Equal(["CountByLoop on [1,2]: the catalog records 2, and it answered 1."], wrong);
    }

    [Fact]
    public void RecordedOutputOf_CaseWithNoRecordedOutput_IsNull()
    {
        var recorded = CatalogJudge.RecordedOutputOf(new LeetCodeCase { Inputs = ["[1]"] });

        Assert.Null(recorded);
    }

    [Fact]
    public void WrongAnswers_EveryCaseAnsweredAsRecorded_IsEmpty() =>
        Assert.Empty(CatalogJudge.WrongAnswers(Arm, AsWritten, Cases, index => JsonValue.Create(index + 2)));

    [Fact]
    public void WrongAnswers_OneCaseAnsweredWrongly_NamesItsInputAndBothAnswers() =>
        Assert.Equal(
            ["CountByLoop on [1,2,3]: the catalog records 3, and it answered 4."],
            CatalogJudge.WrongAnswers(Arm, AsWritten, Cases, index => JsonValue.Create(index * 2 + 2)));

    [Fact]
    public void IsAccepted_NumbersWithinTheTolerance_Accepts() =>
        Assert.True(CatalogJudge.IsAccepted(NodeOf("[36.86989764584401,90]"), NodeOf("[36.86990,90.00000]"), LeetCodeAnswerOrder.AsWritten, Tolerance));

    [Fact]
    public void IsAccepted_NumberPastTheTolerance_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("0.15002"), NodeOf("0.15"), LeetCodeAnswerOrder.AsWritten, Tolerance));

    [Fact]
    public void IsAccepted_ExactQuestionNumbersApart_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("2.0000000000000004"), NodeOf("2.00000"), LeetCodeAnswerOrder.AsWritten));

    [Fact]
    public void AgreementTextOf_NumbersApartInTheLastBits_WritesTheSameTextUnderATolerance() =>
        Assert.Equal(
            CatalogJudge.AgreementTextOf(NodeOf("[2.0000000000000004]"), LeetCodeAnswerOrder.AsWritten, Tolerance),
            CatalogJudge.AgreementTextOf(NodeOf("[2]"), LeetCodeAnswerOrder.AsWritten, Tolerance));

    // Pow(x, n)'s two arms on one case: a million multiplications against squarings, apart in the eleventh
    // digit, where twelve significant digits would write 4.54021997985E-05 for one and 4.5402199798E-05 for
    // the other.
    [Fact]
    public void AgreementTextOf_NumbersApartInTheEleventhDigit_WritesTheSameTextUnderATolerance()
    {
        var bySquaring = CatalogJudge.AgreementTextOf(NodeOf("[4.540219979849E-05]"), LeetCodeAnswerOrder.AsWritten, Tolerance);
        var byMultiplying = CatalogJudge.AgreementTextOf(NodeOf("[4.540219979801E-05]"), LeetCodeAnswerOrder.AsWritten, Tolerance);

        Assert.Equal(bySquaring, byMultiplying);
    }

    [Fact]
    public void IsAccepted_EachInAnyOrderRowReordered_Accepts() =>
        Assert.True(CatalogJudge.IsAccepted(NodeOf("[[0,1,2,6],[]]"), NodeOf("[[0,6,2,1],[]]"), LeetCodeAnswerOrder.EachInAnyOrder));

    [Fact]
    public void IsAccepted_EachInAnyOrderRowsSwapped_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("[[],[0,1]]"), NodeOf("[[0,1],[]]"), LeetCodeAnswerOrder.EachInAnyOrder));

    [Fact]
    public void IsAccepted_AnyOrderThroughoutReorderedAtBothDepths_Accepts() =>
        Assert.True(CatalogJudge.IsAccepted(NodeOf("[[\"eat\",\"tea\"],[\"bat\"]]"), NodeOf("[[\"bat\"],[\"tea\",\"eat\"]]"), LeetCodeAnswerOrder.AnyOrderThroughout));

    [Fact]
    public void IsAccepted_AnyOrderInnerReordered_Refuses() =>
        Assert.False(CatalogJudge.IsAccepted(NodeOf("[[3,1]]"), NodeOf("[[1,3]]"), LeetCodeAnswerOrder.AnyOrder));

    [Fact]
    public void AgreementTextOf_AnyOrderThroughoutReordered_WritesTheSameText() =>
        Assert.Equal(
            CatalogJudge.AgreementTextOf(NodeOf("[[1,3],[0,1]]"), LeetCodeAnswerOrder.AnyOrderThroughout),
            CatalogJudge.AgreementTextOf(NodeOf("[[1,0],[3,1]]"), LeetCodeAnswerOrder.AnyOrderThroughout));

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
        Assert.Empty(CatalogJudge.WrongAnswers(Arm, AsWritten, [new() { Inputs = ["[1]"] }], _ => JsonValue.Create(-1)));

    [Fact]
    public void AgreementTextOf_AnyOrderSequenceReordered_WritesTheSameText() =>
        Assert.Equal(
            CatalogJudge.AgreementTextOf(NodeOf("[[0,1],[2]]"), LeetCodeAnswerOrder.AnyOrder),
            CatalogJudge.AgreementTextOf(NodeOf("[[2],[0,1]]"), LeetCodeAnswerOrder.AnyOrder));

    [Fact]
    public void AgreementTextOf_AsWrittenSequenceReordered_WritesDifferentText() =>
        Assert.NotEqual(
            CatalogJudge.AgreementTextOf(NodeOf("[0,1]"), LeetCodeAnswerOrder.AsWritten),
            CatalogJudge.AgreementTextOf(NodeOf("[1,0]"), LeetCodeAnswerOrder.AsWritten));

    // Every text a test writes is a JSON value, never a bare null.
    private static JsonNode NodeOf(string text) => JsonNode.Parse(text) ?? throw new InvalidOperationException($"{text} is a bare null.");
}
