using DSAExperimentation.LeetCode.RandomFlipMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RandomFlipMatrixSolution's, the same classes
// RandomFlipMatrixSolutionTests proves correct. Each arm builds its own 1 x Cells matrix (a
// Design problem's whole point is a sequence of calls against one instance, so there
// is no separate "prepare input" step to hoist into [GlobalSetup]) and drains it with
// the same seeded Random, returning every cell Flip picked, in call order.
public class RandomFlipMatrixBenchmarks
{
    private int[][] _flips = [];

    [Params(200, 5_000)]
    public int Cells { get; set; }

    [GlobalSetup]
    public void Setup() => _flips = new int[Cells][];

    [Benchmark(Baseline = true)]
    public int[][] ListScan() => Drain(new RandomFlipMatrixSolution.FlipMatrixByListScan(1, Cells, new Random(1)));

    [Benchmark]
    public int[][] HashMapSwapRemove() => Drain(new RandomFlipMatrixSolution.FlipMatrixByHashMapSwapRemove(1, Cells, new Random(1)));

    private int[][] Drain(RandomFlipMatrixSolution.IFlipMatrix matrix)
    {
        for (var i = 0; i < Cells; i++)
        {
            _flips[i] = matrix.Flip();
        }

        return _flips;
    }
}
