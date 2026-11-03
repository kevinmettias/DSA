using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class StrategyShapeTests
{
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
        Assert.Equal("(bool, int, int)", StrategyShape.Of(GeneratorProbe.Method(Holder, "TryFindByScan")).AnswerType);

    [Fact]
    public void Of_TryShapedStrategy_ReadsOnlyItsInputsFromACase() =>
        Assert.Equal(["int[]", "int"], StrategyShape.Of(GeneratorProbe.Method(Holder, "TryFindByScan")).Inputs);

    [Fact]
    public void Of_TryShapedStrategy_KeepsEveryParameterInDeclarationOrder() =>
        Assert.Equal([false, false, true, true], StrategyShape.Of(GeneratorProbe.Method(Holder, "TryFindByScan")).Parameters.Select(parameter => parameter.IsOutput));

    [Fact]
    public void Of_StrategyReturningAValue_IsNamedForItsMethod() =>
        Assert.Equal("CountByLoop", StrategyShape.Of(GeneratorProbe.Method(Holder, "CountByLoop")).Name);

    [Fact]
    public void ReasonUnbound_StrategyReturningAValue_IsNull() =>
        Assert.Null(StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, "CountByLoop")));

    [Theory]
    [MemberData(nameof(Refusals))]
    public void ReasonUnbound_StrategyNoArmCanCall_SaysWhy(string method, string reason) =>
        Assert.Contains(reason, StrategyShape.ReasonUnbound(GeneratorProbe.Method(Holder, method)), StringComparison.Ordinal);
}
