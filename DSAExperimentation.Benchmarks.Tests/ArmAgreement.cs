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
        typeof(CriticalConnectionsInANetworkBenchmarks),
        typeof(ErectTheFenceBenchmarks),
        typeof(FindModeInBinarySearchTreeBenchmarks),
        typeof(KClosestPointsToOriginBenchmarks),
        typeof(MatrixCellsInDistanceOrderBenchmarks),
        typeof(SingleNumberIIIBenchmarks),
        typeof(SortCharactersByFrequencyBenchmarks),
        typeof(SubsetsBenchmarks),
        typeof(ThreeSumBenchmarks),
        typeof(TopKFrequentElementsBenchmarks),
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
        [typeof(AllOneDataStructureBenchmarks)] =
            "GetMaxKey and GetMinKey may return any key at the extreme count, and the arms pick different keys from a tie.",
        [typeof(AllPossibleFullBinaryTreesBenchmarks)] =
            "LeetCode reads each tree on its own, and the memoized arm shares subtrees across trees the plain recursion builds afresh.",
        [typeof(DeleteNodeInABSTBenchmarks)] =
            "LeetCode accepts any valid BST without the key, and one arm rebuilds a balanced tree where the other deletes in place.",
        [typeof(InsertIntoABinarySearchTreeBenchmarks)] =
            "LeetCode accepts any valid tree, and each arm answers with the root of the different tree it builds.",
    };

    private const string RandomDraw =
        "The problem asks for a random draw, and each arm consumes the seeded generator differently.";
}
