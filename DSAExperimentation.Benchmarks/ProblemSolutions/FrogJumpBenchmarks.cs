using DSAExperimentation.LeetCode.FrogJump;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FrogJumpSolution's, the same methods
// FrogJumpSolutionTests proves correct. The stones are built as consecutive integers
// (every jump delta from every reachable stone lands on another real stone)
// with an unreachable final stone appended far away - the same "rig the
// input so both strategies are forced through their full worst case" trick
// TwoSumBenchmarks/JumpGameBenchmarks use, here forcing
// CanCrossByRecursiveBruteForce through its full exponential search instead
// of returning early on success.
//
// Sizes are per arm. That search stops at 16 stones; the hash-map DP only visits
// each (stone, jump size) pair once and runs on to LC 403's own bound of 2,000 stones,
// and the two are compared at the counts both run.
public class FrogJumpBenchmarks
{
    // Offset from StoneCount back to the value of the last consecutively-filled
    // stone (the loop fills indices [0, StoneCount - 2] with their own index).
    private const int LastConsecutiveStoneOffset = 2;

    // Pushed onto the last consecutive stone's value so the appended final stone
    // sits far enough away that no jump size can ever reach it.
    private const int UnreachableStoneGap = 1_000;

    private Dictionary<int, int[]> _stonesByCount = [];

    public static IEnumerable<int> BruteForceSizes => [10, 16];

    public static IEnumerable<int> DynamicProgrammingSizes => [.. BruteForceSizes, 200, 2_000];

    // Every stone count any arm runs is built here, outside the timed region; an arm looks
    // its own up.
    [GlobalSetup]
    public void Setup() => _stonesByCount = DynamicProgrammingSizes.ToDictionary(count => count, BuildStones);

    private static int[] BuildStones(int stoneCount)
    {
        var stones = new int[stoneCount];

        for (var i = 0; i < stoneCount - 1; i++)
        {
            stones[i] = i;
        }

        stones[stoneCount - 1] = stoneCount - LastConsecutiveStoneOffset + UnreachableStoneGap;

        return stones;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public bool CanCrossByRecursiveBruteForce(int stoneCount) =>
        FrogJumpSolution.CanCrossByRecursiveBruteForce(_stonesByCount[stoneCount]);

    [Benchmark]
    [ArgumentsSource(nameof(DynamicProgrammingSizes))]
    public bool CanCrossByHashMapDynamicProgramming(int stoneCount) =>
        FrogJumpSolution.CanCrossByHashMapDynamicProgramming(_stonesByCount[stoneCount]);
}
