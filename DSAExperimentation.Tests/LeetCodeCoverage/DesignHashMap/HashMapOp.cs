using DSAExperimentation.LeetCode.DesignHashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashMap;

// One call in a MyHashMap script: which method to invoke and with what
// key/value. Pure dispatch, built via the named factories below so a script
// (like Examples in DesignHashMapTests) reads like the LeetCode call sequence it
// replays.
public readonly record struct HashMapOp(HashMapOp.OpKind kind, int key, int value)
{
    public static HashMapOp Put(int key, int value) => new(OpKind.Put, key, value);

    public static HashMapOp Get(int key) => new(OpKind.Get, key, 0);

    public static HashMapOp Remove(int key) => new(OpKind.Remove, key, 0);

    // null for the two void calls, the looked-up value (or -1) for Get - so a
    // script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IMyHashMap is internal to
    // DesignHashMapSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(DesignHashMapSolution.IMyHashMap map)
    {
        switch (kind)
        {
            case OpKind.Put:
                map.Put(key, value);
                return null;
            case OpKind.Remove:
                map.Remove(key);
                return null;
            default:
                return map.Get(key);
        }
    }

    public enum OpKind
    {
        Put,
        Get,
        Remove,
    }
}
