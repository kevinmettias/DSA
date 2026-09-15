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
        var factorial = BigIntegerFactorialTable(n);
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var total = factorial[oddCount] * factorial[evenCount] * (IsEven(n) ? 2 : 1);

        if (k > total)
        {
            return [];
        }

        return UnrankByBigIntegerRank(n, k, factorial);
    }

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

    // Odds and evens hold the still-unplaced values of each parity, and the
    // cursor carries the rank still to spend plus the parity of the value just
    // placed, which is what pins every later slot's required parity.
    private static int[] UnrankByBigIntegerRank(int n, long k, BigInteger[] factorial)
    {
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var pools = (
            Factorial: factorial,
            Odds: Enumerable.Range(0, oddCount).Select(i => (2 * i) + 1).ToList(),
            Evens: Enumerable.Range(0, evenCount).Select(i => (2 * i) + 2).ToList());
        var cursor = (RemainingRank: (BigInteger)(k - 1), PreviousWasOdd: (bool?)null);
        var result = new int[n];

        for (var position = 0; position < n; position++)
        {
            var (value, nextCursor) = TakeBigIntegerStep(pools, cursor, n, position);
            result[position] = value;
            cursor = nextCursor;
        }

        return result;
    }

    // One unranking step. Position 0 of an even-length permutation is the only
    // slot where both parities are still legal, and it divides by its own
    // block size; every other position takes the parity the previous value
    // forces and divides by that pool's own completion count.
    private static (int Value, (BigInteger RemainingRank, bool? PreviousWasOdd) Cursor) TakeBigIntegerStep(
        (BigInteger[] Factorial, List<int> Odds, List<int> Evens) pools,
        (BigInteger RemainingRank, bool? PreviousWasOdd) cursor, int n, int position)
    {
        if (position == 0 && IsEven(n))
        {
            return TakeBigIntegerOpener(pools, cursor);
        }

        var pickOdd = cursor.PreviousWasOdd is null ? IsOdd(n) : !cursor.PreviousWasOdd.Value;
        var pool = pickOdd ? pools.Odds : pools.Evens;
        var otherCount = pickOdd ? pools.Evens.Count : pools.Odds.Count;
        var stepBlockSize = pools.Factorial[pool.Count - 1] * pools.Factorial[otherCount];
        var poolIndex = (int)(cursor.RemainingRank / stepBlockSize);

        var value = pool[poolIndex];
        pool.RemoveAt(poolIndex);

        return (value, (cursor.RemainingRank % stepBlockSize, pickOdd));
    }

    // Both parities are valid openers here, and since both pools are still full
    // and equally sized, every opener - odd or even - completes the same number
    // of ways. So the k-th opener overall is simply the (index+1)-th smallest
    // value in 1..n; no pool choice is needed yet. Both pools are full at this
    // point, so their counts are the per-parity counts.
    private static (int Value, (BigInteger RemainingRank, bool? PreviousWasOdd) Cursor) TakeBigIntegerOpener(
        (BigInteger[] Factorial, List<int> Odds, List<int> Evens) pools,
        (BigInteger RemainingRank, bool? PreviousWasOdd) cursor)
    {
        var blockSize = pools.Factorial[pools.Odds.Count - 1] * pools.Factorial[pools.Evens.Count];
        var index = (int)(cursor.RemainingRank / blockSize);
        var value = index + 1;
        var valueIsOdd = value % 2 != 0;
        (valueIsOdd ? pools.Odds : pools.Evens).Remove(value);

        return (value, (cursor.RemainingRank % blockSize, valueIsOdd));
    }

    // The composed arm: the same unranking, but each parity pool is a presence
    // FenwickTree (1 = still unused) and "the r-th still-unused value" is this
    // repo's own BinarySearch.LowerBound over the tree's running prefix count -
    // O(log^2 n) per pick instead of List<T>.RemoveAt's O(n) shift - with capped
    // long arithmetic standing in for BigInteger.
    public static int[] KthPermutationByFenwickOrderStatistics(int n, long k)
    {
        var factorial = SaturatingFactorialTable(n);
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var oddEvenWays = SaturatingMultiply(factorial[oddCount], factorial[evenCount]);
        var total = SaturatingMultiply(oddEvenWays, IsEven(n) ? 2 : 1);

        if (k > total)
        {
            return [];
        }

        return UnrankByFenwickOrderStatistics(n, k, factorial);
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

    // The same unranking as the BigInteger arm with a presence FenwickTree per
    // parity, so the cursor also carries how many values each parity still has
    // left - the pool's own completion count is read off it.
    private static int[] UnrankByFenwickOrderStatistics(int n, long k, long[] factorial)
    {
        var oddCount = (n + 1) / 2;
        var evenCount = n / 2;
        var pools = (
            Factorial: factorial,
            OddPresence: new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, oddCount).ToList()),
            EvenPresence: new FenwickTree<int, SumOperation<int>>(Enumerable.Repeat(1, evenCount).ToList()));
        var cursor = (
            RemainingRank: k - 1,
            OddRemaining: oddCount,
            EvenRemaining: evenCount,
            PreviousWasOdd: (bool?)null);
        var result = new int[n];

        for (var position = 0; position < n; position++)
        {
            var (value, nextCursor) = TakeFenwickStep(pools, cursor, n, position);
            result[position] = value;
            cursor = nextCursor;
        }

        return result;
    }

    private static (int Value, (long RemainingRank, int OddRemaining, int EvenRemaining, bool? PreviousWasOdd) Cursor)
        TakeFenwickStep(
            (long[] Factorial, FenwickTree<int, SumOperation<int>> OddPresence,
                FenwickTree<int, SumOperation<int>> EvenPresence) pools,
            (long RemainingRank, int OddRemaining, int EvenRemaining, bool? PreviousWasOdd) cursor,
            int n, int position)
    {
        if (position == 0 && IsEven(n))
        {
            return TakeFenwickOpener(pools, cursor);
        }

        var pickOdd = cursor.PreviousWasOdd is null ? IsOdd(n) : !cursor.PreviousWasOdd.Value;
        var presence = pickOdd ? pools.OddPresence : pools.EvenPresence;
        var poolCount = pickOdd ? cursor.OddRemaining : cursor.EvenRemaining;
        var otherCount = pickOdd ? cursor.EvenRemaining : cursor.OddRemaining;
        var stepBlockSize = SaturatingMultiply(pools.Factorial[poolCount - 1], pools.Factorial[otherCount]);
        var poolIndex = (int)(cursor.RemainingRank / stepBlockSize);

        var slot = SelectAndRemove(presence, poolIndex);
        var value = pickOdd ? OddValue(slot) : EvenValue(slot);

        return (value, (
            cursor.RemainingRank % stepBlockSize,
            pickOdd ? RemainingAfterPick(cursor.OddRemaining) : cursor.OddRemaining,
            pickOdd ? cursor.EvenRemaining : RemainingAfterPick(cursor.EvenRemaining),
            pickOdd));
    }

    // Both pools are full at position 0 of an even-length permutation, so the
    // k-th opener is the (index+1)-th smallest value in 1..n and its own parity
    // decides which pool loses it.
    private static (int Value, (long RemainingRank, int OddRemaining, int EvenRemaining, bool? PreviousWasOdd) Cursor)
        TakeFenwickOpener(
            (long[] Factorial, FenwickTree<int, SumOperation<int>> OddPresence,
                FenwickTree<int, SumOperation<int>> EvenPresence) pools,
            (long RemainingRank, int OddRemaining, int EvenRemaining, bool? PreviousWasOdd) cursor)
    {
        var blockSize = SaturatingMultiply(
            pools.Factorial[pools.OddPresence.Count - 1], pools.Factorial[pools.EvenPresence.Count]);
        var index = (int)(cursor.RemainingRank / blockSize);
        var value = index + 1;
        var valueIsOdd = value % 2 != 0;

        if (valueIsOdd)
        {
            pools.OddPresence.Add(OddSlot(value), -1);
        }
        else
        {
            pools.EvenPresence.Add(EvenSlot(value), -1);
        }

        return (value, (
            cursor.RemainingRank % blockSize,
            valueIsOdd ? RemainingAfterPick(cursor.OddRemaining) : cursor.OddRemaining,
            valueIsOdd ? cursor.EvenRemaining : RemainingAfterPick(cursor.EvenRemaining),
            valueIsOdd));
    }

    private static int OddSlot(int value) => (value - 1) / 2;

    private static int EvenSlot(int value) => (value - 2) / 2;

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

    private static int OddValue(int slot) => (2 * slot) + 1;

    private static int EvenValue(int slot) => (2 * slot) + 2;

    // How many values a parity pool still holds once this step has taken one
    // out of it.
    private static int RemainingAfterPick(int remaining) => remaining - 1;

    private static bool IsEven(int value) => value % 2 == 0;

    private static bool IsOdd(int value) => value % 2 == 1;

    // Capping a product means deciding whether the exact product fits before
    // computing it, so the overflow test and the product it guards are separate.
    private static bool IsProductOverflowing(long left, long right) => left > SaturationCap / right;

    private static long Product(long left, long right) => left * right;

    private static long SaturatingMultiply(long left, long right)
    {
        if (left == 0 || right == 0)
        {
            return 0;
        }

        return IsProductOverflowing(left, right) ? SaturationCap : Product(left, right);
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
