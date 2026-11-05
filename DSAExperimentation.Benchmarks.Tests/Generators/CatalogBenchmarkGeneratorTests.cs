using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class CatalogBenchmarkGeneratorTests
{
    // The file the generator writes for the probe's ProbeSolution.
    private const string ProbeClassFile = "ProbeCatalogBenchmarks.g.cs";

    // The diagnostic the generator reports for a strategy no arm can call.
    private const string UnboundStrategy = "DSACAT001";

    // An in-place arm's call, then the answer it stores: the input the call left. And the mark of the setup
    // that copies an arm's inputs before each iteration.
    private const string InPlaceCall = "global::ProbeSolution.SortByInsertion(input0[index]);\n            answers[index] = input0[index];";
    private const string CopySetup = "IterationSetup";

    private const string EnrolledProbe = """
        using DSAExperimentation.LeetCode;

        namespace DSAExperimentation.LeetCode.Solutions.Probe
        {
            [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
            internal static class ProbeSolution
            {
                public static int CountByLoop(int[] values) => values.Length;

                public static int CountByRecursion(int[] values) => values.Length;

                public static int Halve(int value) => value / 2;
            }
        }
        """;

    private const string RewritingProbe = """
        using DSAExperimentation.LeetCode;

        namespace DSAExperimentation.LeetCode.Solutions.Probe
        {
            [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten, RewritesInput = true)]
            internal static class ProbeSolution
            {
                public static int CountByLoop(int[] values) => values.Length;
            }
        }
        """;

    private const string AnsweringInPlaceProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten, AnswersInPlace = true)]
        internal static class ProbeSolution
        {
            public static void SortByInsertion(int[] values) { }
        }
        """;

    // A strategy with a hoisted overload the notation reads too, as 17.4 allows beside LeetCode's shape, and
    // the question's file that declares which shape is LeetCode's.
    private const string TwoShapeProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
        internal static class ProbeSolution
        {
            public static int CountByLoop(int[][] intervals) => intervals.Length;

            public static int CountByLoop((int Start, int End)[] intervals) => intervals.Length;
        }
        """;

    private const string TwoShapeQuestion = """
        { "Signature": { "FunctionName": "count", "Parameters": [ { "Name": "intervals", "Type": "integer[][]" } ], "ReturnType": "integer" } }
        """;

    private const string ProbeSlug = "probe";

    private const string InPlaceProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
        internal static class ProbeSolution
        {
            public static void SortByInsertion(int[] values) { }
        }
        """;

    // A strategy that calls a static local function, which its assembly's metadata holds as an internal method
    // named for the strategy around it - one no arm could store an answer from, since it takes a ref.
    private const string LocalFunctionProbe = """
        using DSAExperimentation.LeetCode;

        namespace DSAExperimentation.LeetCode.Solutions.Probe
        {
            [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
            internal static class ProbeSolution
            {
                public static int CountByHelper(int[] values)
                {
                    var total = 0;

                    foreach (var value in values)
                    {
                        Add(ref total);
                    }

                    return total;

                    static void Add(ref int total) => total++;
                }
            }
        }
        """;

    [Fact]
    public void Initialize_StrategyCallingALocalFunctionInMetadata_ReportsNoArmForTheLocalFunction() =>
        Assert.DoesNotContain(GeneratorProbe.GenerateFromEmittedReferences(LocalFunctionProbe).Diagnostics, diagnostic => diagnostic.Id == UnboundStrategy);

    [Fact]
    public void Initialize_EnrolledSolution_WritesOneClassNamedForItsProblem() =>
        Assert.Equal([ProbeClassFile], GeneratorProbe.Generate(EnrolledProbe).GeneratedTrees.Select(tree => Path.GetFileName(tree.FilePath)));

    // The real solutions sit in an assembly above the conventions that declare the attribute.
    [Fact]
    public void Initialize_SolutionInAnAssemblyAboveTheConventions_WritesItsClass() =>
        Assert.Equal(
            [ProbeClassFile],
            GeneratorProbe.GenerateFromReferences(EnrolledProbe).GeneratedTrees.Select(tree => Path.GetFileName(tree.FilePath)));

    [Fact]
    public void Initialize_EnrolledSolution_MakesTheFirstStrategyTheBaseline() =>
        Assert.Contains(
            "[global::BenchmarkDotNet.Attributes.Benchmark(Baseline = true)]\n    public int[] CountByLoop()",
            SourceOf(EnrolledProbe),
            StringComparison.Ordinal);

    [Fact]
    public void Initialize_EnrolledSolution_WritesAnArmForEveryLaterStrategy() =>
        Assert.Contains(
            "[global::BenchmarkDotNet.Attributes.Benchmark]\n    public int[] CountByRecursion()",
            SourceOf(EnrolledProbe),
            StringComparison.Ordinal);

    // Halve is a helper: no `By` names a strategy in it.
    [Fact]
    public void Initialize_EnrolledSolution_WritesNoArmForAHelper() =>
        Assert.DoesNotContain("Halve", SourceOf(EnrolledProbe), StringComparison.Ordinal);

    // Its strategies may rewrite their inputs, so each call spends a copy read before its iteration.
    [Fact]
    public void Initialize_SolutionThatRewritesInput_FixesAnIterationsCallsToItsCopies() =>
        Assert.Contains(
            "[global::BenchmarkDotNet.Attributes.InvocationCount(ProbeCatalogBenchmarks.CopiesPerIteration, 1)]\npublic class ProbeCatalogBenchmarks",
            SourceOf(RewritingProbe),
            StringComparison.Ordinal);

    [Fact]
    public void Initialize_SolutionThatRewritesInput_CopiesEachArmsInputsBeforeEachIteration() =>
        Assert.Contains(
            "[global::BenchmarkDotNet.Attributes.IterationSetup(Target = nameof(CountByLoop))]\n    public void CopyCountByLoopInputs()",
            SourceOf(RewritingProbe),
            StringComparison.Ordinal);

    [Fact]
    public void Initialize_SolutionThatRewritesInput_SpendsOneCopyPerCall() =>
        Assert.Contains(
            "var copy = _countByLoopCopy++;\n        var input0 = _countByLoopInput0Copies[copy];",
            SourceOf(RewritingProbe),
            StringComparison.Ordinal);

    [Fact]
    public void Initialize_SolutionThatKeepsItsInput_ReadsItsCasesOnce() =>
        Assert.DoesNotContain("IterationSetup", SourceOf(EnrolledProbe), StringComparison.Ordinal);

    [Fact]
    public void Initialize_StrategyAnsweringInPlace_ReportsItRatherThanLeavingItOut() =>
        Assert.Contains(GeneratorProbe.Generate(InPlaceProbe).Diagnostics, diagnostic => diagnostic.Id == UnboundStrategy);

    [Fact]
    public void Initialize_SolutionAnsweringInPlace_StoresEachCallsFirstInputAsItsAnswer() =>
        Assert.Contains(InPlaceCall, SourceOf(AnsweringInPlaceProbe), StringComparison.Ordinal);

    [Fact]
    public void Initialize_SolutionAnsweringInPlace_CopiesEachArmsInputsBeforeEachIteration() =>
        Assert.Contains(CopySetup, SourceOf(AnsweringInPlaceProbe), StringComparison.Ordinal);

    [Fact]
    public void Initialize_TwoShapesWithTheQuestionsFile_BindsLeetCodesShape() =>
        Assert.DoesNotContain(GeneratorProbe.GenerateWithQuestion(TwoShapeProbe, ProbeSlug, TwoShapeQuestion).Diagnostics, diagnostic => diagnostic.Id == UnboundStrategy);

    [Fact]
    public void Initialize_TwoShapesWithNoQuestionsFile_ReportsTheChoiceUnsettled() =>
        Assert.Contains(GeneratorProbe.Generate(TwoShapeProbe).Diagnostics, diagnostic => diagnostic.Id == UnboundStrategy);

    [Fact]
    public void Initialize_NoSolutionNamesAQuestion_WritesNothing() =>
        Assert.Empty(GeneratorProbe.Generate("internal static class ProbeSolution { public static int CountByLoop(int[] values) => 0; }").GeneratedTrees);

    private static string SourceOf(string probe) => GeneratorProbe.Generate(probe).GeneratedTrees.Single().ToString();
}
