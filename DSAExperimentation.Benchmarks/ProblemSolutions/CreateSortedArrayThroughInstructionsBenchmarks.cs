using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Create Sorted Array through Instructions (LC 1649): the textbook O(n^2) pairwise
// scan of everything inserted so far vs. a left-to-right sweep through this repo's
// own FenwickTree<int,SumOperation<int>> (a Binary Indexed Tree of counts), the
// same value-indexed approach CreateSortedArrayThroughInstructionsTests and
// CountOfSmallerNumbersAfterSelfBenchmarks use - O(n log maxValue) overall.
[MemoryDiagnoser]
public class CreateSortedArrayThroughInstructionsBenchmarks
{
    private const int Modulus = 1_000_000_007;

    [Params(200, 5_000)]
    public int Length;

    private int[] _instructions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1649);
        _instructions = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan()
    {
        var inserted = new List<int>(_instructions.Length);
        long cost = 0;

        foreach (var value in _instructions)
        {
            var less = 0;
            var greater = 0;

            foreach (var existing in inserted)
            {
                if (existing < value)
                {
                    less++;
                }
                else if (existing > value)
                {
                    greater++;
                }
            }

            cost += Math.Min(less, greater);
            inserted.Add(value);
        }

        return (int)(cost % Modulus);
    }

    [Benchmark]
    public int FenwickTreeSweep()
    {
        var maxValue = _instructions.Max();
        var tree = new FenwickTree<int, SumOperation<int>>(maxValue + 1);
        long cost = 0;

        for (var i = 0; i < _instructions.Length; i++)
        {
            var value = _instructions[i];
            var lessCount = value == 0 ? 0 : tree.PrefixQuery(value - 1);
            var greaterCount = i - tree.PrefixQuery(value);
            cost += Math.Min(lessCount, greaterCount);
            tree.Add(value, 1);
        }

        return (int)(cost % Modulus);
    }
}
