namespace DSAExperimentation.LeetCode.FlattenNestedListIterator;

// Mirrors LeetCode's own NestedInteger union type: a node is either a single
// integer or a list of further NestedIntegers, never both. Answers LC 341 alone -
// no fixed content of its own (no modulus, no vertex set), just the shape LC's
// own interface hands every strategy here, so it lives beside the solution rather
// than in Domain/ (ARCHITECTURE.md #17.6), the same call CourseNode makes for
// CourseSchedule/CourseScheduleII.
internal sealed class NestedInteger
{
    private readonly List<NestedInteger>? _list;
    private readonly int _value;

    private NestedInteger(int value, List<NestedInteger>? list)
    {
        _value = value;
        _list = list;
    }

    public bool IsInteger => _list is null;

    public int Value => IsInteger ? _value : throw new InvalidOperationException("Not an integer.");

    public List<NestedInteger> Elements => _list ?? throw new InvalidOperationException("Not a list.");

    public static NestedInteger OfInteger(int value) => new(value, null);

    public static NestedInteger OfList(params NestedInteger[] elements) => new(0, [.. elements]);
}
