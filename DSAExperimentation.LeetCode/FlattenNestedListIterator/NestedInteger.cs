namespace DSAExperimentation.LeetCode.FlattenNestedListIterator;

// Mirrors LeetCode's own NestedInteger union type: a node is either a single
// integer or a list of further NestedIntegers, never both. Answers LC 341 alone -
// no fixed content of its own (no modulus, no vertex set), just the shape LC's
// own interface hands every strategy here, so it lives beside the solution rather
// than in Domain/ (ARCHITECTURE.md #17.6), the same call CourseNode makes for
// CourseSchedule/CourseScheduleII.
internal sealed class NestedInteger(int value, List<NestedInteger>? list)
{
    public bool IsInteger => list is null;

    public int Value => IsInteger ? value : ThrowNotAnInteger();

    public List<NestedInteger> Elements => list ?? throw new InvalidOperationException("Not a list.");

    public static NestedInteger OfInteger(int value) => new(value, null);

    public static NestedInteger OfList(params NestedInteger[] elements) => new(0, [.. elements]);

    // `Value`'s non-integer arm: a node that holds a list has no single integer.
    private static int ThrowNotAnInteger() =>
        throw new InvalidOperationException("Not an integer.");
}
