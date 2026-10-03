using System.Reflection;
using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests;

// What a filter like `--anyCategories DataStructures.Heap` selects is decided here, so these pin
// the three things a filter depends on: a benchmark is filed under the library code it actually
// reaches, under every prefix of that namespace, and as a whole class rather than arm by arm.
public sealed partial class LibraryCategoryDiscovererTests
{
    private const string ProblemFolder = "ProblemSolutions";
    private const string StrategySwapFolder = "StrategySwaps";

    // Dijkstra in NetworkDelayTimeSolution reaches ShortestPath.Dijkstra, which keeps its frontier
    // in this repo's Heap - two library hops below the arm, neither named in the benchmark's source.
    [Fact]
    public void GetCategories_ShortestPathArm_NamesEveryPrimitiveItReachesAndEachPrefix()
    {
        var categories = CategoriesOf(typeof(ShortestPathAlgorithmBenchmarks), nameof(ShortestPathAlgorithmBenchmarks.Dijkstra));

        Assert.Contains("Algorithms.ShortestPaths", categories);
        Assert.Contains("Algorithms", categories);
        Assert.Contains("DataStructures.Heap", categories);
        Assert.Contains("DataStructures", categories);
        Assert.Contains(ProblemFolder, categories);
    }

    // LinearScan is the BCL baseline and reaches no heap itself; it carries the heap arm's
    // categories so a heap filter still runs the comparison the class exists for.
    [Fact]
    public void GetCategories_BaselineArm_CarriesItsClassesCategories()
    {
        var baseline = CategoriesOf(typeof(DesignAFoodRatingSystemBenchmarks), nameof(DesignAFoodRatingSystemBenchmarks.LinearScan));
        var rival = CategoriesOf(typeof(DesignAFoodRatingSystemBenchmarks), nameof(DesignAFoodRatingSystemBenchmarks.LazyDeletionHeap));

        Assert.Contains("DataStructures.Heap", baseline);
        Assert.Equal(rival, baseline);
    }

    [Fact]
    public void GetCategories_StrategySwapSuite_IsFiledUnderItsOwnFolder()
    {
        var categories = CategoriesOf(typeof(ReduceOrderBenchmarks), nameof(ReduceOrderBenchmarks.BreadthFirst));

        Assert.Contains(StrategySwapFolder, categories);
        Assert.DoesNotContain(ProblemFolder, categories);
    }

    [Fact]
    public void GetCategories_ExplicitBenchmarkCategory_IsKept()
    {
        var categories = CategoriesOf(typeof(PinnedCategorySample), nameof(PinnedCategorySample.Arm));

        Assert.Contains(PinnedCategorySample.Category, categories);
    }

    // Every arm is filed at least under its folder, so `--anyCategories ProblemSolutions` and
    // `--anyCategories StrategySwaps` between them select every benchmark there is.
    [Fact]
    public void GetCategories_EveryBenchmarkArm_IsFiledUnderItsFolder()
    {
        var unfiled = BenchmarkClass.All
            .SelectMany(benchmark => benchmark.Arms)
            .Where(arm => !LibraryCategoryDiscoverer.Instance.GetCategories(arm).Any(IsFolder))
            .Select(arm => $"{arm.DeclaringType?.Name}.{arm.Name}")
            .ToList();

        Assert.Empty(unfiled);
    }

    private static bool IsFolder(string category) => category is ProblemFolder or StrategySwapFolder;

    private static string[] CategoriesOf(Type benchmark, string armName)
    {
        var arm = benchmark.GetMethod(armName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"{benchmark.Name} has no public arm named {armName}.");

        return LibraryCategoryDiscoverer.Instance.GetCategories(arm);
    }

    // A benchmark outside the benchmark assembly, so the generic arm check never sees it, carrying
    // a category written by hand.
    public sealed class PinnedCategorySample
    {
        public const string Category = "Pinned";

        [Benchmark]
        [BenchmarkCategory(Category)]
        public int Arm() => 0;
    }
}
