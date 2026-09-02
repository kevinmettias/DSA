namespace DSAExperimentation.LeetCode.DesignTaskManager;

// Heap element for TaskManagerByLazyDeletionHeap: max-heap ordering is priority
// first, then taskId - LC's own tie-break rule for execTop (the highest taskId
// wins on equal priority). IComparable<TaskEntry> is what
// DataStructures.Heap.MaxHeapOrder<Element> requires; a witness meaningful only to
// this problem's heap ordering, so it lives here rather than in Domain/ (§17.3).
internal readonly record struct TaskEntry(int Priority, int TaskId, int UserId) : IComparable<TaskEntry>
{
    public int CompareTo(TaskEntry other)
    {
        var priorityCompare = Priority.CompareTo(other.Priority);
        return priorityCompare != 0 ? priorityCompare : TaskId.CompareTo(other.TaskId);
    }
}
