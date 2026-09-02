using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Make Array Empty (LC 2659): repeatedly remove the front if it's the smallest
// remaining value, else move it to the back - count operations until empty.
// LinearScan re-derives, for each value in ascending removal order, how many
// still-present slots lie between the previous removal and this one (wrapping
// around) via an O(n) present-count scan; FenwickTreeSweep is the exact same
// circular-sweep algorithm but answers each range's present count with this
// repo's own FenwickTree<int,SumOperation<int>> (Binary Indexed Tree) in O(log n)
// instead - the same O(n) vs O(log n) per-step contrast
// CountGoodTripletsInAnArrayBenchmarks already establishes for LC 2179.
[MemoryDiagnoser]
public class MakeArrayEmptyBenchmarks
{
    private const int Seed = 2659; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nums = ShuffledDistinctValues(new Random(Seed), Length);
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => CountOperationsByLinearScan(_nums);

    [Benchmark]
    public long FenwickTreeSweep() => CountOperationsByFenwickTree(_nums);

    private static long CountOperationsByLinearScan(int[] nums)
    {
        var n = nums.Length;
        var order = Enumerable.Range(0, n).OrderBy(i => nums[i]).ToArray();
        var present = new bool[n];
        Array.Fill(present, true);

        long total = 0;
        var prev = -1;

        foreach (var idx in order)
        {
            total += idx > prev
                ? CountPresent(present, prev + 1, idx)
                : CountPresent(present, prev + 1, n - 1) + CountPresent(present, 0, idx);

            present[idx] = false;
            prev = idx;
        }

        return total;
    }

    private static int CountPresent(bool[] present, int left, int right)
    {
        var count = 0;
        for (var i = left; i <= right; i++)
        {
            if (present[i])
            {
                count++;
            }
        }

        return count;
    }

    private static long CountOperationsByFenwickTree(int[] nums)
    {
        var n = nums.Length;
        var order = Enumerable.Range(0, n).OrderBy(i => nums[i]).ToArray();
        var present = new int[n];
        Array.Fill(present, 1);
        var tree = new FenwickTree<int, SumOperation<int>>(present);

        long total = 0;
        var prev = -1;

        foreach (var idx in order)
        {
            total += idx > prev
                ? PresentCount(tree, prev + 1, idx)
                : PresentCount(tree, prev + 1, n - 1) + PresentCount(tree, 0, idx);

            tree.Add(idx, -1);
            prev = idx;
        }

        return total;
    }

    private static int PresentCount(FenwickTree<int, SumOperation<int>> tree, int left, int right)
        => left > right ? 0 : tree.Query(left, right);

    private static int[] ShuffledDistinctValues(Random random, int length)
    {
        var values = Enumerable.Range(0, length).ToArray();
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }
}
