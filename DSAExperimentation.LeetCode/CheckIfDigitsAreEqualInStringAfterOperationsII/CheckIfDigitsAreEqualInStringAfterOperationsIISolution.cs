using DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsI;

namespace DSAExperimentation.LeetCode.CheckIfDigitsAreEqualInStringAfterOperationsII;

// LeetCode 3463. Check If Digits Are Equal in String After Operations II: the same
// reduction as Part I - repeatedly replace s with the length-(n-1) string of
// (s[i]+s[i+1]) % 10 - but s.Length is now up to 10^5, so Part I's row-by-row
// Pascal's triangle build (O(n^2)) is no longer an option for the composed arm.
//
// After k = n - 2 reduction steps, the two surviving digits are
// final[i] = sum_j C(k, j) * original[i + j] mod 10 (Part I's own derivation).
// Computing each C(k, j) mod 10 directly still means a full Pascal row; instead
// this splits mod 10 = mod 2 * mod 5 (coprime) via Lucas' theorem on each small
// prime and recombines with the Chinese remainder theorem, which prices every
// coefficient at O(log n) instead of O(n).
internal static class CheckIfDigitsAreEqualInStringAfterOperationsIISolution
{
    private const int Modulo = 10;
    private const int ModTwo = 2;
    private const int ModFive = 5;

    private static readonly int[,] BinomialModFiveTable = BuildBinomialModFiveTable();

    // The textbook answer: perform the reduction exactly as stated. O(n^2) - the
    // arm the Lucas strategy has to beat once n approaches the 10^5 bound, and the
    // very reduction LC 3461 asks for at a bound that stays quadratic-friendly, so
    // Part I's own baseline is the one implementation of it rather than a second
    // copy restating the same loop under this file's own modulus name.
    public static bool IsEqualByAdjacentSumReduction(string digits) =>
        CheckIfDigitsAreEqualInStringAfterOperationsISolution.IsEqualByAdjacentSumReduction(digits);

    public static bool IsEqualByLucasBinomialCoefficients(string digits)
    {
        var steps = digits.Length - 2;
        var first = 0;
        var second = 0;

        for (var j = 0; j <= steps; j++)
        {
            var coefficient = BinomialModTen(steps, j);
            first = (first + coefficient * (digits[j] - '0')) % Modulo;
            second = (second + coefficient * (digits[j + 1] - '0')) % Modulo;
        }

        return first == second;
    }

    private static int BinomialModTen(int totalCount, int selectedCount)
    {
        var modTwo = BinomialModTwo(totalCount, selectedCount);
        var modFive = BinomialModFive(totalCount, selectedCount);

        return CombineByChineseRemainder(modTwo, modFive);
    }

    // Kummer's theorem specialization for p = 2: C(totalCount, selectedCount) is
    // odd exactly when adding selectedCount and totalCount - selectedCount in
    // binary never carries, i.e. every set bit of selectedCount is also set in
    // totalCount.
    private static int BinomialModTwo(int totalCount, int selectedCount) =>
        IsBinomialCoefficientOdd(totalCount, selectedCount) ? 1 : 0;

    private static bool IsBinomialCoefficientOdd(int totalCount, int selectedCount) =>
        (selectedCount & ~totalCount) == 0;

    // Lucas' theorem for p = 5: split totalCount and selectedCount into base-5
    // digits and multiply the per-digit binomial coefficients mod 5, which is 0
    // the moment a digit of selectedCount exceeds the matching digit of totalCount.
    private static int BinomialModFive(int totalCount, int selectedCount)
    {
        var result = 1;

        while (selectedCount > 0)
        {
            var totalDigit = totalCount % ModFive;
            var selectedDigit = selectedCount % ModFive;

            if (selectedDigit > totalDigit)
            {
                return 0;
            }

            result = result * BinomialModFiveTable[totalDigit, selectedDigit] % ModFive;
            totalCount /= ModFive;
            selectedCount /= ModFive;
        }

        return result;
    }

    // The unique x in [0, 10) with x mod 2 == modTwo and x mod 5 == modFive:
    // modFive already has the right value mod 5, so it is the answer whenever its
    // parity already matches, otherwise modFive + 5 keeps the mod-5 residue and
    // flips the parity.
    private static int CombineByChineseRemainder(int modTwo, int modFive) =>
        IsParityAlreadyMatched(modFive, modTwo) ? modFive : ParityFlipped(modFive);

    private static bool IsParityAlreadyMatched(int modFive, int modTwo) => modFive % ModTwo == modTwo;

    private static int ParityFlipped(int modFive) => modFive + ModFive;

    // C(a, b) mod 5 for 0 <= b <= a <= 4 - the only inputs Lucas' theorem ever
    // hands BinomialModFive - built once as Pascal's triangle rows 0..4 mod 5.
    private static int[,] BuildBinomialModFiveTable()
    {
        var table = new int[ModFive, ModFive];

        for (var row = 0; row < ModFive; row++)
        {
            table[row, 0] = 1;

            for (var col = 1; col <= row; col++)
            {
                table[row, col] = (table[row - 1, col - 1] + table[row - 1, col]) % ModFive;
            }
        }

        return table;
    }
}
