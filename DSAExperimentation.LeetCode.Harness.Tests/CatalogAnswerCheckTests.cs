using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogAnswerCheckTests
{
    private static readonly Type Check = typeof(SameParityProbeAnswerCheck);

    private static readonly LeetCodeCase Judged = new() { Inputs = ["[5,6,7,8]"], ExpectedOutput = "2" };

    private static readonly LeetCodeCase Unjudged = new() { Inputs = ["[5,6,7,8]"] };

    [Fact]
    public void IsAccepted_AnswerOtherThanTheRecordedOneThatTheCheckTakes_IsTrue()
    {
        var isAccepted = CatalogAnswerCheck.IsAccepted(Check, Judged, JsonValue.Create(4), JsonValue.Create(2));

        Assert.True(isAccepted);
    }

    [Fact]
    public void IsAccepted_AnswerTheCheckRefusesForTheCasesInput_IsFalse()
    {
        var isAccepted = CatalogAnswerCheck.IsAccepted(Check, Judged, JsonValue.Create(6), JsonValue.Create(2));

        Assert.False(isAccepted);
    }

    [Fact]
    public void IsAccepted_TypeWithNoIsAcceptedMethod_Throws() =>
        Assert.Throws<InvalidOperationException>(() => CatalogAnswerCheck.IsAccepted(typeof(CountingProbeSolution), Judged, JsonValue.Create(4), JsonValue.Create(2)));

    [Fact]
    public void VerdictTextOf_TwoAnswersTheCheckTakes_WritesTheSameText()
    {
        var recordedText = CatalogAnswerCheck.VerdictTextOf(Check, Judged, JsonValue.Create(2));
        var otherText = CatalogAnswerCheck.VerdictTextOf(Check, Judged, JsonValue.Create(4));

        Assert.Equal(recordedText, otherText);
    }

    [Fact]
    public void VerdictTextOf_AnswerTheCheckRefuses_WritesDifferentText()
    {
        var acceptedText = CatalogAnswerCheck.VerdictTextOf(Check, Judged, JsonValue.Create(2));
        var refusedText = CatalogAnswerCheck.VerdictTextOf(Check, Judged, JsonValue.Create(3));

        Assert.NotEqual(acceptedText, refusedText);
    }

    [Fact]
    public void VerdictTextOf_CaseWithNoRecordedOutput_WritesOneTextForEveryAnswer()
    {
        var evenText = CatalogAnswerCheck.VerdictTextOf(Check, Unjudged, JsonValue.Create(2));
        var oddText = CatalogAnswerCheck.VerdictTextOf(Check, Unjudged, JsonValue.Create(3));

        Assert.Equal(evenText, oddText);
    }
}
