using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfComputerUnlockingPermutations;

// LeetCode 3577. Count the Number of Computer Unlocking Permutations: computer 0
// starts unlocked; computer i (i > 0) can only be unlocked once some already-
// unlocked j < i has complexity[j] < complexity[i]. Count the permutations of
// [0, n-1] that are legal unlock orders, modulo 1e9+7.
internal static class CountTheNumberOfComputerUnlockingPermutationsSolution
{
    // Enumerate every legal unlock order by DFS: at each step, only a computer with
    // some earlier, already-unlocked index of strictly lower complexity is a legal
    // next choice. This is exactly Backtrack.Search's "count the leaves reached"
    // shape (N-Queens, Combination Sum) - the arm the closed form below has to
    // justify itself against, with none of that formula's insight built in.
    public static long CountUnlockingPermutationsByBacktracking(int[] complexity)
    {
        var state = new UnlockState(complexity);
        var count = 0L;

        Backtrack.Search<UnlockState, int>(
            state,
            IsFullyUnlocked,
            Candidates,
            static (s, choice) => s.Unlock(choice),
            static (s, choice) => s.Relock(choice),
            _ => count++);

        return count % ModularArithmetic.Modulo;
    }

    private static bool IsFullyUnlocked(UnlockState state) => state.UnlockedCount == state.Complexity.Length;

    private static IEnumerable<int> Candidates(UnlockState state)
    {
        for (var i = 0; i < state.Complexity.Length; i++)
        {
            if (!state.IsUnlocked(i) && state.HasUnlockerFor(i))
            {
                yield return i;
            }
        }
    }

    // The insight that collapses the search: index 0 is always < every other index
    // and starts unlocked, so it is a legal unlocker for computer i the moment
    // complexity[i] > complexity[0] - and if that ever fails, no other index can
    // unlock i either, since 0 is the smallest complexity any legal unlocker of a
    // solvable instance could have. Once every computer clears that one check, unlock
    // order is unconstrained: any permutation of 1..n-1 works, so the count is
    // exactly (n-1)!.
    public static long CountUnlockingPermutationsByFactorialFormula(int[] complexity)
    {
        for (var i = 1; i < complexity.Length; i++)
        {
            if (complexity[i] <= complexity[0])
            {
                return 0;
            }
        }

        var result = 1L;

        for (var i = 2; i < complexity.Length; i++)
        {
            result = result * i % ModularArithmetic.Modulo;
        }

        return result;
    }

    // Mutable DFS state for Backtrack.Search: which computers are unlocked so far,
    // aliased (not copied) across Choose/Unchoose the way Backtrack.TState requires.
    private sealed class UnlockState
    {
        private readonly bool[] _unlocked;

        public int[] Complexity { get; }

        public int UnlockedCount { get; private set; }

        public UnlockState(int[] complexity)
        {
            Complexity = complexity;
            _unlocked = new bool[complexity.Length];
            _unlocked[0] = true;
            UnlockedCount = 1;
        }

        public bool IsUnlocked(int computerIndex) => _unlocked[computerIndex];

        public bool HasUnlockerFor(int computerIndex)
        {
            for (var j = 0; j < computerIndex; j++)
            {
                if (_unlocked[j] && Complexity[j] < Complexity[computerIndex])
                {
                    return true;
                }
            }

            return false;
        }

        public void Unlock(int computerIndex)
        {
            _unlocked[computerIndex] = true;
            UnlockedCount++;
        }

        public void Relock(int computerIndex)
        {
            _unlocked[computerIndex] = false;
            UnlockedCount--;
        }
    }
}
