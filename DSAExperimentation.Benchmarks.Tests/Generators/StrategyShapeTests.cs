using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class StrategyShapeTests
{
    // Holder's strategy that answers in place, one that returns its answer, what the first answers with,
    // and the words a refusal of the second, for an in-place question, says.
    private const string InPlaceStrategy = "SortByInsertion";
    private const string ValueStrategy = "CountByLoop";
    private const string InPlaceAnswerType = "int[]";
    private const string InPlaceRefusal = "answers in place";

    // Holder's Try-shaped strategy, the tuple it answers with, and the inputs a case fills for it.
    private const string TryShapedStrategy = "TryFindByScan";
    private const string TryShapedAnswerType = "(bool, int, int)";
    private static readonly string[] TryShapedInputs = ["int[]", "int"];

    private const string Holder = """
        internal static class Holder
        {
            public static bool TryFindByScan(int[] nums, int target, out int first, out int second) { first = 0; second = 0; return false; }

            public static int CountByLoop(int[] values) => values.Length;

            public static void SortByInsertion(int[] values) { }

            public static int SumByLoop<T>(T[] values) => 0;

            public static int CountByWalk(System.Collections.Generic.Dictionary<int, int> counts) => 0;

            public static int CountByRef(ref int value) => value;
        }
        """;

    public static TheoryData<string, string> Refusals => new()
    {
        { "SortByInsertion", "in place" },
        { "SumByLoop", "generic" },
        { "CountByWalk", "no reading" },
        { "CountByRef", "by reference" },
    };

    [Fact]
    public void Of_TryShapedStrategy_AnswersItsReturnValueAndOutputsAsOneTuple() =>
        Assert.Equal(TryShapedAnswerType, StrategyShape.Of(GeneratorProbe.Method(Holder, TryShapedStrategy), AnswerSource.ReturnValue).AnswerType);

    [Fact]
    public void Of_TryShapedStrategy_ReadsOnlyItsInputsFromACase() =>
        Assert.Equal(TryShapedInputs, StrategyShape.Of(GeneratorProbe.Method(Holder, TryShapedStrategy), AnswerSource.ReturnValue).Inputs);

    [Fact]
    public void Of_TryShapedStrategy_KeepsEveryParameterInDeclarationOrder() =>
        Assert.Equal([false, false, true, true], StrategyShape.Of(GeneratorProbe.Method(Holder, TryShapedStrategy), AnswerSource.ReturnValue).Parameters.Select(parameter => parameter.IsOutput));

    [Fact]
    public void Of_StrategyReturningAValue_IsNamedForItsMethod() =>
        Assert.Equal(ValueStrategy, StrategyShape.Of(GeneratorProbe.Method(Holder, ValueStrategy), AnswerSource.ReturnValue).Name);

    [Fact]
    public void ReasonUnbound_StrategyAnsweringInPlaceForAnInPlaceQuestion_IsNull()
    {
        var reason = StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, InPlaceStrategy), AnswerSource.FirstInput);

        Assert.Null(reason);
    }

    [Fact]
    public void ReasonUnbound_StrategyReturningAValueForAnInPlaceQuestion_SaysTheQuestionAnswersInPlace()
    {
        var reason = StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, ValueStrategy), AnswerSource.FirstInput);

        Assert.Contains(InPlaceRefusal, reason, StringComparison.Ordinal);
    }

    [Fact]
    public void Of_StrategyAnsweringInPlace_AnswersWithItsFirstInputsType()
    {
        var shape = StrategyShape.Of(GeneratorProbe.Method(Holder, InPlaceStrategy), AnswerSource.FirstInput);

        Assert.Equal(InPlaceAnswerType, shape.AnswerType);
    }

    [Fact]
    public void ReasonUnbound_StrategyReturningAValue_IsNull() =>
        Assert.Null(StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, ValueStrategy), AnswerSource.ReturnValue));

    [Theory]
    [MemberData(nameof(Refusals))]
    public void ReasonUnbound_StrategyNoArmCanCall_SaysWhy(string method, string reason) =>
        Assert.Contains(reason, StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, method), AnswerSource.ReturnValue), StringComparison.Ordinal);
}
