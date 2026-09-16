namespace DSAExperimentation.LeetCode.SortedGcdPairQueries;

// LeetCode 3312. Sorted GCD Pair Queries: gcdPairs is gcd(nums[i], nums[j]) for
// every 0 <= i < j < n, sorted ascending; each query asks for gcdPairs at that
// index.
//
// The brute-force arm actually materializes and sorts all C(n,2) gcds - the
// literal reading of the problem statement, and the arm the sieve strategy below
// has to beat. GcdPairCountIndex turns "sort every pair's gcd" into "count pairs
// by gcd value instead of enumerating them," which this repo's own
// BinarySearch.LowerBound then answers in O(log maxValue) per query.
internal static class SortedGcdPairQueriesSolution
{
    public static int[] AnswerQueriesByBruteForce(int[] nums, int[] queries)
    {
        var pairCount = nums.Length * (nums.Length - 1) / 2;
        var gcds = new int[pairCount];
        var next = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                gcds[next++] = Gcd(nums[i], nums[j]);
            }
        }

        Array.Sort(gcds);

        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = gcds[queries[i]];
        }

        return answers;
    }

    private static int Gcd(int first, int second) => second == 0 ? first : Gcd(second, first % second);

    // This repo's own BinarySearch.LowerBound over a sieve-built cumulative count
    // array, so answering a query never touches an individual pair.
    public static int[] AnswerQueriesByGcdCountingSieve(int[] nums, int[] queries)
    {
        var index = GcdPairCountIndex.Build(nums);

        return AnswerQueriesByGcdCountingSieve(index, queries);
    }

    public static int[] AnswerQueriesByGcdCountingSieve(GcdPairCountIndex index, int[] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = index.KthSmallestGcd(queries[i]);
        }

        return answers;
    }
}
