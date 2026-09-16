using DSAExperimentation.LeetCode.DesignHashSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashSet;

// One call in a MyHashSet script: which method to invoke and with what key.
// Pure dispatch, built via the named factories below so a script (like Examples
// in DesignHashSetTests) reads like the LeetCode call sequence it replays.
public readonly record struct HashSetOp(HashSetOp.OpKind kind, int key)
{
    public static HashSetOp Add(int key) => new(OpKind.Add, key);

    public static HashSetOp Remove(int key) => new(OpKind.Remove, key);

    public static HashSetOp Contains(int key) => new(OpKind.Contains, key);

    // null for the two void calls, the membership result for Contains - so a
    // script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IMyHashSet is internal to
    // DesignHashSetSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal bool? Apply(DesignHashSetSolution.IMyHashSet set)
    {
        switch (kind)
        {
            case OpKind.Add:
                set.Add(key);
                return null;
            case OpKind.Remove:
                set.Remove(key);
                return null;
            default:
                return set.Contains(key);
        }
    }

    public enum OpKind
    {
        Add,
        Remove,
        Contains,
    }
}
