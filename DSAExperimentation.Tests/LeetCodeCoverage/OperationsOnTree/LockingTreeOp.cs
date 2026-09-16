using DSAExperimentation.LeetCode.OperationsOnTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OperationsOnTree;

// One call in an OperationsOnTree script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples in each of the two test classes) reads like the LeetCode call sequence it
// replays.
//
// Its own file rather than a second type beside the tests: both
// OperationsOnTreeTests (the lock/unlock/upgrade script) and
// OperationsOnTreeLockedDescendantsTests (the LockedDescendantsOf setups) name it in
// a public MemberData source, so it is not a private detail of either one.
public readonly record struct LockingTreeOp(LockingTreeOp.OpKind kind, int num, int user)
{
    public static LockingTreeOp Lock(int num, int user) => new(OpKind.Lock, num, user);

    public static LockingTreeOp Unlock(int num, int user) => new(OpKind.Unlock, num, user);

    public static LockingTreeOp Upgrade(int num, int user) => new(OpKind.Upgrade, num, user);

    // Internal, not public: only this same assembly's test methods ever call TryApply,
    // and LockingTree itself is internal to the solution tier.
    internal bool TryApply(LockingTree tree) => kind switch
    {
        OpKind.Lock => tree.Lock(num, user),
        OpKind.Unlock => tree.Unlock(num, user),
        _ => tree.Upgrade(num, user),
    };

    public enum OpKind
    {
        Lock,
        Unlock,
        Upgrade,
    }
}
