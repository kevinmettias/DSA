namespace DSAExperimentation.DataStructures.SegmentTree;

// The update type of the trivial action, NoAction: it has one value, and that value means
// "nothing". A tree whose update type is this one has nothing it could ever defer, so SegmentTree
// asks for this type, rather than asking NoAction, when it decides to keep no pending tags and to
// compile its push-down out (SegmentTree.cs says why the type is what it asks).
/// <summary>The one update of the trivial action, which changes nothing.</summary>
public readonly struct NoUpdate
{
}
