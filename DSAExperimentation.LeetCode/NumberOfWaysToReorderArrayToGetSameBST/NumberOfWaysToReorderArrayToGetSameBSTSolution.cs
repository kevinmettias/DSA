using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToReorderArrayToGetSameBST;

// LeetCode 1569. Number of Ways to Reorder Array to Get Same BST: how many other
// permutations of nums insert into the same binary search tree, modulo 1e9+7.
//
// Both strategies count the same quantity the same way - a subtree whose children
// hold sizeL and sizeR values contributes C(sizeL + sizeR, sizeL) interleavings of
// its two children's already-counted insertion sequences, times each child's own
// count - and differ only in how the tree is obtained: re-filtering a fresh pair of
// lists at every recursive call, or building the tree once and folding it. Both
// take each binomial from the same memoized Pascal's triangle - the baseline through
// a hand-rolled Dictionary memo, since a baseline reaches no library type
// (ARCHITECTURE 17.5), the composed arm through this repo's Memoizer - so the
// comparison isolates list-splitting-and-filtering against build-once-then-fold. The
// answer excludes the original array itself, hence the subtracted 1.
internal static class NumberOfWaysToReorderArrayToGetSameBSTSolution
{
    // LeetCode's answer modulus, restated rather than read from ModularArithmetic: a baseline
    // reaches no library type, not even a constant (ARCHITECTURE 17.5).
    private const long Modulo = 1_000_000_007;

    private const int BinaryChildCount = 2;

    // The textbook answer: at each call the first element is the root, the rest are
    // re-scanned into a fresh "less than root" / "greater than root" List<int>, and
    // the two halves recurse. Deliberately BCL-only - it is the arm the composed
    // solution below has to justify itself against.
    public static int CountWaysByListSplitting(int[] nums) => ExcludeOriginal(SplitWays(nums));

    // Build the actual BST via this repo's own BinarySearchTree<int>.Insert, then
    // fold it bottom-up with this repo's own TreeFold/IFoldAlgebra catamorphism -
    // the same machinery TreeMetrics closes over BinaryTreeNode<int> for - so every
    // node's (size, ways) is computed in one walk rather than by re-partitioning the
    // input at every level.
    public static int CountWaysByTreeFold(int[] nums)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in nums)
        {
            tree.Insert(value);
        }

        var (_, ways) = TreeFold.Fold<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            WaysAlgebra, (int Size, long Ways)>(tree.Root);

        return ExcludeOriginal(ways);
    }

    // Pascal's rule, named: a binomial coefficient is the sum of the two terms above
    // it in the triangle, and the edges of the triangle are 1.
    private sealed class PascalRow : IRecurrence<(int N, int K), long>
    {
        public long Replay((int N, int K) state, IRecurrence<(int N, int K), long> rest)
        {
            var (n, k) = state;

            if (k == 0 || k == n)
            {
                return 1;
            }

            var aboveLeft = rest.Replay((n - 1, k - 1), rest);
            var aboveRight = rest.Replay((n - 1, k), rest);

            return (aboveLeft + aboveRight) % ModularArithmetic.Modulo;
        }
    }

    private static long SplitWays(int[] arr)
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
            var belongsLeft = arr[i] < root;
            (belongsLeft ? left : right).Add(arr[i]);
        }

        return DictionaryChoose(left.Count + right.Count, left.Count) * SplitWays([.. left]) % Modulo
            * SplitWays([.. right]) % Modulo;
    }

    // The baseline's binomial: Pascal's rule over a fresh Dictionary memo per call - the
    // same triangle PascalRow states and the same per-call table Memoizer keeps, with
    // the lookup and store written out by hand.
    private static long DictionaryChoose(int itemCount, int pickCount)
    {
        var memo = new Dictionary<(int N, int K), long>();

        return DictionaryPascalEntry((itemCount, pickCount), memo);
    }

    private static long DictionaryPascalEntry((int N, int K) state, Dictionary<(int N, int K), long> memo)
    {
        if (memo.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var result = DictionaryPascalStep(state, memo);
        memo[state] = result;
        return result;
    }

    // A binomial coefficient is the sum of the two terms above it in the triangle, and
    // the edges of the triangle are 1.
    private static long DictionaryPascalStep((int N, int K) state, Dictionary<(int N, int K), long> memo)
    {
        var (n, k) = state;

        if (k == 0 || k == n)
        {
            return 1;
        }

        var aboveLeft = DictionaryPascalEntry((n - 1, k - 1), memo);
        var aboveRight = DictionaryPascalEntry((n - 1, k), memo);

        return (aboveLeft + aboveRight) % Modulo;
    }

    // Every count above includes the original array's own insertion order, which LC
    // does not count as a reordering.
    private static int ExcludeOriginal(long ways) =>
        (int)((ways - 1 + Modulo) % Modulo);

    private static long Choose(int itemCount, int pickCount) =>
        Memoizer.Memoize<(int N, int K), long>((itemCount, pickCount), new PascalRow());

    // Per node: total child-subtree size, and the ways to interleave every child's
    // already-counted insertion sequence back into one sequence for this node - a
    // binary tree has at most 2 children, so choosing one child's size out of the
    // combined total is the same C(n,k) regardless of which child is picked first
    // (C(n,k) = C(n,n-k)), and reduces to the correct "1 way" for 0 or 1 children.
    // The algebra answers this problem alone, so it lives beside the solution.
    private readonly struct WaysAlgebra : IFoldAlgebra<BinaryTreeNode<int>, (int Size, long Ways)>
    {
        public (int Size, long Ways) Empty => (0, 1);

        public (int Size, long Ways) Combine(BinaryTreeNode<int> node, IReadOnlyList<(int Size, long Ways)> children)
        {
            if (children.Count == 0)
            {
                return (1, 1);
            }

            var totalSize = 0;
            var ways = 1L;

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];
                totalSize += child.Size;
                ways = ways * child.Ways % ModularArithmetic.Modulo;
            }

            if (children.Count == BinaryChildCount)
            {
                ways = ways * Choose(totalSize, children[0].Size) % ModularArithmetic.Modulo;
            }

            return (totalSize + 1, ways);
        }
    }
}
