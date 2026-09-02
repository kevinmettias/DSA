using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Reorder Array to Get Same BST (LC 1569): the textbook recursive
// list-splitting solution (re-filter a fresh List<int> into "less than root"/
// "greater than root" at every recursive call, the naive approach with no repo
// primitive) vs. this repo's own BinarySearchTree<int>.Insert + one TreeFold/
// IFoldAlgebra pass computing each node's (Size, Ways) bottom-up in a single walk -
// the same primitive composition NumberOfWaysToReorderArrayToGetSameBSTTests proves
// correct. Both sides share the identical Memoizer-backed Pascal's-triangle Choose,
// so the comparison isolates list-splitting-and-filtering vs. build-once-then-fold.
[MemoryDiagnoser]
public class NumberOfWaysToReorderArrayToGetSameBSTBenchmarks
{
    private const long Modulo = 1_000_000_007;
    private const int RandomSeed = 1569; // LC problem number
    private const int BinaryChildCount = 2;

    [Params(200, 1000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveListSplitting() => (int)((Ways(_nums) - 1 + Modulo) % Modulo);

    private static long Ways(int[] arr)
    {
        if (arr.Length <= 1)
        {
            return 1;
        }

        var root = arr[0];
        var left = new List<int>();
        var right = new List<int>();

        for (var i = 1; i < arr.Length; i++)
        {
            (arr[i] < root ? left : right).Add(arr[i]);
        }

        return Choose(left.Count + right.Count, left.Count) * Ways([.. left]) % Modulo * Ways([.. right]) % Modulo;
    }

    [Benchmark]
    public int PrimitiveComposed()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _nums)
        {
            tree.Insert(value);
        }

        var (_, ways) = TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            WaysAlgebra, (int Size, long Ways)>(tree.Root);

        return (int)((ways - 1 + Modulo) % Modulo);
    }

    private static long Choose(int n, int k) => Memoizer.Memoize<(int N, int K), long>((n, k), ChooseRecurrence);

    private static long ChooseRecurrence((int N, int K) state, Func<(int, int), long> choose)
    {
        var (n, k) = state;
        return k == 0 || k == n ? 1 : (choose((n - 1, k - 1)) + choose((n - 1, k))) % Modulo;
    }

    private readonly struct WaysAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Size, long Ways)>
    {
        public static (int Size, long Ways) Empty => (0, 1);

        public static (int Size, long Ways) Combine(BinaryTreeNode<int> node, IReadOnlyList<(int Size, long Ways)> children)
        {
            if (children.Count == 0)
            {
                return (1, 1);
            }

            var totalSize = 0;
            var ways = 1L;

            foreach (var child in children)
            {
                totalSize += child.Size;
                ways = ways * child.Ways % Modulo;
            }

            if (children.Count == BinaryChildCount)
            {
                ways = ways * Choose(totalSize, children[0].Size) % Modulo;
            }

            return (totalSize + 1, ways);
        }
    }
}
