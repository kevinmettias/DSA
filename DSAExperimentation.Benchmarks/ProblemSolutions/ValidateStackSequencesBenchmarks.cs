using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Validate Stack Sequences (LC 946): exhaustive push/pop-timing backtracking (every
// state tries both "pop now" and "push next," O(2^n) worst case, undoing a branch on
// failure) vs. the single O(n) greedy sweep through this repo's own Stack<int> -
// popping whenever the top matches is provably never wrong, so the greedy sweep never
// has anything to undo. _pushed/_popped are generated as a genuinely valid pair (by
// simulating a random push/pop interleaving) so BacktrackingSearch is forced through
// a real search instead of failing fast on an early mismatch.
[MemoryDiagnoser]
public class ValidateStackSequencesBenchmarks
{
    [Params(10, 16)]
    public int Length;

    private int[] _pushed = null!;
    private int[] _popped = null!;

    private const int RandomSeed = 946;
    private const int BranchChoiceCount = 2;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pushed = Enumerable.Range(0, Length).ToArray();
        _popped = SimulatePushPopSequence(random, Length, _pushed);
    }

    private static int[] SimulatePushPopSequence(Random random, int length, int[] pushed)
    {
        var stack = new System.Collections.Generic.Stack<int>();
        var popped = new List<int>();
        var pushIndex = 0;

        while (popped.Count < length)
        {
            var canPush = pushIndex < length;
            var canPop = stack.Count > 0;

            if (canPush && (!canPop || random.Next(BranchChoiceCount) == 0))
            {
                stack.Push(pushed[pushIndex]);
                pushIndex++;
            }
            else
            {
                popped.Add(stack.Pop());
            }
        }

        return popped.ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BacktrackingSearch() => TryMatch(0, 0, new System.Collections.Generic.Stack<int>());

    private bool TryMatch(int pushIndex, int popIndex, System.Collections.Generic.Stack<int> stack)
    {
        if (popIndex == _popped.Length)
        {
            return true;
        }

        if (TryPopBranch(pushIndex, popIndex, stack))
        {
            return true;
        }

        return TryPushBranch(pushIndex, popIndex, stack);
    }

    private bool TryPopBranch(int pushIndex, int popIndex, System.Collections.Generic.Stack<int> stack)
    {
        if (stack.Count == 0 || stack.Peek() != _popped[popIndex])
        {
            return false;
        }

        var top = stack.Pop();
        if (TryMatch(pushIndex, popIndex + 1, stack))
        {
            return true;
        }

        stack.Push(top);
        return false;
    }

    private bool TryPushBranch(int pushIndex, int popIndex, System.Collections.Generic.Stack<int> stack)
    {
        if (pushIndex >= _pushed.Length)
        {
            return false;
        }

        stack.Push(_pushed[pushIndex]);
        if (TryMatch(pushIndex + 1, popIndex, stack))
        {
            return true;
        }

        stack.Pop();
        return false;
    }

    [Benchmark]
    public bool GreedyStackSweep()
    {
        var stack = new RepoIntStack();
        var popIndex = 0;

        foreach (var value in _pushed)
        {
            stack.Push(value);

            while (stack.TryPeek(out var top) && popIndex < _popped.Length && top == _popped[popIndex])
            {
                stack.TryPop(out _);
                popIndex++;
            }
        }

        return popIndex == _popped.Length;
    }
}
