using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Sum of Rectangle No Larger Than K (LC 363): for every column pair, an
// O(rows^2) brute-force scan of every start/end window in the compressed 1D
// row-sum array vs. this repo's own BinarySearchTree<int> holding prefix sums,
// answering each "smallest prefix sum >= (current - k)" query in O(log rows)
// instead of an O(rows) scan - the same ceiling composition
// MaxSumOfRectangleNoLargerThanKTests uses. Columns stay fixed at a small constant
// (Cols) so both methods pay the same O(cols^2) outer-loop factor; Rows is the
// scaled [Params] axis, isolating the O(rows^2) vs. O(rows*log(rows)) inner-window
// asymptotic split the same way MaximumSubarrayBenchmarks isolates brute-force vs.
// Kadane on array Length alone. A brute-force O(cols^2*rows^2) with both axes
// scaled together would blow up to a 4th-power cost long before the BST's
// per-node allocation overhead stops dominating at small n.
[MemoryDiagnoser]
public class MaxSumOfRectangleNoLargerThanKBenchmarks
{
    private const int K = 50;
    private const int Cols = 8;

    [Params(200, 3_000)]
    public int Rows;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(363);
        _matrix = Enumerable.Range(0, Rows)
            .Select(_ => Enumerable.Range(0, Cols).Select(_ => random.Next(-10, 11)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceWindowScan()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var best = int.MinValue;

        for (var left = 0; left < cols; left++)
        {
            var rowSums = new int[rows];

            for (var right = left; right < cols; right++)
            {
                for (var r = 0; r < rows; r++)
                {
                    rowSums[r] += _matrix[r][right];
                }

                best = Math.Max(best, BestWindowBruteForce(rowSums));
            }
        }

        return best;
    }

    [Benchmark]
    public int BstCeilingScan()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var best = int.MinValue;

        for (var left = 0; left < cols; left++)
        {
            var rowSums = new int[rows];

            for (var right = left; right < cols; right++)
            {
                for (var r = 0; r < rows; r++)
                {
                    rowSums[r] += _matrix[r][right];
                }

                best = Math.Max(best, BestWindowBstCeiling(rowSums));
            }
        }

        return best;
    }

    private static int BestWindowBruteForce(int[] rowSums)
    {
        var best = int.MinValue;

        for (var start = 0; start < rowSums.Length; start++)
        {
            var sum = 0;

            for (var end = start; end < rowSums.Length; end++)
            {
                sum += rowSums[end];

                if (sum <= K)
                {
                    best = Math.Max(best, sum);
                }
            }
        }

        return best;
    }

    private static int BestWindowBstCeiling(int[] rowSums)
    {
        var prefixes = new BinarySearchTree<int>();
        prefixes.Insert(0);

        var best = int.MinValue;
        var prefix = 0;

        foreach (var value in rowSums)
        {
            prefix += value;

            if (TryFindCeiling(prefixes, prefix - K, out var ceiling))
            {
                best = Math.Max(best, prefix - ceiling);
            }

            prefixes.Insert(prefix);
        }

        return best;
    }

    private static bool TryFindCeiling(BinarySearchTree<int> tree, int target, out int ceiling)
    {
        var node = tree.Root;
        var found = false;
        ceiling = default;

        while (node is not null)
        {
            if (node.Value >= target)
            {
                ceiling = node.Value;
                found = true;
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }

        return found;
    }
}
