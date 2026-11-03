using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class CatalogBenchmarkGeneratorTests
{
    private const string EnrolledProbe = """
        using DSAExperimentation.LeetCode.Solutions.Conventions;

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

    private const string InPlaceProbe = """
        using DSAExperimentation.LeetCode.Solutions.Conventions;

        [LeetCodeQuestion("probe", LeetCodeAnswerOrder.AsWritten)]
        internal static class ProbeSolution
        {
            public static void SortByInsertion(int[] values) { }
        }
        """;

    [Fact]
    public void Initialize_EnrolledSolution_WritesOneClassNamedForItsProblem() =>
        Assert.Equal(["ProbeCatalogBenchmarks.g.cs"], GeneratorProbe.Generate(EnrolledProbe).GeneratedTrees.Select(tree => Path.GetFileName(tree.FilePath)));

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

    [Fact]
    public void Initialize_StrategyAnsweringInPlace_ReportsItRatherThanLeavingItOut() =>
        Assert.Contains(GeneratorProbe.Generate(InPlaceProbe).Diagnostics, diagnostic => diagnostic.Id == "DSACAT001");

    [Fact]
    public void Initialize_NoSolutionNamesAQuestion_WritesNothing() =>
        Assert.Empty(GeneratorProbe.Generate("internal static class ProbeSolution { public static int CountByLoop(int[] values) => 0; }").GeneratedTrees);

    private static string SourceOf(string probe) => GeneratorProbe.Generate(probe).GeneratedTrees.Single().ToString();
}
