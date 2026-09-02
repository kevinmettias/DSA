namespace DSAExperimentation.LeetCode.DesignEventManager;

// Heap element for EventManagerByLazyDeletionHeap: max-heap ordering is priority
// first, then reversed eventId - LC's own tie-break rule for pollHighest (the
// smallest eventId wins on equal priority, the opposite of DesignTaskManager's own
// tie-break, so this can't reuse TaskEntry's CompareTo as-is). IComparable<EventEntry>
// is what DataStructures.Heap.MaxHeapOrder<Element> requires; a witness meaningful
// only to this problem's heap ordering, so it lives here rather than in Domain/
// (§17.3).
internal readonly record struct EventEntry(int Priority, int EventId) : IComparable<EventEntry>
{
    public int CompareTo(EventEntry other)
    {
        var priorityCompare = Priority.CompareTo(other.Priority);
        return priorityCompare != 0 ? priorityCompare : other.EventId.CompareTo(EventId);
    }
}
