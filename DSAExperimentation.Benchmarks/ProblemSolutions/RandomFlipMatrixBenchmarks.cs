using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.RandomFlipMatrix.RandomFlipMatrixSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomFlipMatrixSolution's, the same classes
// RandomFlipMatrixTests proves correct. Each arm builds its own 1 x Cells matrix (a
// Design problem's whole point is a sequence of calls against one instance, so there
// is no separate "prepare input" step to hoist into [GlobalSetup]) and drains it with
// the same seeded Random, returning a checksum of every picked column so a mismatched
// checksum would mean the two strategies disagree, not just run at different speeds.
[MemoryDiagnoser]
public class RandomFlipMatrixBenchmarks
{
    [Params(200, 5_000)]
    public int Cells;

    [Benchmark(Baseline = true)]
    public long ListScan() => Drain(new FlipMatrixByListScan(1, Cells, new Random(1)));

    [Benchmark]
    public long HashMapSwapRemove() => Drain(new FlipMatrixByHashMapSwapRemove(1, Cells, new Random(1)));

    private long Drain(IFlipMatrix matrix)
    {
        long checksum = 0;
        for (var i = 0; i < Cells; i++)
        {
            checksum += matrix.Flip()[1];
        }

        return checksum;
    }
}
