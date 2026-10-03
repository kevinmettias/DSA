using DSAExperimentation.LeetCode.DesignBitset;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignBitsetSolution's, the same classes
// DesignBitsetSolutionTests proves correct. Each [Benchmark] fixes/unfixes a scattered
// subset of indices, then runs Size flip() calls (each followed by a count() read,
// so the flag's effect is actually observed) - the operation flip() exists
// specifically to make O(1) instead of O(size), so the eager baseline pays
// O(Size^2) over the loop while the lazy flag pays O(Size). Each arm returns every
// count() it read, in order.
public class DesignBitsetBenchmarks
{
    private const int FixEveryNth = 3;
    private const int UnfixEveryNth = 5;

    // Every count() the workload reads; sized in setup so the workload allocates nothing.
    private int[] _counts = [];

    [Params(500, 20_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _counts = new int[Size];

    [Benchmark(Baseline = true)]
    public int[] EagerArrayFlip() => RunWorkload(new DesignBitsetSolution.BitsetByEagerFlip(Size));

    [Benchmark]
    public int[] LazyFlagDynamicArray() => RunWorkload(new DesignBitsetSolution.BitsetByLazyFlag(Size));

    private int[] RunWorkload(DesignBitsetSolution.IBitset bitset)
    {
        for (var i = 0; i < Size; i++)
        {
            if (i % FixEveryNth == 0)
            {
                bitset.Fix(i);
            }

            if (i % UnfixEveryNth == 0)
            {
                bitset.Unfix(i);
            }
        }

        for (var i = 0; i < Size; i++)
        {
            bitset.Flip();
            _counts[i] = bitset.Count();
        }

        return _counts;
    }
}
