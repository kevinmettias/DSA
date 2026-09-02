namespace DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayI;

// Heap element for MinOperationsByLazyPairHeap: min-heap ordering is sum first,
// then leftmost original index - LC's own tie-break rule ("if multiple such
// pairs exist, choose the leftmost one"). IComparable<PairCandidate> is what
// DataStructures.Heap.MinHeapOrder<Element> requires; a witness meaningful only
// to this problem's heap ordering, so it lives here rather than in Domain/
// (§17.3).
internal readonly record struct PairCandidate(int Sum, int LeftIndex) : IComparable<PairCandidate>
{
    public int CompareTo(PairCandidate other)
    {
        var sumCompare = Sum.CompareTo(other.Sum);
        return sumCompare != 0 ? sumCompare : LeftIndex.CompareTo(other.LeftIndex);
    }
}
