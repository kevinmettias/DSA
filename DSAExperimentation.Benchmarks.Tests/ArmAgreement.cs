using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests;

// What BenchmarkArmsTests cannot learn by reflection: the benchmarks whose arms answer correctly yet
// differently. Everything not listed here is held to exact agreement, so this list is the whole of
// the per-problem knowledge the generic check needs, and an entry is a claim about the problem
// statement or the harness that a reader can check against it.
internal static class ArmAgreement
{
    // LeetCode leaves the order of these answers unspecified, so arms agree when they return the same
    // elements in any order. MatrixCellsInDistanceOrder and SortCharactersByFrequency pin an order but
    // leave ties free, so for them this compares only the elements, and the order stays a property
    // their own tests check.
    public static IReadOnlySet<Type> UnorderedAnswers { get; } = new HashSet<Type>
    {
        typeof(AccountsMergeBenchmarks),
        typeof(ErectTheFenceBenchmarks),
        typeof(FindModeInBinarySearchTreeBenchmarks),
        typeof(KClosestPointsToOriginBenchmarks),
        typeof(MatrixCellsInDistanceOrderBenchmarks),
        typeof(SingleNumberIIIBenchmarks),
        typeof(SortCharactersByFrequencyBenchmarks),
        typeof(SubsetsBenchmarks),
        typeof(ThreeSumBenchmarks),
    };

    // Arms here answer differently by design, so the generic check only runs each one to completion.
    public static IReadOnlyDictionary<Type, string> IncomparableAnswers { get; } = new Dictionary<Type, string>
    {
        [typeof(GenerateRandomPointInACircleBenchmarks)] = RandomDraw,
        [typeof(ImplementRand10UsingRand7Benchmarks)] = RandomDraw,
        [typeof(LinkedListRandomNodeBenchmarks)] = RandomDraw,
        [typeof(RandomPickIndexBenchmarks)] = RandomDraw,
        [typeof(RandomPickWithBlacklistBenchmarks)] = RandomDraw,
        [typeof(ShuffleAnArrayBenchmarks)] = RandomDraw,
        [typeof(InsertIntoABinarySearchTreeBenchmarks)] =
            "LeetCode accepts any valid tree, and each arm answers with the root of the different tree it builds.",
        [typeof(TopKFrequentElementsBenchmarks)] =
            "The workload ties 23 values at the top-10 boundary, outside LC 347's guarantee of a unique answer.",
    };

    // Not a case at all: its arms are [ParamsSource] values naming registered problems rather than
    // [Benchmark] methods, and LeetCodeProblemBenchmarksTests covers the properties it owes.
    public static IReadOnlySet<Type> Excluded { get; } = new HashSet<Type> { typeof(LeetCodeProblemBenchmarks) };

    private const string RandomDraw =
        "The problem asks for a random draw, and each arm consumes the seeded generator differently.";
}
