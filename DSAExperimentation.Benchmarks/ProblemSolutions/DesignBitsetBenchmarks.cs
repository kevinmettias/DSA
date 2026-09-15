using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignBitset;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignBitsetSolution's, the same classes
// DesignBitsetTests proves correct. Each [Benchmark] fixes/unfixes a scattered
// subset of indices, then runs Size flip() calls (each followed by a count() read,
// so the flag's effect is actually observed) - the operation flip() exists
// specifically to make O(1) instead of O(size), so the eager baseline pays
// O(Size^2) over the loop while the lazy flag pays O(Size).
[MemoryDiagnoser]
public class DesignBitsetBenchmarks
{
    private const int FixEveryNth = 3;
    private const int UnfixEveryNth = 5;

    [Params(500, 20_000)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public long EagerArrayFlip() => RunWorkload(new DesignBitsetSolution.BitsetByEagerFlip(Size));

    [Benchmark]
    public long LazyFlagDynamicArray() => RunWorkload(new DesignBitsetSolution.BitsetByLazyFlag(Size));

    private long RunWorkload(DesignBitsetSolution.IBitset bitset)
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

        long total = 0;

        for (var i = 0; i < Size; i++)
        {
            bitset.Flip();
            total += bitset.Count();
        }

        return total;
    }
}
