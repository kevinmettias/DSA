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
    private const string TryInPlaceCall = "global::ProbeSolution.TrySortByInsertion(input0[index]);\n            answers[index] = input0[index];";
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

    // An in-place strategy written as a Try method, returning whether it succeeded, which LeetCode never reads.
    private const string TryAnsweringInPlaceProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten, AnswersInPlace = true)]
        internal static class ProbeSolution
        {
            public static bool TrySortByInsertion(int[] values) => true;
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

    // What the generated class of DesignProbe holds: an arm returning its calls' answers; put, which the
    // map answers itself; get on the cache, through the Calls type with the instance first; each case's
    // instance made by the arm's factory; and the answer buffers as one list, put's slot empty.
    private const string DesignArm = "public global::DSAExperimentation.LeetCode.Harness.CatalogCallAnswers CreateByMap()";
    private const string OwnMethodCall = "made.Put(call0Input0[index][call], call0Input1[index][call]);";
    private const string StatedMethodCall = "call1Answers[index][call] = global::ProbeCalls.Get(made, call1Input0[index][call]);";
    private const string FactoryCall = "var made = global::ProbeSolution.CreateByCache(input0[index]);";
    private const string BuffersByMethod = "AnswersFrom([null, _createByMapCall1Answers]);";

    // The refusal for a get no method answers.
    private const string NoGetCall = "no get call";

    // What a design class that copies its inputs writes: an [IterationSetup] per arm, and each call's
    // constructor input read from the copy it spends.
    private const string DesignCopySetup = "[global::BenchmarkDotNet.Attributes.IterationSetup(Target = nameof(CreateByMap))]";
    private const string DesignCopySpent = "var input0 = _createByMapInput0Copies[copy];";

    // DesignProbe whose instances may rewrite the inputs they are handed.
    private static readonly string RewritingDesignProbe =
        DesignProbe.Replace("Calls = typeof(ProbeCalls))]", "Calls = typeof(ProbeCalls), RewritesInput = true)]", StringComparison.Ordinal);

    // A design question's probe: one factory makes a map that answers put and get itself, the other a cache
    // whose get the question's Calls type states, since the cache answers only TryGet.
    private const string DesignProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten, Calls = typeof(ProbeCalls))]
        internal static class ProbeSolution
        {
            public static ProbeMap CreateByMap(int capacity) => new();

            public static ProbeCache CreateByCache(int capacity) => new();
        }

        internal sealed class ProbeMap
        {
            public void Put(int key, int value) { }

            public int Get(int key) => key;
        }

        internal sealed class ProbeCache
        {
            public void Put(int key, int value) { }

            public bool TryGet(int key, out int value)
            {
                value = key;
                return true;
            }
        }

        internal static class ProbeCalls
        {
            public static int Get(ProbeCache cache, int key) => cache.TryGet(key, out var value) ? value : -1;
        }
        """;

    // The same question with no Calls type, so the cache answers no get.
    private const string UncalledDesignProbe = """
        using DSAExperimentation.LeetCode;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
        internal static class ProbeSolution
        {
            public static ProbeCache CreateByCache(int capacity) => new();
        }

        internal sealed class ProbeCache
        {
            public void Put(int key, int value) { }

            public bool TryGet(int key, out int value)
            {
                value = key;
                return true;
            }
        }
        """;

    private const string DesignQuestion = """
        { "Signature": { "FunctionName": null, "Parameters": [], "ReturnType": null, "ClassName": "Probe",
          "ConstructorParameters": [ { "Name": "capacity", "Type": "integer" } ],
          "Methods": [
            { "Name": "put", "Parameters": [ { "Name": "key", "Type": "integer" }, { "Name": "value", "Type": "integer" } ], "ReturnType": "void" },
            { "Name": "get", "Parameters": [ { "Name": "key", "Type": "integer" } ], "ReturnType": "integer" } ] } }
        """;

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
    public void Initialize_TryMethodAnsweringInPlace_StoresEachCallsFirstInputNotItsResult() =>
        Assert.Contains(TryInPlaceCall, SourceOf(TryAnsweringInPlaceProbe), StringComparison.Ordinal);

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
    public void Initialize_DesignSolution_ArmsReturnTheirCallsAnswers() =>
        Assert.Contains(DesignArm, DesignSourceOf(DesignProbe));

    [Fact]
    public void Initialize_DesignSolution_CallsAMethodTheMadeInstanceAnswersOnIt() =>
        Assert.Contains(OwnMethodCall, DesignSourceOf(DesignProbe));

    [Fact]
    public void Initialize_DesignSolution_CallsTheCallsTypesMethodWithTheInstanceFirst() =>
        Assert.Contains(StatedMethodCall, DesignSourceOf(DesignProbe));

    [Fact]
    public void Initialize_DesignSolution_MakesEachCasesInstanceWithTheArmsFactory() =>
        Assert.Contains(FactoryCall, DesignSourceOf(DesignProbe));

    // As one list, a slot per method in the signature's order: put returns nothing, get fills its buffer. A
    // buffer handed alone to a params Array[] would stand for the list itself.
    [Fact]
    public void Initialize_DesignSolution_HandsItsAnswerBuffersAsOneListByMethod() =>
        Assert.Contains(BuffersByMethod, DesignSourceOf(DesignProbe));

    [Fact]
    public void Initialize_DesignSolutionThatRewritesInput_CopiesEachArmsInputsBeforeEachIteration() =>
        Assert.Contains(DesignCopySetup, DesignSourceOf(RewritingDesignProbe));

    [Fact]
    public void Initialize_DesignSolutionThatRewritesInput_SpendsOneCopyPerCall() =>
        Assert.Contains(DesignCopySpent, DesignSourceOf(RewritingDesignProbe));

    [Fact]
    public void Initialize_DesignInstanceAnsweringNoCall_ReportsTheCallItCannotMake() =>
        Assert.Contains(
            GeneratorProbe.GenerateWithQuestion(UncalledDesignProbe, ProbeSlug, DesignQuestion).Diagnostics,
            diagnostic => diagnostic.Id == UnboundStrategy && diagnostic.GetMessage().Contains(NoGetCall));

    [Fact]
    public void Initialize_NoSolutionNamesAQuestion_WritesNothing() =>
        Assert.Empty(GeneratorProbe.Generate("internal static class ProbeSolution { public static int CountByLoop(int[] values) => 0; }").GeneratedTrees);

    private static string SourceOf(string probe) => GeneratorProbe.Generate(probe).GeneratedTrees.Single().ToString();

    private static string DesignSourceOf(string probe) =>
        GeneratorProbe.GenerateWithQuestion(probe, ProbeSlug, DesignQuestion).GeneratedTrees.Single().ToString();
}
