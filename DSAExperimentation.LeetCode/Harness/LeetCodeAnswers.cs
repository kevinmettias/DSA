namespace DSAExperimentation.LeetCode.Harness;

// The handful of answer-equality rules LeetCode problems actually use, written
// once. These are named helpers a registration passes to MatchingAnswersWith
// explicitly - NOT a default it inherits by saying nothing. That distinction is
// the point: picking IsSetEqual over IsSequenceEqual is a statement about the
// problem ("return the answer in any order"), and a registration that never had
// to make it is a registration where nobody checked.
internal static class LeetCodeAnswers
{
    // LeetCode's own stated tolerance for its floating-point answers.
    private const double DefaultTolerance = 1e-5;

    // Ordinary value equality. Correct for int/bool/string answers, and WRONG for
    // arrays and lists - use IsSequenceEqual for those.
    public static bool IsExactlyEqual<TAnswer>(TAnswer actual, TAnswer expected)
        => EqualityComparer<TAnswer>.Default.Equals(actual, expected);

    public static bool IsSequenceEqual<TItem>(IEnumerable<TItem> actual, IEnumerable<TItem> expected)
        => actual.SequenceEqual(expected);

    // For "you may return the answer in any order": same multiset, any sequence.
    public static bool IsSetEqual<TItem>(IEnumerable<TItem> actual, IEnumerable<TItem> expected)
        where TItem : IComparable<TItem>
        => actual.Order().SequenceEqual(expected.Order());

    // The rows may come back in any order, but each row's own contents may NOT be
    // reordered - the shape K Closest Points to Origin returns, where the set of
    // points is unordered but [x, y] is emphatically not [y, x]. Worth spelling
    // out because the mirror image (ordered rows, unordered contents) is a real
    // shape too, and picking the wrong one passes on LeetCode's own examples
    // whenever the example happens to be symmetric.
    public static bool IsRowSetEqual<TItem>(
        IEnumerable<IEnumerable<TItem>> actual, IEnumerable<IEnumerable<TItem>> expected)
        where TItem : IComparable<TItem>
    {
        var actualRows = OrderRows(actual);
        var expectedRows = OrderRows(expected);

        return actualRows.Count == expectedRows.Count
            && actualRows.Zip(expectedRows).All(pair => pair.First.SequenceEqual(pair.Second));
    }

    private static List<List<TItem>> OrderRows<TItem>(IEnumerable<IEnumerable<TItem>> rows)
        where TItem : IComparable<TItem>
        => rows
            .Select(row => row.ToList())
            .OrderBy(row => row, Comparer<List<TItem>>.Create(CompareRows))
            .ToList();

    private static int CompareRows<TItem>(List<TItem>? left, List<TItem>? right)
        where TItem : IComparable<TItem>
    {
        var leftRow = left ?? [];
        var rightRow = right ?? [];

        for (var index = 0; index < Math.Min(leftRow.Count, rightRow.Count); index++)
        {
            var comparison = leftRow[index].CompareTo(rightRow[index]);

            if (comparison != 0)
            {
                return comparison;
            }
        }

        return leftRow.Count.CompareTo(rightRow.Count);
    }

    public static bool IsSequenceOfSequencesEqual<TItem>(
        IEnumerable<IEnumerable<TItem>> actual, IEnumerable<IEnumerable<TItem>> expected)
    {
        var actualRows = actual.ToList();
        var expectedRows = expected.ToList();

        return actualRows.Count == expectedRows.Count
            && actualRows.Zip(expectedRows).All(pair => pair.First.SequenceEqual(pair.Second));
    }

    public static bool IsWithinTolerance(double actual, double expected, double tolerance = DefaultTolerance)
        => Math.Abs(actual - expected) <= tolerance;
}
