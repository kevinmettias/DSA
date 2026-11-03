using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DSAExperimentation.Tests.Architecture;

// The trace's reach, stated as probes: a benchmark and a solution compiled against the real
// library, each planting one way a baseline can reach it, or one input section 17.5 excuses. A
// sweep that charges nothing proves only that nothing the trace can see is wrong.
public sealed partial class BaselineTraceTests
{
    private const string Stack = "DSAExperimentation.DataStructures.Stack.Stack<Element>";

    [Fact]
    public void From_ConstantTheBaselineReads_IsCharged()
        => Assert.Contains(
            "DSAExperimentation.Domain.Language.English.EnglishLetters.Size",
            ReachedLibraryNames(
                "public static int Baseline(int[] values) => values.Length % DSAExperimentation.Domain.Language.English.EnglishLetters.Size;",
                ArmCalling("ProbeSolution.Baseline(_values)")));

    [Fact]
    public void From_LibraryTypeAHelperBuildsIntoAnInferredLocal_IsCharged()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                public static int Baseline(int[] values)
                {
                    var stack = Build(values);

                    return stack.Count;
                }

                private static RepoStack Build(int[] values)
                {
                    var stack = new RepoStack();

                    foreach (var value in values)
                    {
                        stack.Push(value);
                    }

                    return stack;
                }
                """,
                ArmCalling("ProbeSolution.Baseline(_values)")));

    // A static abstract member is read through the type argument that chooses it.
    [Fact]
    public void From_LibraryWitnessAsATypeArgument_IsCharged()
        => Assert.Contains(
            "DSAExperimentation.Domain.Games.Dice.SixSidedDie",
            ReachedLibraryNames(
                """
                public static int Baseline(int[] values) => values.Length + Faces<DSAExperimentation.Domain.Games.Dice.SixSidedDie>.Count;

                private static class Faces<TDie>
                    where TDie : struct, DSAExperimentation.Domain.Games.Dice.IDie
                {
                    public static int Count => TDie.Faces.Length;
                }
                """,
                ArmCalling("ProbeSolution.Baseline(_values)")));

    // The arm calls through an interface, on an object a factory built in [GlobalSetup].
    [Fact]
    public void From_InstanceSetupBuiltBehindAnInterface_IsTracedWhereItWasMade()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                internal interface ICounter
                {
                    int Count();
                }

                public static ICounter CreateByLibrary() => new LibraryCounter();

                private sealed class LibraryCounter : ICounter
                {
                    public int Count()
                    {
                        var stack = new RepoStack();
                        stack.Push(1);

                        return stack.Count;
                    }
                }
                """,
                """
                private ProbeSolution.ICounter _counter = null!;

                [GlobalSetup]
                public void Setup() => _counter = ProbeSolution.CreateByLibrary();

                [Benchmark(Baseline = true)]
                public int Arm() => _counter.Count();
                """));

    [Fact]
    public void From_MethodGroupAHelperInvokes_IsCharged()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                public static int UsesLibrary(int value)
                {
                    var stack = new RepoStack();
                    stack.Push(value);

                    return stack.Count;
                }
                """,
                """
                [Benchmark(Baseline = true)]
                public int Arm() => Apply(ProbeSolution.UsesLibrary);

                private static int Apply(Func<int, int> step) => step(1);
                """));

    // The runtime calls a comparer back from inside Array.Sort, where no source shows the call.
    [Fact]
    public void From_ComparerTheBclCallsBack_IsCharged()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                public static int Baseline(int[] values)
                {
                    Array.Sort(values, new ByStackCount());

                    return values[0];
                }

                private sealed class ByStackCount : IComparer<int>
                {
                    public int Compare(int x, int y) => new RepoStack().Count + x - y;
                }
                """,
                ArmCalling("ProbeSolution.Baseline(_values)")));

    // The field is an int, so only its initializer shows the library.
    [Fact]
    public void From_InitializerOfAStaticFieldTheBaselineReads_IsCharged()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                private static readonly int Seed = new RepoStack().Count;

                public static int Baseline(int[] values) => values.Length + Seed;
                """,
                ArmCalling("ProbeSolution.Baseline(_values)")));

    // Only the overload the arm's arguments fit is the baseline; the LeetCode-shaped one beside it
    // builds the prepared input and is not charged to an arm handed that input.
    [Fact]
    public void From_OverloadBesideTheOneTheArgumentsFit_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            """
            public static int Baseline(int[] values) => values.Length;

            public static int Baseline(RepoStack prepared) => new RepoStack().Count + prepared.Count;
            """,
            ArmCalling("ProbeSolution.Baseline(_values)")));

    [Fact]
    public void From_StaticFieldNoBaselineReads_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            """
            private static readonly RepoStack Shared = new();
            private static readonly int[] Table = [1, 2];

            public static int Baseline(int[] values) => values.Length + Table.Length;
            """,
            ArmCalling("ProbeSolution.Baseline(_values)")));

    // Constructing one strategy class gives up its members, not a sibling's.
    [Fact]
    public void From_SiblingStrategyTheArmDoesNotConstruct_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            """
            internal sealed class ByLibrary
            {
                public int Run() => new RepoStack().Count;
            }

            internal sealed class ByHand
            {
                public int Run() => 0;
            }
            """,
            """
            [Benchmark(Baseline = true)]
            public int Arm() => new ProbeSolution.ByHand().Run();
            """));

    // Section 17.5's one exception: the container the harness hands the baseline.
    [Fact]
    public void From_LibraryTypeTheEntrysParametersName_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            "public static int Baseline(RepoStack input) => input.Count;",
            """
            private RepoStack _input = new();

            [Benchmark(Baseline = true)]
            public int Arm() => ProbeSolution.Baseline(_input);
            """));

    // What that container holds is the caller's choice too, though no parameter names it.
    [Fact]
    public void From_LibraryTypeTheInputContainerHolds_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            """
            internal sealed class Prepared(int[] values)
            {
                public RepoStack Inner { get; } = StackOf(values);

                private static RepoStack StackOf(int[] values)
                {
                    var stack = new RepoStack();

                    foreach (var value in values)
                    {
                        stack.Push(value);
                    }

                    return stack;
                }
            }

            public static int Baseline(Prepared prepared) => prepared.Inner.Count;
            """,
            """
            private ProbeSolution.Prepared _prepared = new([1, 2]);

            [Benchmark(Baseline = true)]
            public int Arm() => ProbeSolution.Baseline(_prepared);
            """));

    [Fact]
    public void From_LeetCodesTreeNodeTheBaselineBuilds_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            "public static int Baseline(int[] values) => new BinaryTreeNode<int>(values.Length).Value;",
            ArmCalling("ProbeSolution.Baseline(_values)")));

    // The harness's own code - the arm, and the fixtures that lay its input out - hands the input
    // over, and is not the baseline.
    [Fact]
    public void From_LibraryTheHarnessUsesToLayInputOut_IsNotCharged()
        => Assert.Empty(ReachedLibraryNames(
            """
            public static int Baseline(int[] values) => values.Length;

            public static int LayOut() => new RepoStack().Count;
            """,
            """
            private RepoStack _stack = new();

            [Benchmark(Baseline = true)]
            public int Arm() => ProbeSolution.Baseline([.. DSAExperimentation.Benchmarks.Fixtures.ProbeWorkloads.Build(), _stack.Count]);
            """,
            "public static int[] Build() => [ProbeSolution.LayOut()];"));

    // An instance field's initializer runs with the constructor, though no body names it.
    [Fact]
    public void From_InstanceFieldInitializerOfATypeTheBaselineCreates_IsCharged()
        => Assert.Contains(
            Stack,
            ReachedLibraryNames(
                """
                internal sealed class Seeded
                {
                    private readonly int _seed = new RepoStack().Count;

                    public int Seed => _seed;
                }

                public static int Baseline(int[] values) => values.Length + new Seeded().Seed;
                """,
                ArmCalling("ProbeSolution.Baseline(_values)")));

    [Fact]
    public void From_EveryArm_FindsTheSolutionMemberItCalls()
        => Assert.Equal(
            ["DSAExperimentation.LeetCode.Solutions.Probe.ProbeSolution.Baseline(int[])"],
            TraceOf("public static int Baseline(int[] values) => values.Length;", ArmCalling("ProbeSolution.Baseline(_values)")).Entries);

    private static string ArmCalling(string call)
        => $$"""
            private int[] _values = [];

            [Benchmark(Baseline = true)]
            public int Arm() => {{call}};
            """;

    private static List<string> ReachedLibraryNames(string solutionMembers, string benchmarkMembers, string fixtureMembers = "")
        => [.. TraceOf(solutionMembers, benchmarkMembers, fixtureMembers).Reaches.Select(reach => reach.Name)];

    private static BaselineTrace.ArmTrace TraceOf(string solutionMembers, string benchmarkMembers, string fixtureMembers = "")
    {
        var probe = ProbeCompilation(solutionMembers, benchmarkMembers, fixtureMembers);
        var arm = Assert.Single(BaselineTrace.ArmsIn(probe));

        return BaselineTrace.From(arm, new SourceModels([probe, RepositoryCompilation.Solutions, RepositoryCompilation.Framework]));
    }

    // The probe compiled against what the benchmark project compiles against. A probe that does not
    // compile would trace nothing, so it fails here instead.
    private static CSharpCompilation ProbeCompilation(string solutionMembers, string benchmarkMembers, string fixtureMembers)
    {
        var parse = new CSharpParseOptions(LanguageVersion.CSharp14);
        var trees = new[]
        {
            CSharpSyntaxTree.ParseText(ProbeSource(solutionMembers, benchmarkMembers, fixtureMembers), parse, Path.Combine("ProblemSolutions", "ProbeBenchmarks.cs")),
            CSharpSyntaxTree.ParseText("global using System; global using System.Collections.Generic; global using System.Linq;", parse),
        };
        var probe = CSharpCompilation.Create(
            "Probe",
            trees,
            RepositoryCompilation.Benchmarks.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
        var errors = probe.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToList();

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));

        return probe;
    }

    // One solution class, one fixture class and one benchmark class, each in the namespace the trace
    // reads it by, the benchmark's file under a ProblemSolutions folder.
    private static string ProbeSource(string solutionMembers, string benchmarkMembers, string fixtureMembers)
        => $$"""
            using BenchmarkDotNet.Attributes;
            using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
            using DSAExperimentation.LeetCode.Solutions.Probe;
            using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

            namespace DSAExperimentation.LeetCode.Solutions.Probe
            {
                internal static class ProbeSolution
                {
            {{solutionMembers}}
                }
            }

            namespace DSAExperimentation.Benchmarks.Fixtures
            {
                internal static class ProbeWorkloads
                {
            {{fixtureMembers}}
                }
            }

            namespace DSAExperimentation.Benchmarks.ProblemSolutions
            {
                public class ProbeBenchmarks
                {
            {{benchmarkMembers}}
                }
            }
            """;
}
