using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignEventManager;

// LeetCode 3885. Design Event Manager: an instance API (updatePriority/pollHighest)
// rather than a pure function, so "every strategy for the problem" (§17.3) takes
// the form of two classes implementing the shared IEventManagerStrategy surface
// below - the same shape DesignTaskManagerSolution uses for its own instance-API
// design problem (LC 3408), just without an Add/Rmv pair since this problem's
// events are all fixed at construction and only ever updated or polled away, never
// added afterward.
internal static class DesignEventManagerSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IEventManagerStrategy
    {
        void UpdatePriority(int eventId, int newPriority);

        int PollHighest();
    }

    // The textbook answer: a BCL Dictionary<eventId, priority> and a full linear
    // scan for the highest-priority event on every PollHighest - deliberately
    // without this repo's Heap, the arm the lazy-deletion heap strategy below has
    // to justify itself against.
    internal sealed class EventManagerByLinearScan : IEventManagerStrategy
    {
        private readonly Dictionary<int, int> _priorityByEventId = new();

        public EventManagerByLinearScan(IEnumerable<(int EventId, int Priority)> events)
        {
            foreach (var (eventId, priority) in events)
            {
                _priorityByEventId[eventId] = priority;
            }
        }

        public void UpdatePriority(int eventId, int newPriority) => _priorityByEventId[eventId] = newPriority;

        public int PollHighest()
        {
            if (_priorityByEventId.Count == 0)
            {
                return LeetCodeAnswer.None;
            }

            var top = _priorityByEventId.Aggregate((best, next) => IsHigherPriority(next, best) ? next : best);
            _priorityByEventId.Remove(top.Key);
            return top.Key;
        }

        private static bool IsHigherPriority(KeyValuePair<int, int> candidate, KeyValuePair<int, int> incumbent)
            => candidate.Value != incumbent.Value
                ? candidate.Value > incumbent.Value
                : candidate.Key < incumbent.Key;
    }

    // This repo's own Heap<Element, MaxHeapOrder> ordered by EventEntry's
    // (Priority, reversed EventId) comparison, plus a HashMap<eventId, EventEntry>
    // holding each event's current authoritative state. UpdatePriority pushes a
    // fresh EventEntry rather than mutating the heap in place - Heap has no
    // decrease-key or arbitrary remove (see Heap.cs) - so PollHighest lazily
    // discards popped entries that no longer match the HashMap's current record
    // for that eventId, the standard lazy-deletion trick for a priority queue
    // without one (the same shape DesignTaskManagerSolution's own
    // TaskManagerByLazyDeletionHeap already uses for a per-task heap).
    internal sealed class EventManagerByLazyDeletionHeap : IEventManagerStrategy
    {
        private readonly Heap<EventEntry, MaxHeapOrder<EventEntry>> _heap = new();
        private readonly HashMap<int, EventEntry> _current = new();

        public EventManagerByLazyDeletionHeap(IEnumerable<(int EventId, int Priority)> events)
        {
            foreach (var (eventId, priority) in events)
            {
                Push(new EventEntry(priority, eventId));
            }
        }

        public void UpdatePriority(int eventId, int newPriority) => Push(new EventEntry(newPriority, eventId));

        public int PollHighest()
        {
            while (_heap.TryPop(out var candidate))
            {
                if (_current.TryGetValue(candidate.EventId, out var authoritative) && authoritative == candidate)
                {
                    _current.TryRemove(candidate.EventId);
                    return candidate.EventId;
                }
            }

            return LeetCodeAnswer.None;
        }

        private void Push(EventEntry entry)
        {
            _current.Set(entry.EventId, entry);
            _heap.Push(entry);
        }
    }
}
