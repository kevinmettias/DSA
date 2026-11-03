namespace DSAExperimentation.Algorithms.Walking;

// Whether a walk asks its algebra's IsDone, carried as a type rather than a flag. The walks run
// as shared generic code over a reference-type node, so a flag passed down the recursion is one
// more argument and one more test at every node, and on OperationsOnTree's walks of one node apiece
// that measured 20%. A type argument is fixed when the walk is compiled: the instantiation for
// NoStopCheck is the walk with no check in it at all, and the one for AlgebraStopCheck asks after
// each event. The order strategy picks one from the algebra's StopsEarly, once per walk, and that
// read folds to a constant wherever the strategy is inlined into a caller with exact types. Neither
// witness is generic over the node, so the walk calls IsEnabled directly and inlines it.
internal interface IStopCheck
{
    /// <summary>Whether the walk asks the algebra's IsDone after each event.</summary>
    /// <returns>True when the walk can stop before its last event.</returns>
    static abstract bool IsEnabled { get; }
}
