using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Finding3DigitEvenNumbers;

// LeetCode 2094. Finding 3-Digit Even Numbers: try every ordered triple of distinct
// array positions (not distinct values - the same digit value at two different
// positions may both be used), skip a leading zero or an odd last digit, and dedupe
// the resulting numbers with this repo's own Set<int> (O(1) membership, the same
// "TryAdd guards a growing result list" shape AccountsMergeTests' HashMap<string,bool>
// email dedupe already uses) before sorting the distinct values with this repo's
// MergeSort over ArrayIndexedSequence - the same "sort with this repo's MergeSort"
// convention SortAnArrayTests/AccountsMergeTests both already use.
public sealed partial class Finding3DigitEvenNumbersTests
{
    [Fact]
    public void FindEvenNumbers_ClassicExample_ReturnsSortedDistinctEvenNumbers()
    {
        int[] digits = [2, 1, 3, 0];

        var evenNumbers = FindEvenNumbers(digits);

        Assert.Equal([102, 120, 130, 132, 210, 230, 302, 310, 312, 320], evenNumbers);
    }

    [Fact]
    public void FindEvenNumbers_RepeatedDigitsAtDifferentPositions_DedupesEqualNumbers()
    {
        int[] digits = [2, 2, 8, 8, 2];

        var evenNumbers = FindEvenNumbers(digits);

        Assert.Equal([222, 228, 282, 288, 822, 828, 882], evenNumbers);
    }

    [Fact]
    public void FindEvenNumbers_NoEvenDigitAvailable_ReturnsEmpty()
    {
        int[] digits = [3, 7, 5];

        var evenNumbers = FindEvenNumbers(digits);

        Assert.Empty(evenNumbers);
    }

    private static int[] FindEvenNumbers(int[] digits)
    {
        var seen = new Set<int>();
        var found = new List<int>();

        CollectEvenNumbers(digits, seen, found);

        var result = found.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(result));
        return result;
    }

    private static void CollectEvenNumbers(int[] digits, Set<int> seen, List<int> found)
    {
        for (var hundreds = 0; hundreds < digits.Length; hundreds++)
        {
            if (digits[hundreds] == 0)
            {
                continue;
            }

            for (var tens = 0; tens < digits.Length; tens++)
            {
                if (tens == hundreds)
                {
                    continue;
                }

                CollectEvenNumbersWithPrefix(digits, new DigitPrefix(hundreds, tens), seen, found);
            }
        }
    }

    private static void CollectEvenNumbersWithPrefix(int[] digits, DigitPrefix prefix, Set<int> seen, List<int> found)
    {
        for (var ones = 0; ones < digits.Length; ones++)
        {
            if (ones == prefix.Hundreds || ones == prefix.Tens || digits[ones] % 2 != 0)
            {
                continue;
            }

            var number = (digits[prefix.Hundreds] * 100) + (digits[prefix.Tens] * 10) + digits[ones];

            if (seen.TryAdd(number))
            {
                found.Add(number);
            }
        }
    }

    private readonly record struct DigitPrefix(int Hundreds, int Tens);
}
