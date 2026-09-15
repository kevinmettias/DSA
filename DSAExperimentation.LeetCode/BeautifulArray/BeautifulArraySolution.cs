using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.BeautifulArray;

// LeetCode 932. Beautiful Array: produce any permutation of 1..n in which no
// element is the arithmetic mean of one element before it and one after it -
// formally, no i < k < j with 2 * a[k] == a[i] + a[j].
//
// LeetCode accepts ANY beautiful permutation, so the two strategies here are not
// required to return the same array: the backtracking arm returns the
// lexicographically smallest one, the divide-and-conquer arm returns the one its
// recursion builds, and they first diverge at n = 8.
internal static class BeautifulArraySolution
{
    // 2 * a[k] == a[i] + a[j] is the averaging rule the whole problem is about.
    private const int ArithmeticMeanMultiplier = 2;

    // Each recursion level halves the range, then rescales a value into an odd
    // (2x - 1) or an even (2x) slot.
    private const int ProblemSizeDivisor = 2;

    private const int RangeScaleFactor = 2;

    private const int SmallestValue = 1;

    // The textbook answer: place candidate values left to right and reject a prefix
    // the instant its newest element completes an averaging triple, backtracking
    // when a position runs out of candidates. Exponential - n = 16 already costs
    // ~13M search nodes - and written with BCL arrays only, because this is what
    // you reach for before noticing the parity argument below.
    public static int[] ConstructByPrunedBacktracking(int n)
    {
        var search = new PrefixSearch(n, new bool[n + 1], new int[n]);
        Extend(0, search);

        return search.Values;
    }

    // Top-down divide and conquer: if Build(m) is beautiful over 1..m, then mapping
    // it through 2x - 1 gives a beautiful arrangement of the odds and through 2x a
    // beautiful arrangement of the evens, and concatenating odds before evens stays
    // beautiful - a[i] + a[j] with one odd and one even is odd, so it can never be
    // 2 * a[k], and any triple within one half is the recursive property. This
    // repo's own HashMap<int, int[]> memoizes the O(log n) distinct subproblem
    // sizes the two recursive calls keep revisiting.
    public static int[] ConstructByMemoizedDivideAndConquer(int n) =>
        Build(n, new HashMap<int, int[]>());

    private static bool Extend(int position, PrefixSearch search)
    {
        if (position == search.Length)
        {
            return true;
        }

        for (var candidate = SmallestValue; candidate <= search.Length; candidate++)
        {
            if (TryPlaceCandidate(position, candidate, search))
            {
                return true;
            }
        }

        return false;
    }

    // One candidate at one position: skip it when it is spent or breaks the prefix,
    // otherwise place it and recurse. True means the search below completed, so the
    // caller stops too; false means try the next candidate.
    private static bool TryPlaceCandidate(int position, int candidate, PrefixSearch search)
    {
        if (search.Used[candidate])
        {
            return false;
        }

        search.Values[position] = candidate;

        if (!IsValidPrefix(search.Values, position))
        {
            return false;
        }

        search.Used[candidate] = true;

        if (Extend(position + 1, search))
        {
            return true;
        }

        search.Used[candidate] = false;
        return false;
    }

    // Everything before lastIndex is already beautiful, so only triples whose right
    // end is the newly placed element can be newly broken.
    private static bool IsValidPrefix(int[] values, int lastIndex)
    {
        for (var i = 0; i < lastIndex; i++)
        {
            for (var k = i + 1; k < lastIndex; k++)
            {
                if ((ArithmeticMeanMultiplier * values[k]) == values[i] + values[lastIndex])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static int[] Build(int n, HashMap<int, int[]> memo)
    {
        if (n == SmallestValue)
        {
            return [SmallestValue];
        }

        if (memo.TryGetValue(n, out var cached))
        {
            return cached;
        }

        var odds = Build((n + 1) / ProblemSizeDivisor, memo).Select(x => (RangeScaleFactor * x) - 1);
        var evens = Build(n / ProblemSizeDivisor, memo).Select(x => RangeScaleFactor * x);
        var result = odds.Concat(evens).ToArray();

        memo.Set(n, result);

        return result;
    }

    // The backtracking arm's mutable state: the permutation being built, and which
    // values it has already spent.
    private readonly record struct PrefixSearch(int Length, bool[] Used, int[] Values);
}
