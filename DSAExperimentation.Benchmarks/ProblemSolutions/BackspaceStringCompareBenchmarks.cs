using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Backspace String Compare (LC 844): the BCL's own Stack<char> replaying each
// string's keystrokes vs. this repo's own Stack<char> (DynamicArray-backed) doing
// exactly the same push-on-letter/pop-on-'#' replay - the same "same algorithm, BCL
// structure vs. repo structure" contrast OpenTheLockBenchmarks' Queue<string> vs.
// Reduce.Graph already draws for LC 752. _s and _t are built from the same seed, so
// both benchmarks are forced through their full replay of both strings instead of
// short-circuiting on an early character mismatch.
[MemoryDiagnoser]
public class BackspaceStringCompareBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _t = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s = BuildKeystrokes(Length, seed: 844);
        _t = BuildKeystrokes(Length, seed: 844);
    }

    [Benchmark(Baseline = true)]
    public bool BclStack() => ProcessBcl(_s) == ProcessBcl(_t);

    [Benchmark]
    public bool RepoStack() => ProcessRepo(_s) == ProcessRepo(_t);

    private static string ProcessBcl(string input)
    {
        var stack = new Stack<char>();

        foreach (var ch in input)
        {
            if (ch == '#')
            {
                if (stack.Count > 0)
                {
                    stack.Pop();
                }
            }
            else
            {
                stack.Push(ch);
            }
        }

        var chars = stack.ToArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    private static string ProcessRepo(string input)
    {
        var stack = new RepoCharStack();

        foreach (var ch in input)
        {
            if (ch == '#')
            {
                stack.TryPop(out _);
            }
            else
            {
                stack.Push(ch);
            }
        }

        var chars = new char[stack.Count];

        for (var i = chars.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out chars[i]);
        }

        return new string(chars);
    }

    // ~20% backspaces so the stack genuinely grows and shrinks instead of only ever
    // growing - the same "reachable, real work" shaping LockGraphs/PuzzleGraphs
    // already document for their own random benchmark inputs.
    private static string BuildKeystrokes(int length, int seed)
    {
        var random = new Random(seed);
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = random.Next(5) == 0 ? '#' : (char)('a' + random.Next(26));
        }

        return new string(chars);
    }
}
