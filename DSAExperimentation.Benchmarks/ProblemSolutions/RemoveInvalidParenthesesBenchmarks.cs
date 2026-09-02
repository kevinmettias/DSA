using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Invalid Parentheses (LC 301): the brute-force baseline most people reach for
// first - enumerate every one of the 2^n character subsets and keep the longest valid
// one - vs. this repo's own Queue<string> (BFS frontier), Set<string> (visited dedup),
// and Stack<char> (the same LC20 ValidParentheses bracket-matching check) doing a
// level-by-level "remove one paren" search that stops at the first level containing
// any valid string, exploring only candidates within the true minimum removal
// distance instead of every possible subset. [Length] stays small since brute force
// is genuinely O(2^n * n); the generated input always carries exactly 2 unmatched
// leading '(' characters, so both strategies do real removal work.
[MemoryDiagnoser]
public class RemoveInvalidParenthesesBenchmarks
{
    // Splits Length in half to build the generated input's opener/closer counts.
    private const int HalfDivisor = 2;

    [Params(14, 20)]
    public int Length;

    private string _input = null!;

    [GlobalSetup]
    public void Setup() => _input = new string('(', (Length / HalfDivisor) + 1) + new string(')', (Length / HalfDivisor) - 1);

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubsets()
    {
        var maxLength = 0;
        var totalMasks = 1 << _input.Length;

        for (var mask = 0; mask < totalMasks; mask++)
        {
            var chars = new List<char>();

            for (var i = 0; i < _input.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    chars.Add(_input[i]);
                }
            }

            if (chars.Count > maxLength && IsValid(new string(chars.ToArray())))
            {
                maxLength = chars.Count;
            }
        }

        return maxLength;
    }

    [Benchmark]
    public int QueueBfsMinimalRemoval()
    {
        var results = RemoveInvalidParentheses(_input);
        return results.Count > 0 ? results[0].Length : 0;
    }

    private static List<string> RemoveInvalidParentheses(string s)
    {
        var visited = new Set<string>();
        var queue = new RepoQueue();
        visited.TryAdd(s);
        queue.Enqueue(s);

        while (queue.Count > 0)
        {
            var validAtThisLevel = ProcessLevel(queue, visited);

            if (validAtThisLevel is not null)
            {
                return validAtThisLevel;
            }
        }

        return [];
    }

    private static List<string>? ProcessLevel(RepoQueue queue, Set<string> visited)
    {
        var levelSize = queue.Count;
        var validAtThisLevel = new List<string>();

        for (var i = 0; i < levelSize; i++)
        {
            queue.TryDequeue(out var candidate);

            if (IsValid(candidate))
            {
                validAtThisLevel.Add(candidate);
                continue;
            }

            EnqueueValidRemovals(candidate, queue, visited);
        }

        return validAtThisLevel.Count > 0 ? validAtThisLevel : null;
    }

    private static void EnqueueValidRemovals(string candidate, RepoQueue queue, Set<string> visited)
    {
        for (var j = 0; j < candidate.Length; j++)
        {
            if (candidate[j] != '(' && candidate[j] != ')')
            {
                continue;
            }

            var next = candidate.Remove(j, 1);

            if (visited.TryAdd(next))
            {
                queue.Enqueue(next);
            }
        }
    }

    private static bool IsValid(string s)
    {
        var openers = new RepoStack();

        foreach (var ch in s)
        {
            if (ch == '(')
            {
                openers.Push(ch);
            }
            else if (ch == ')' && !openers.TryPop(out _))
            {
                return false;
            }
        }

        return openers.Count == 0;
    }
}
