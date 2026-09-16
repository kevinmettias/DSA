using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountValidSequences;

// LeetCode 4002. Count Valid Sequences: sequences of k positive integers summing
// to n, whose product is even, counted modulo 1e9+7.
//
// "Even product" is easier to count as its complement: total sequences minus the
// sequences whose product is ODD, i.e. every entry is odd. By stars-and-bars,
// sequences of k positive integers summing to n number C(n - 1, k - 1). An
// all-odd sequence substitutes x_i = 2y_i - 1 (y_i >= 1); summing to n forces
// sum(y_i) = (n + k) / 2, only possible when n + k is even, and then there are
// C((n + k) / 2 - 1, k - 1) of them (the same stars-and-bars count, one level
// down). The answer is the difference of the two binomial coefficients mod p.
//
// Both strategies compute that same difference; they differ only in how nCr(n, r)
// itself is produced - recomputed on the spot, or read from a table prepared once
// (the same recomputed-vs-precomputed-factorials split CountWaysToBuildRoomsIn
// AnAntColony uses, here without a tree to fold over).
internal static class CountValidSequencesSolution
{
    // The textbook arm: no precomputed table, each nCr(n, r) multiplies out its
    // own numerator and denominator from scratch and divides by one modular
    // inverse - correct on its own, but O(k) of work per call with no reuse
    // across the two coefficients this problem always needs.
    public static int CountByDirectBinomial(int targetSum, int length)
    {
        var total = ChooseDirect(targetSum - 1, length - 1);
        var odd = HasAllOddSequences(targetSum, length)
            ? ChooseDirect((targetSum + length) / 2 - 1, length - 1)
            : 0;

        return Difference(total, odd);
    }

    // The composed arm: FactorialTable.Build(targetSum) does the same O(n) factorial
    // work once, up front, so both nCr lookups below are O(1).
    public static int CountByPrecomputedFactorials(int targetSum, int length) =>
        CountByPrecomputedFactorials(FactorialTable.Build(targetSum), targetSum, length);

    public static int CountByPrecomputedFactorials(FactorialTable table, int targetSum, int length)
    {
        var total = table.Choose(targetSum - 1, length - 1);
        var odd = HasAllOddSequences(targetSum, length)
            ? table.Choose((targetSum + length) / 2 - 1, length - 1)
            : 0;

        return Difference(total, odd);
    }

    private static int Difference(long total, long odd) =>
        (int)((total - odd + ModularArithmetic.Modulo) % ModularArithmetic.Modulo);

    // An all-odd sequence exists only when n + k is even: the substitution
    // x_i = 2y_i - 1 makes the k odd entries sum to (n + k) / 2, which has to be a
    // whole number of positive y_i.
    private static bool HasAllOddSequences(int targetSum, int length) => (targetSum + length) % 2 == 0;

    // nCr is zero whenever itemCount or chooseCount is negative, or chooseCount
    // overshoots itemCount.
    private static bool IsOutsideBinomialRange(int itemCount, int chooseCount)
        => itemCount < 0 || chooseCount < 0 || chooseCount > itemCount;

    private static long ChooseDirect(int itemCount, int chooseCount)
    {
        if (IsOutsideBinomialRange(itemCount, chooseCount))
        {
            return 0;
        }

        chooseCount = Math.Min(chooseCount, itemCount - chooseCount);
        var numerator = 1L;
        var denominator = 1L;

        for (var i = 0; i < chooseCount; i++)
        {
            numerator = numerator * (itemCount - i) % ModularArithmetic.Modulo;
            denominator = denominator * (i + 1) % ModularArithmetic.Modulo;
        }

        return numerator * ModularArithmetic.Inverse(denominator) % ModularArithmetic.Modulo;
    }
}
