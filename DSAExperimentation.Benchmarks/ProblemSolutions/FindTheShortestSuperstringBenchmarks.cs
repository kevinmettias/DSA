using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Shortest Superstring (LC 943): the textbook permutation brute force (try
// every word order, O(n! * n)) vs. the bitmask-TSP DP built on this repo's own
// Memoizer<TState,TResult> (O(2^n * n^2)). Both reduce to the same core question -
// the maximum total overlap achievable across all words - computed once in
// [GlobalSetup] as an overlap matrix shared by both strategies, so each [Benchmark]
// method times only its own search strategy, not string-overlap preprocessing common
// to both. Word count stays small ([6, 9]) because n! overtakes 2^n * n^2 fast enough
// that BruteForce would otherwise dominate the run.
[MemoryDiagnoser]
public class FindTheShortestSuperstringBenchmarks
{
    private const string Alphabet = "ACGT";
    private const int WordLength = 5;
    private const int RandomSeed = 943; // LC problem number

    [Params(6, 9)]
    public int WordCount;

    private int[,] _overlap = null!;
    private int _totalLength;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var seen = new HashSet<string>();
        var words = new List<string>();

        while (words.Count < WordCount)
        {
            var chars = new char[WordLength];
            for (var i = 0; i < WordLength; i++)
            {
                chars[i] = Alphabet[random.Next(Alphabet.Length)];
            }

            var word = new string(chars);
            if (seen.Add(word))
            {
                words.Add(word);
            }
        }

        _totalLength = words.Sum(word => word.Length);
        _overlap = BuildOverlaps(words);
    }

    private readonly record struct PermutationState(int Size, bool[] Used, int[] Order);

    [Benchmark(Baseline = true)]
    public int BruteForcePermutations()
    {
        var state = new PermutationState(WordCount, new bool[WordCount], new int[WordCount]);
        var bestOverlap = PermuteBestOverlap(0, 0, state);
        return _totalLength - bestOverlap;
    }

    private int PermuteBestOverlap(int depth, int overlapSoFar, PermutationState state)
    {
        if (depth == state.Size)
        {
            return overlapSoFar;
        }

        var best = 0;

        for (var next = 0; next < state.Size; next++)
        {
            if (state.Used[next])
            {
                continue;
            }

            var branchBest = TryCandidate(next, depth, overlapSoFar, state);
            best = Math.Max(best, branchBest);
        }

        return best;
    }

    private int TryCandidate(int next, int depth, int overlapSoFar, PermutationState state)
    {
        state.Used[next] = true;
        state.Order[depth] = next;
        var added = depth == 0 ? 0 : _overlap[state.Order[depth - 1], next];
        var branchBest = PermuteBestOverlap(depth + 1, overlapSoFar + added, state);
        state.Used[next] = false;

        return branchBest;
    }

    [Benchmark]
    public int MemoizedBitmaskDp()
    {
        var n = WordCount;
        var fullMask = (1 << n) - 1;
        var bestOverlap = 0;

        for (var last = 0; last < n; last++)
        {
            var (total, _) = Memoizer.Memoize<(int Mask, int Last), (int Best, int Prev)>((fullMask, last), Recurrence);
            bestOverlap = Math.Max(bestOverlap, total);
        }

        return _totalLength - bestOverlap;
    }

    private readonly record struct CandidateContext(
        int Remaining,
        int LastWord,
        Func<(int Mask, int Last), (int Best, int Prev)> Best);

    private (int Best, int Prev) Recurrence(
        (int Mask, int Last) state,
        Func<(int Mask, int Last), (int Best, int Prev)> best)
    {
        var remaining = state.Mask & ~(1 << state.Last);
        if (remaining == 0)
        {
            return (0, -1);
        }

        var context = new CandidateContext(remaining, state.Last, best);
        var result = (Best: -1, Prev: -1);

        for (var candidate = 0; candidate < WordCount; candidate++)
        {
            result = ConsiderCandidate(candidate, context, result);
        }

        return result;
    }

    private (int Best, int Prev) ConsiderCandidate(int candidate, CandidateContext context, (int Best, int Prev) currentResult)
    {
        if ((context.Remaining & (1 << candidate)) == 0)
        {
            return currentResult;
        }

        var (subBest, _) = context.Best((context.Remaining, candidate));
        var total = subBest + _overlap[candidate, context.LastWord];

        return total > currentResult.Best ? (total, candidate) : currentResult;
    }

    private static int[,] BuildOverlaps(List<string> words)
    {
        var n = words.Count;
        var overlap = new int[n, n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (i != j)
                {
                    overlap[i, j] = OverlapLength(words[i], words[j]);
                }
            }
        }

        return overlap;
    }

    private static int OverlapLength(string left, string right)
    {
        var max = Math.Min(left.Length, right.Length);

        for (var len = max; len > 0; len--)
        {
            var candidateSuffix = left.AsSpan(left.Length - len);
            var candidatePrefix = right.AsSpan(0, len);
            if (candidateSuffix.SequenceEqual(candidatePrefix))
            {
                return len;
            }
        }

        return 0;
    }
}
