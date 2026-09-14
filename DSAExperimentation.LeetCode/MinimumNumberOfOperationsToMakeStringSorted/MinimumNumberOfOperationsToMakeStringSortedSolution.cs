using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeStringSorted;

// LeetCode 1830. Minimum Number of Operations to Make String Sorted: the operation
// the problem describes is exactly the "previous permutation" step, so the number of
// operations is s's 0-indexed rank among the distinct permutations of its own
// multiset of characters, sorted ascending, reported modulo 1e9+7.
//
// Both strategies sweep s left to right accumulating the same rank sum: at each
// position, every not-yet-placed letter smaller than the current one could have led
// the remaining suffix, and each such choice contributes (remaining-1)! divided by
// the factorial of every letter's remaining count - folded here as a running
// inverse-factorial product that costs one multiplication per step instead of a
// recomputation. Domain.Modular supplies the modulus and the Fermat's-little-theorem
// inverse the factorial table needs.
//
// The only thing the two strategies differ in is how they answer "how many
// remaining letters are smaller than this one": a linear scan of the 26-slot
// frequency array, or a PrefixQuery on this repo's own Binary Indexed Tree over
// that same array.
internal static class MinimumNumberOfOperationsToMakeStringSortedSolution
{
    private const int AlphabetSize = 26;

    // The textbook answer without this repo: scan the frequency array for the
    // smaller-letter count at every position. Nothing but BCL arrays inside - only
    // the modulus itself is shared, because 1e9+7 is LeetCode's reporting
    // convention rather than part of the algorithm's character.
    public static int MakeStringSortedByFrequencyScan(string s)
    {
        var (factorial, inverseFactorial) = BuildFactorialTables(s.Length);
        var remaining = BuildFrequencyTable(s);
        var state = new RankState(0L, InverseFactorialProduct(remaining, inverseFactorial), s.Length);

        foreach (var letter in s)
        {
            var index = letter - 'a';
            var smaller = 0;

            for (var candidate = 0; candidate < index; candidate++)
            {
                smaller += remaining[candidate];
            }

            state = Place(state, smaller, remaining[index], factorial);
            remaining[index]--;
        }

        return (int)state.Rank;
    }

    // This repo's FenwickTree<int, SumOperation<int>>, seeded from the same 26-slot
    // frequency array, answers the smaller-letter count as one PrefixQuery and
    // absorbs each placement as one Add - the same value-indexed-Fenwick shape
    // MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits uses for digits.
    public static int MakeStringSortedByFenwickSweep(string s)
    {
        var (factorial, inverseFactorial) = BuildFactorialTables(s.Length);
        var remaining = BuildFrequencyTable(s);
        var counts = new FenwickTree<int, SumOperation<int>>(remaining);
        var state = new RankState(0L, InverseFactorialProduct(remaining, inverseFactorial), s.Length);

        foreach (var letter in s)
        {
            var index = letter - 'a';
            var smaller = index == 0 ? 0 : counts.PrefixQuery(index - 1);

            state = Place(state, smaller, remaining[index], factorial);
            remaining[index]--;
            counts.Add(index, -1);
        }

        return (int)state.Rank;
    }

    // One placement step, shared so the two strategies differ only in how `smaller`
    // was obtained: credit the ranks skipped by every smaller leading letter, then
    // fold this letter's own remaining count back into the inverse-factorial
    // product (it drops from k! to (k-1)! once the letter is consumed).
    private static RankState Place(RankState state, int smaller, int remainingOfLetter, long[] factorial)
    {
        var rank = smaller > 0
            ? (state.Rank + ((long)smaller * factorial[state.Remaining - 1] % ModularArithmetic.Modulo * state.InverseFactorialProduct)) % ModularArithmetic.Modulo
            : state.Rank;

        return new RankState(
            rank,
            state.InverseFactorialProduct * remainingOfLetter % ModularArithmetic.Modulo,
            state.Remaining - 1);
    }

    private static (long[] Factorial, long[] InverseFactorial) BuildFactorialTables(int length)
    {
        var factorial = new long[length + 1];
        var inverseFactorial = new long[length + 1];
        factorial[0] = 1;

        for (var i = 1; i <= length; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[length] = ModularArithmetic.Inverse(factorial[length]);

        for (var i = length; i > 0; i--)
        {
            inverseFactorial[i - 1] = inverseFactorial[i] * i % ModularArithmetic.Modulo;
        }

        return (factorial, inverseFactorial);
    }

    private static int[] BuildFrequencyTable(string s)
    {
        var frequency = new int[AlphabetSize];

        foreach (var letter in s)
        {
            frequency[letter - 'a']++;
        }

        return frequency;
    }

    private static long InverseFactorialProduct(int[] frequency, long[] inverseFactorial)
    {
        var product = 1L;

        foreach (var count in frequency)
        {
            product = product * inverseFactorial[count] % ModularArithmetic.Modulo;
        }

        return product;
    }

    private readonly record struct RankState(long Rank, long InverseFactorialProduct, int Remaining);
}
