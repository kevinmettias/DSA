using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Chunked Palindrome Decomposition (LC 1147): the classic greedy solution
// written the textbook way - growing `left`/`right` via repeated string
// concatenation and comparing them with == - vs. the same greedy walk using this
// repo's own RollingHash for an O(1) chunk-equality screen per growth step instead
// of an O(len) string build+compare (LongestChunkedPalindromeDecompositionTests'
// exact composition). _text has every character distinct (Unicode code points
// starting at 1000, so no accidental match ever fires), the same "force the real
// worst case" intent TwoSumBenchmarks' own setup comment names - both strategies
// are forced to grow their pending window all the way to the middle, which is
// exactly where NaiveConcatenation's repeated O(len) string work costs the most.
[MemoryDiagnoser]
public class LongestChunkedPalindromeDecompositionBenchmarks
{
    private const int CodePointBase = 1000;

    private const int MatchedPairChunkCount = 2;

    [Params(200, 2_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)(CodePointBase + i);
        }

        _text = new string(chars);
    }

    private readonly record struct ChunkScanState(string Left, string Right, int Count, int I, int J);

    [Benchmark(Baseline = true)]
    public int NaiveConcatenation()
    {
        var state = new ChunkScanState(string.Empty, string.Empty, 0, 0, _text.Length - 1);

        while (state.I < state.J)
        {
            state = AdvanceNaiveStep(state);
        }

        return state.Count + (state.Left.Length > 0 || state.I == state.J ? 1 : 0);
    }

    private ChunkScanState AdvanceNaiveStep(ChunkScanState state)
    {
        var left = state.Left + _text[state.I];
        var right = _text[state.J] + state.Right;
        var count = state.Count;

        if (left == right)
        {
            count += MatchedPairChunkCount;
            left = string.Empty;
            right = string.Empty;
        }

        return state with { Left = left, Right = right, Count = count, I = state.I + 1, J = state.J - 1 };
    }

    private readonly record struct HashChunkState(int MatchStart, int I, int J, int Count);

    [Benchmark]
    public int RollingHashChunking()
    {
        var hash = new RollingHash(_text);
        var state = new HashChunkState(0, 0, _text.Length - 1, 0);

        while (state.I < state.J)
        {
            state = AdvanceHashStep(hash, state);
        }

        return state.Count + (state.MatchStart <= state.J ? 1 : 0);
    }

    private HashChunkState AdvanceHashStep(RollingHash hash, HashChunkState state)
    {
        var len = state.I - state.MatchStart + 1;
        var matchStart = state.MatchStart;
        var count = state.Count;

        var leftSpan = _text.AsSpan(matchStart, len);
        var rightSpan = _text.AsSpan(state.J, len);

        if (hash.Hash(matchStart, len) == hash.Hash(state.J, len) && leftSpan.SequenceEqual(rightSpan))
        {
            count += MatchedPairChunkCount;
            matchStart = state.I + 1;
        }

        return state with { MatchStart = matchStart, Count = count, I = state.I + 1, J = state.J - 1 };
    }
}
