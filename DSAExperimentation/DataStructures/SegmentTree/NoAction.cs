namespace DSAExperimentation.DataStructures.SegmentTree;

// The trivial action: its only update is NoUpdate, which leaves every range's value as it was and
// composes with itself into itself. SegmentTree<Element, TOperation> is the one tree with this
// action injected, a tree of point updates and range queries (ARCHITECTURE.md §13.10). The
// tree never calls these members, because a tree whose update type is NoUpdate keeps no tags to
// push down; they exist so the configuration states its action like any other, and they obey the
// action's laws.
/// <summary>
/// The action under which nothing is ever pending: point updates only, never a range update.
/// </summary>
/// <typeparam name="Element">The element type of the tree the action belongs to.</typeparam>
public readonly struct NoAction<Element> : IRangeAction<Element, NoUpdate>
{
    public static NoUpdate NoUpdate => default;

    public static NoUpdate ComposeUpdate(NoUpdate outer, NoUpdate inner) => outer;

    public static Element ApplyUpdate(Element aggregate, NoUpdate update, int rangeLength) =>
        aggregate;
}
