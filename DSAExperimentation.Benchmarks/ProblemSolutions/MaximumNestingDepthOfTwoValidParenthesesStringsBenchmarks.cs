using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.MaximumNestingDepthOfTwoValidParenthesesStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximumNestingDepthOfTwoValidParenthesesStringsSolution's - the O(n^2) rescan
// baseline against the single-pass Stack<char> walk.
[MemoryDiagnoser]
public class MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks
{
    // LC problem number, reused as the deterministic random seed.
    private const int RandomSeed = 1111;

    // The generated sequence is split evenly between '(' and ')'.

    // Exclusive upper bound for the open/close coin flip (0 or 1).
    private const int CoinFlipBound = 2;

    private string _sequence = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _sequence = Generate(new Random(RandomSeed), Length);

    // Workload sizing only: a random but always-valid parentheses sequence of the
    // requested length.
    private static string Generate(Random random, int length)
    {
        var builder = new System.Text.StringBuilder(length);
        var openRemaining = length / AlgorithmConstants.HalvingFactor;
        var closeRemaining = length / AlgorithmConstants.HalvingFactor;

        while (openRemaining > 0 || closeRemaining > 0)
        {
            if (ShouldOpenNext(openRemaining, closeRemaining, random))
            {
                builder.Append('(');
                openRemaining--;
            }
            else
            {
                builder.Append(')');
                closeRemaining--;
            }
        }

        return builder.ToString();
    }

    // An opener goes down while one is still owed and either the closers have caught
    // up - an opener is owed to keep the sequence valid - or the coin flip says so.
    private static bool ShouldOpenNext(int openRemaining, int closeRemaining, Random random) =>
        openRemaining > 0 && (closeRemaining == openRemaining || random.Next(CoinFlipBound) == 0);

    [Benchmark(Baseline = true)]
    public int[] RecomputeDepthPerPosition() =>
        MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByDepthRescan(_sequence);

    [Benchmark]
    public int[] StackTrackedSinglePass() =>
        MaximumNestingDepthOfTwoValidParenthesesStringsSolution.MaxDepthAfterSplitByOpenerStack(_sequence);
}
