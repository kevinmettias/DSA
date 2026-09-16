namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// The frontier the shared grid relaxation settles cells from: the BCL-baseline arm fronts it
// with PriorityQueue<TElement,TPriority>, the composed arm with this repo's own Heap, and
// everything between those two is written once. Naming the seam is also what makes "the
// frontier swap is the only difference between the arms" a fact about the code rather than a
// property the two copies of the loop have to be kept in step by hand.
internal interface IArrivalFrontier
{
    // Adds a cell at the arrival second it was just relaxed to. The same cell may be added
    // more than once - a better arrival re-enters it rather than replacing the earlier entry,
    // which is why the search tracks what it has already settled.
    void Insert((int Row, int Col) node, int priority);

    // Removes the pending cell with the smallest arrival second and reports it, or reports
    // false once nothing is left. Reporting rather than throwing is load-bearing: an empty
    // frontier is how the search learns the destination is unreachable.
    bool TryTake(out ((int Row, int Col) Node, int Priority) entry);
}
