using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StickersToSpellWord;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StickersToSpellWordSolution's, the same methods
// StickersToSpellWordTests proves correct. Each arm is handed the prepared input
// its hoisted overload takes, so per-sticker letter counting and target sorting is
// charged to [GlobalSetup] rather than to the recursion being measured.
//
// "ab"/"ba" are deliberately equal-effect stickers (both cover exactly one 'a' and
// one 'b') applied to a target built from repeated "ab" pairs, so every choice at
// every step lands on the identical resulting state - maximal overlap, Length+1
// distinct states total - while the naive strategy recomputes all 2^Length
// equivalent choice paths from scratch.
[MemoryDiagnoser]
public class StickersToSpellWordBenchmarks
{
    private static readonly string[] Stickers = ["ab", "ba"];

    private const string RepeatedPair = "ab";

    [Params(10, 16)]
    public int PairCount;

    private PreparedStickers _input;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedPairs = Enumerable.Repeat(RepeatedPair, PairCount);
        var target = string.Concat(repeatedPairs);
        _input = PreparedStickers.Build(Stickers, target);
    }

    [Benchmark(Baseline = true)]
    public int Naive() => StickersToSpellWordSolution.MinStickersByNaiveRecursion(_input);

    [Benchmark]
    public int Memoized() => StickersToSpellWordSolution.MinStickersByMemoizedRecursion(_input);
}
