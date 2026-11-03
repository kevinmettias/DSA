using DSAExperimentation.Benchmarks.Generators;

namespace DSAExperimentation.Benchmarks.Tests.Generators;

public sealed partial class CatalogBenchmarkWriterTests
{
    private const string Holder = """
        internal static class Holder
        {
            public static int[] SortByMerge(int[] values) => values;

            public static bool TryFindByScan(int[] nums, int target, out int first, out int second) { first = 0; second = 0; return false; }
        }
        """;

    // C# writes the count before an array element's own brackets: new int[n][], never new int[][n].
    [Fact]
    public void Write_ArmAnsweringAnArray_SizesItsBufferBeforeTheAnswersBrackets() =>
        Assert.Contains("_sortByMergeAnswers = new int[_sortByMergeColumns.Count][];", SourceFor("SortByMerge"), StringComparison.Ordinal);

    [Fact]
    public void Write_TryShapedArm_GathersItsOutputsWithItsReturnValue() =>
        Assert.Contains("answers[index] = (answer, output0, output1);", SourceFor("TryFindByScan"), StringComparison.Ordinal);

    [Fact]
    public void Write_AnyArm_ReadsItsColumnsInDeclarationOrder() =>
        Assert.Contains("global::Holder.TryFindByScan(input0[index], input1[index], out var output0, out var output1)", SourceFor("TryFindByScan"), StringComparison.Ordinal);

    private static string SourceFor(string strategy) =>
        new CatalogBenchmarkWriter("global::Holder", "HolderCatalogBenchmarks", [StrategyShape.Of(GeneratorProbe.Method(Holder, strategy))]).Write();
}
