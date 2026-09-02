using System.Numerics;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PermutationsIV;

// LeetCode 3470. Permutations IV: the k-th lexicographically smallest permutation
// of 1..n whose adjacent elements always differ in parity (this problem's own
// sense of "alternating" - parity alternation, not the up-down zigzag some other
// problems mean by that word), or [] if fewer than k such permutations exist.
//
// A valid permutation's parity pattern is pinned the instant its first element is
// placed: every later slot's required parity is just "the opposite of whatever
// came before". That means the number of ways to complete ANY valid prefix
// depends only on how many odd/even values remain unused -
// remainingOdds! * remainingEvens! - never on which specific values were already
// placed. So this is the standard factorial-number-system unranking: at each
// position, divide the remaining rank by that completion count to learn which
// remaining candidate (by rank within its parity pool, not by value) goes next.
// Both strategies share that structure; they differ only in how a parity pool
// tracks "which of my values are still unused" and finds the r-th one.
internal static class PermutationsIVSolution
{
    private const long SaturationCap = 2_000_000_000_000_000L; // comfortably above k's 10^15 bound

    // The textbook arm: a BCL List<int> per parity (remove-by-rank is a linear
    // shift) and exact BigInteger factorials - nothing here depends on this repo
    // at all, the arm the Fenwick strategy has to beat.
    public static int[] KthPermutationByBigIntegerRank(int n, long k)
    {
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var factorial = BigIntegerFactorialTable(n);
        var total = factorial[oddCount] * factorial[evenCount] * (n % 2 == 0 ? 2 : 1);

        if (k > total)
        {
            return [];
        }

        var odds = Enumerable.Range(0, oddCount).Select(i => (2 * i) + 1).ToList();
        var evens = Enumerable.Range(0, evenCount).Select(i => (2 * i) + 2).ToList();
        var remainingRank = (BigInteger)(k - 1);
        var result = new int[n];
        bool? previousWasOdd = null;

        for (var position = 0; position < n; position++)
        {
            if (position == 0 && n % 2 == 0)
            {
                // Both parities are valid openers here, and since both pools are
                // still full and equally sized, every opener - odd or even -
                // completes the same number of ways. So the k-th opener overall
                // is simply the (index+1)-th smallest value in 1..n; no pool
                // choice is needed yet.
                var blockSize = factorial[oddCount - 1] * factorial[evenCount];
                var index = (int)(remainingRank / blockSize);
                remainingRank %= blockSize;

                var value = index + 1;
                result[0] = value;
                previousWasOdd = value % 2 != 0;
                (previousWasOdd.Value ? odds : evens).Remove(value);
                continue;
            }

            var pickOdd = previousWasOdd is null ? n % 2 == 1 : !previousWasOdd.Value;
            var pool = pickOdd ? odds : evens;
            var otherCount = pickOdd ? evens.Count : odds.Count;
            var stepBlockSize = factorial[pool.Count - 1] * factorial[otherCount];
            var poolIndex = (int)(remainingRank / stepBlockSize);
            remainingRank %= stepBlockSize;

            result[position] = pool[poolIndex];
            pool.RemoveAt(poolIndex);
            previousWasOdd = pickOdd;
        }

        return result;
    }

    // The composed arm: the same unranking, but each parity pool is a presence
    // FenwickTree (1 = still unused) and "the r-th still-unused value" is this
    // repo's own BinarySearch.LowerBound over the tree's running prefix count -
    // O(log^2 n) per pick instead of List<T>.RemoveAt's O(n) shift - with capped
    // long arithmetic standing in for BigInteger.
    public static int[] KthPermutationByFenwickOrderStatistics(int n, long k)
    {
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var factorial = SaturatingFactorialTable(n);
        var total = SaturatingMultiply(SaturatingMultiply(factorial[oddCount], factorial[evenCount]), n % 2 == 0 ? 2 : 1);

        if (k > total)
        {
            return [];
        }

        var oddPresence = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, oddCount).ToList());
        var evenPresence = new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, evenCount).ToList());
        var oddRemaining = oddCount;
        var evenRemaining = evenCount;
        var remainingRank = k - 1;
        var result = new int[n];
        bool? previousWasOdd = null;

        for (var position = 0; position < n; position++)
        {
            if (position == 0 && n % 2 == 0)
            {
                var blockSize = SaturatingMultiply(factorial[oddCount - 1], factorial[evenCount]);
                var index = (int)(remainingRank / blockSize);
                remainingRank %= blockSize;

                var value = index + 1;
                result[0] = value;
                previousWasOdd = value % 2 != 0;

                if (previousWasOdd.Value)
                {
                    oddPresence.Add(OddSlot(value), -1);
                    oddRemaining--;
                }
                else
                {
                    evenPresence.Add(EvenSlot(value), -1);
                    evenRemaining--;
                }

                continue;
            }

            var pickOdd = previousWasOdd is null ? n % 2 == 1 : !previousWasOdd.Value;
            var presence = pickOdd ? oddPresence : evenPresence;
            var poolCount = pickOdd ? oddRemaining : evenRemaining;
            var otherCount = pickOdd ? evenRemaining : oddRemaining;
            var stepBlockSize = SaturatingMultiply(factorial[poolCount - 1], factorial[otherCount]);
            var poolIndex = (int)(remainingRank / stepBlockSize);
            remainingRank %= stepBlockSize;

            var slot = SelectAndRemove(presence, poolIndex);
            result[position] = pickOdd ? (2 * slot) + 1 : (2 * slot) + 2;
            previousWasOdd = pickOdd;

            if (pickOdd)
            {
                oddRemaining--;
            }
            else
            {
                evenRemaining--;
            }
        }

        return result;
    }

    // Finds the slot holding the (rank)-th still-present value (0-indexed,
    // ascending by slot) via BinarySearch.LowerBound over the tree's running
    // presence count - the first slot whose prefix count reaches rank + 1 - then
    // marks that slot absent.
    private static int SelectAndRemove(FenwickTree<int, SumOperation<int>> presence, int rank)
    {
        var prefixCounts = new PresencePrefixSequence(presence);
        var slot = BinarySearch.LowerBound(prefixCounts, rank + 1);
        presence.Add(slot, -1);
        return slot;
    }

    private static int OddSlot(int value) => (value - 1) / 2;

    private static int EvenSlot(int value) => (value - 2) / 2;

    private static BigInteger[] BigIntegerFactorialTable(int n)
    {
        var table = new BigInteger[n + 1];
        table[0] = BigInteger.One;

        for (var i = 1; i <= n; i++)
        {
            table[i] = table[i - 1] * i;
        }

        return table;
    }

    private static long[] SaturatingFactorialTable(int n)
    {
        var table = new long[n + 1];
        table[0] = 1L;

        for (var i = 1; i <= n; i++)
        {
            table[i] = SaturatingMultiply(table[i - 1], i);
        }

        return table;
    }

    private static long SaturatingMultiply(long left, long right)
    {
        if (left == 0 || right == 0)
        {
            return 0;
        }

        return left > SaturationCap / right ? SaturationCap : left * right;
    }

    // Presence counts are 0/1 per slot, so PrefixQuery(i) - how many of slots
    // 0..i are still present - is monotonically non-decreasing in i: exactly the
    // sorted-ascending precondition BinarySearch.LowerBound leans on.
    private readonly struct PresencePrefixSequence(FenwickTree<int, SumOperation<int>> presence)
        : IRandomAccessSequence<int>
    {
        public int Length => presence.Count;

        public int Get(int index) => presence.PrefixQuery(index);
    }
}
