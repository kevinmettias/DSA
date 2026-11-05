using DSAExperimentation.DataStructures.KeyedDisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.ValidArrangementOfPairs;

// LC 2097's promise about its input: there exists a valid arrangement of the pairs, so they make one
// path through every pair, which is an Eulerian path of the graph they draw. LeetCode's judge never poses
// an input that breaks it. On pairs in separate pieces, both strategies arrange the piece they start in
// and stop. Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class ValidArrangementOfPairsPromise
{
    public static bool IsKeptBy(int[][] pairs) => IsBalancedForAPath(pairs) && IsOnePiece(pairs);

    // Every number starts as many pairs as it ends, except at most one that starts one more and, with it,
    // one that ends one more.
    private static bool IsBalancedForAPath(int[][] pairs)
    {
        var surplus = new Dictionary<int, int>();

        foreach (var pair in pairs)
        {
            surplus[pair[0]] = surplus.GetValueOrDefault(pair[0]) + 1;
            surplus[pair[1]] = surplus.GetValueOrDefault(pair[1]) - 1;
        }

        var starts = surplus.Values.Count(value => value == 1);
        var ends = surplus.Values.Count(value => value == -1);
        return surplus.Values.All(value => value is >= -1 and <= 1) && starts == ends && starts <= 1;
    }

    // The pairs, read without direction, join all their numbers into one piece.
    private static bool IsOnePiece(int[][] pairs)
    {
        var numbers = pairs.SelectMany(pair => pair).Distinct().ToList();
        var pieces = new KeyedDisjointSet<int>(numbers);

        foreach (var pair in pairs)
        {
            pieces.TryUnion(pair[0], pair[1]);
        }

        return numbers.All(number => pieces.IsConnected(numbers[0], number));
    }
}
