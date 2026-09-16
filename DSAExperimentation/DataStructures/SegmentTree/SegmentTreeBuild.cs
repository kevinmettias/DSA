namespace DSAExperimentation.DataStructures.SegmentTree;

// The one recursion both range trees build with: a leaf takes its element straight from the source
// array, an internal node takes the combination of its two children, and [Start, End] narrows over
// SegmentRange.Split exactly as the query/update recursions narrow it - which is what makes every
// node this writes a node those recursions later read.
//
// Extracted for the same reason SegmentRange.Split was: SegmentTree and LazySegmentTree each
// carried their own copy of this body and a repo-wide structural-duplication check flagged the
// pair. Free-standing rather than a method on SegmentTreeArray<Element>: that type's own doc
// comment scopes it to a pure O(1) Get/Set Representation, and a recursion that consults
// ICombineOperation is not the array's business (ARCHITECTURE.md §5 step 3, §17.6's tier split
// between a Representation and the Algorithm that populates it).
internal static class SegmentTreeBuild
{
    // TOperation is an explicit witness argument rather than inferable: Element and TOperation are
    // both unconstrained by the arguments, and the caller's own generic parameters are the only
    // place the pair is known.
    public static void Fill<Element, TOperation>(
        SegmentTreeArray<Element> values,
        SegmentRange range,
        IReadOnlyList<Element> initial)
        where TOperation : struct, ICombineOperation<Element>
    {
        if (range.Start == range.End)
        {
            values.Set(range.Node, initial[range.Start]);
            return;
        }

        var (left, right) = range.Split();

        Fill<Element, TOperation>(values, left, initial);
        Fill<Element, TOperation>(values, right, initial);

        var combined = TOperation.Combine(values.Get(left.Node), values.Get(right.Node));
        values.Set(range.Node, combined);
    }
}
