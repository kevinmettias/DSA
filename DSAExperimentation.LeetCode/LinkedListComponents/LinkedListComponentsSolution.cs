using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.LinkedListComponents;

// LeetCode 817. Linked List Components: how many maximal runs of consecutive list
// nodes have every one of their values present in nums.
//
// Both strategies make the same single walk over SinglyLinkedListNode<int>.Next and
// count the same runs; they differ only in how "is this value in nums" is answered -
// a linear scan of the array per node (O(n*k)) against one membership probe into
// this repo's own Set<int> (O(n+k) overall, including seeding the set).
internal static class LinkedListComponentsSolution
{
    // The textbook answer: no auxiliary structure at all, just Array.IndexOf per
    // node. Deliberately written without this repo's primitives beyond the input
    // list shape itself (ARCHITECTURE.md section 17.5) - it is the arm the composed
    // solution below has to justify itself against.
    public static int NumComponentsByLinearScan(SinglyLinkedListNode<int>? head, int[] nums)
    {
        var runs = default(ComponentRuns);

        for (var node = head; node is not null; node = node.Next)
        {
            runs.Advance(Array.IndexOf(nums, node.Value) >= 0);
        }

        return runs.Count;
    }

    // The composed solution: seed this repo's Set<int> (HashMap-backed, the same
    // composition ContainsDuplicate uses) once, then every per-node membership test
    // is O(1) instead of an O(k) scan.
    public static int NumComponentsBySetMembership(SinglyLinkedListNode<int>? head, int[] nums)
    {
        var present = new Set<int>(nums);
        var runs = default(ComponentRuns);

        for (var node = head; node is not null; node = node.Next)
        {
            runs.Advance(present.Has(node.Value));
        }

        return runs.Count;
    }

    // Counts maximal runs of present values in walk order: a run opens on the first
    // present node after a gap (or at the head) and stays open until a node whose
    // value is absent closes it.
    private struct ComponentRuns
    {
        private bool _inRun;

        public int Count { get; private set; }

        public void Advance(bool present)
        {
            if (present && !_inRun)
            {
                Count++;
            }

            _inRun = present;
        }
    }
}
