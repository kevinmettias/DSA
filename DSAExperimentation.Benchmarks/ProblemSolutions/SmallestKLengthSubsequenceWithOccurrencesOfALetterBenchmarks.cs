using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest K-Length Subsequence With Occurrences of a Letter (LC 2030): the O(n*k)
// naive greedy (rescan the whole feasible window from scratch for every one of the k
// output characters) vs. a single O(n) monotonic-stack sweep through this repo's own
// Stack<char> (RemoveDuplicateLettersTests/OnlineStockSpanBenchmarks' precedent),
// where each character is pushed once and popped at most once across the whole
// string.
[MemoryDiagnoser]
public class SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarks
{
    private const char Letter = 'a';
    private const int Repetition = 2;
    private const int LowercaseAlphabetSize = 26;
    private const int SubsequenceLengthDivisor = 2;

    [Params(500, 5_000)]
    public int Length;

    private string _s = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(0, LowercaseAlphabetSize));
        }

        // Force at least Repetition occurrences of Letter so both strategies stay feasible.
        chars[0] = Letter;
        chars[Length - 1] = Letter;

        _s = new string(chars);
        _k = Length / SubsequenceLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public string NaiveWindowRescan()
    {
        var suffixCount = BuildSuffixCount(_s, Letter);
        var answer = new char[_k];
        var progress = new ScanProgress();

        for (var slot = 0; slot < _k; slot++)
        {
            SelectNextCharacter(suffixCount, answer, slot, progress);
        }

        return new string(answer);
    }

    private void SelectNextCharacter(int[] suffixCount, char[] answer, int slot, ScanProgress progress)
    {
        var charsNeeded = _k - slot;
        var lettersStillNeeded = Math.Max(0, Repetition - progress.LettersUsed);
        var windowEnd = _s.Length - charsNeeded;

        var bestPosition = FindBestPosition(suffixCount, lettersStillNeeded, progress.Cursor, windowEnd);

        answer[slot] = _s[bestPosition];
        if (_s[bestPosition] == Letter)
        {
            progress.LettersUsed++;
        }

        progress.Cursor = bestPosition + 1;
    }

    private int FindBestPosition(int[] suffixCount, int lettersStillNeeded, int cursor, int windowEnd)
    {
        var bestPosition = -1;

        for (var p = cursor; p <= windowEnd; p++)
        {
            var remaining = lettersStillNeeded - (_s[p] == Letter ? 1 : 0);
            if (remaining < 0)
            {
                remaining = 0;
            }

            if (suffixCount[p + 1] < remaining)
            {
                continue;
            }

            if (bestPosition == -1 || _s[p] < _s[bestPosition])
            {
                bestPosition = p;
            }
        }

        return bestPosition;
    }

    private sealed class ScanProgress
    {
        public int Cursor { get; set; }

        public int LettersUsed { get; set; }
    }

    [Benchmark]
    public string MonotonicStackSweep()
    {
        var n = _s.Length;
        var suffixCount = BuildSuffixCount(_s, Letter);

        var stack = new RepoCharStack();
        var lettersInStack = 0;

        for (var i = 0; i < n; i++)
        {
            ProcessCharacter(stack, ref lettersInStack, i, suffixCount);
        }

        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    private void ProcessCharacter(RepoCharStack stack, ref int lettersInStack, int index, int[] suffixCount)
    {
        ShrinkWhileNotFeasible(stack, ref lettersInStack, index, suffixCount);

        if (stack.Count >= _k)
        {
            return;
        }

        TryPushCharacter(stack, ref lettersInStack, _s[index]);
    }

    private void ShrinkWhileNotFeasible(RepoCharStack stack, ref int lettersInStack, int index, int[] suffixCount)
    {
        var n = _s.Length;
        var c = _s[index];

        while (stack.TryPeek(out var top)
               && top > c
               && stack.Count + (n - index) > _k
               && (top != Letter || lettersInStack - 1 + suffixCount[index] >= Repetition))
        {
            stack.TryPop(out _);
            if (top == Letter)
            {
                lettersInStack--;
            }
        }
    }

    private void TryPushCharacter(RepoCharStack stack, ref int lettersInStack, char c)
    {
        if (c == Letter)
        {
            stack.Push(c);
            lettersInStack++;
        }
        else if (_k - stack.Count > Repetition - lettersInStack)
        {
            stack.Push(c);
        }
    }

    private static int[] BuildSuffixCount(string s, char letter)
    {
        var suffixCount = new int[s.Length + 1];
        for (var i = s.Length - 1; i >= 0; i--)
        {
            suffixCount[i] = suffixCount[i + 1] + (s[i] == letter ? 1 : 0);
        }

        return suffixCount;
    }
}
