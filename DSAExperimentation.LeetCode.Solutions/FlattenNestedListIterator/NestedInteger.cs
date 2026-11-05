namespace DSAExperimentation.LeetCode.Solutions.FlattenNestedListIterator;

// Mirrors LeetCode's own NestedInteger union type: a node is either a single
// integer or a list of further NestedIntegers, never both. It is the shape LC's own
// interface hands every strategy here, and LC 341 is the only problem in this tier
// that reads it, so it lives beside the solution rather than at the tier's root,
// which holds what LeetCode fixes for every problem (ARCHITECTURE.md #17.3, #17.11).
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
