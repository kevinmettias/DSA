using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignTaskManager;

// LeetCode 3408. Design Task Manager: an instance API (add/edit/rmv/execTop) rather
// than a pure function, so "every strategy for the problem" (§17.3) takes the form
// of two full classes implementing the shared ITaskManagerStrategy surface below,
// instead of two static methods sharing an <Operation>By<Strategy> name - a Design
// problem's whole point is a sequence of mutating calls against one instance, so
// there is no separate "prepare input" step to hoist into a benchmark's
// [GlobalSetup] the way OpenTheLock hoists a built graph; each [Benchmark] arm
// constructs its own instance and replays the same call script instead.
internal static class DesignTaskManagerSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ITaskManagerStrategy
    {
        void Add(int userId, int taskId, int priority);

        void Edit(int taskId, int newPriority);

        void Rmv(int taskId);

        int ExecTop();
    }

    // The textbook answer: a BCL Dictionary<taskId, (userId, priority)> and a full
    // linear scan for the highest-priority task on every ExecTop - deliberately
    // without this repo's Heap, the arm the lazy-deletion heap strategy below has to
    // justify itself against.
    internal sealed class TaskManagerByLinearScan : ITaskManagerStrategy
    {
        private readonly Dictionary<int, (int UserId, int Priority)> _tasks = new();

        public TaskManagerByLinearScan(IEnumerable<(int UserId, int TaskId, int Priority)> tasks)
        {
            foreach (var task in tasks)
            {
                Add(task.UserId, task.TaskId, task.Priority);
            }
        }

        public void Add(int userId, int taskId, int priority) => _tasks[taskId] = (userId, priority);

        public void Edit(int taskId, int newPriority) => _tasks[taskId] = (_tasks[taskId].UserId, newPriority);

        public void Rmv(int taskId) => _tasks.Remove(taskId);

        public int ExecTop()
        {
            if (_tasks.Count == 0)
            {
                return LeetCodeAnswer.None;
            }

            var top = _tasks.Aggregate((best, next) => IsHigherPriority(next, best) ? next : best);
            _tasks.Remove(top.Key);
            return top.Value.UserId;
        }

        private static bool IsHigherPriority(
            KeyValuePair<int, (int UserId, int Priority)> candidate,
            KeyValuePair<int, (int UserId, int Priority)> incumbent)
            => candidate.Value.Priority != incumbent.Value.Priority
                ? candidate.Value.Priority > incumbent.Value.Priority
                : candidate.Key > incumbent.Key;
    }

    // This repo's own Heap<Element, MaxHeapOrder> ordered by TaskEntry's
    // (Priority, TaskId) comparison, plus a HashMap<taskId, TaskEntry> holding each
    // task's current authoritative state. Add/Edit both push a fresh TaskEntry
    // rather than mutating the heap in place - Heap has no decrease-key or
    // arbitrary remove (see Heap.cs) - so ExecTop lazily discards popped entries
    // that no longer match the HashMap's current record for that taskId, the
    // standard lazy-deletion trick for a priority queue without one (the same shape
    // DesignAFoodRatingSystem's own FoodRatings.HighestRated already uses for a
    // per-cuisine heap).
    internal sealed class TaskManagerByLazyDeletionHeap : ITaskManagerStrategy
    {
        private readonly Heap<TaskEntry, MaxHeapOrder<TaskEntry>> _heap = new();
        private readonly HashMap<int, TaskEntry> _current = new();

        public TaskManagerByLazyDeletionHeap(IEnumerable<(int UserId, int TaskId, int Priority)> tasks)
        {
            foreach (var task in tasks)
            {
                Add(task.UserId, task.TaskId, task.Priority);
            }
        }

        public void Add(int userId, int taskId, int priority) => Push(new TaskEntry(priority, taskId, userId));

        public void Edit(int taskId, int newPriority)
        {
            _current.TryGetValue(taskId, out var current);
            Push(current with { Priority = newPriority });
        }

        public void Rmv(int taskId) => _current.TryRemove(taskId);

        public int ExecTop()
        {
            while (_heap.TryPop(out var candidate))
            {
                if (_current.TryGetValue(candidate.TaskId, out var authoritative) && authoritative == candidate)
                {
                    _current.TryRemove(candidate.TaskId);
                    return candidate.UserId;
                }
            }

            return LeetCodeAnswer.None;
        }

        private void Push(TaskEntry entry)
        {
            _current.Set(entry.TaskId, entry);
            _heap.Push(entry);
        }
    }
}
