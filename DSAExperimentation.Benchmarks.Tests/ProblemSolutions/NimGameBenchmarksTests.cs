using System.Reflection;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NimGameBenchmarks (ARCHITECTURE 17.9), at the sizes BenchmarkDotNet runs,
// read from Stones' [Params]. The mover loses exactly from a multiple of 4, so sizes that were all
// multiples of 4, as 20 and 1,000 were, answered false at every size, and an arm that always said
// false agreed. The generic check compares the arms at the smallest size only; these hold the sizes
// to both verdicts and the two arms to each other at every size.
public sealed partial class NimGameBenchmarksTests
{
    private static readonly bool[] BothVerdicts = [false, true];

    [Fact]
    public void CanWinByModuloFormula_BenchmarkSizes_AnswerBothVerdicts() =>
        Assert.Equal(
            BothVerdicts,
            Verdicts(harness => harness.CanWinByModuloFormula()).Distinct().Order());

    [Fact]
    public void CanWinByMemoizedRecursion_BenchmarkSizes_AgreeWithTheFormulaAtEach() =>
        Assert.Equal(
            Verdicts(harness => harness.CanWinByModuloFormula()),
            Verdicts(harness => harness.CanWinByMemoizedRecursion()));

    private static IEnumerable<bool> Verdicts(Func<NimGameBenchmarks, bool> arm) =>
        BenchmarkStones().Select(stones => arm(new NimGameBenchmarks { Stones = stones }));

    private static IEnumerable<int> BenchmarkStones() =>
        typeof(NimGameBenchmarks).GetProperty(nameof(NimGameBenchmarks.Stones))
            ?.GetCustomAttribute<ParamsAttribute>()
            ?.Values.Cast<int>() ?? [];
}
