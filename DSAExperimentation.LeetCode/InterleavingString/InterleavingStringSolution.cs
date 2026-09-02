using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.InterleavingString;

// LeetCode 97. Interleaving String: can target be formed by interleaving first and
// second, preserving each string's own internal character order?
//
// The state (i, j) - how many characters of first/second have been consumed - fixes
// the next position in target (k = i + j), so this is a single memoized recursion
// over one two-dimensional state space rather than two independent walks.
internal static class InterleavingStringSolution
{
    public static bool IsInterleaveByMemoizedRecursion(string first, string second, string target)
    {
        if (first.Length + second.Length != target.Length)
        {
            return false;
        }

        return Memoizer.Memoize<(int First, int Second), bool>((0, 0), CanBuild);

        bool CanBuild((int First, int Second) state, Func<(int First, int Second), bool> build)
        {
            var (i, j) = state;
            var k = i + j;

            return k == target.Length
                || (i < first.Length && first[i] == target[k] && build((i + 1, j)))
                || (j < second.Length && second[j] == target[k] && build((i, j + 1)));
        }
    }
}
