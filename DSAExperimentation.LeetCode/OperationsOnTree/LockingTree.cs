namespace DSAExperimentation.LeetCode.OperationsOnTree;

// LC 1993's own object: a rooted tree over dense ids where every node additionally
// carries the id of the user currently holding its lock (0 = unlocked, since
// LeetCode's user ids start at 1). A witness meaningful only to this problem, so it
// lives beside the solution rather than in Domain/ (§17.3).
//
// Lock, Unlock and the ancestor check are the same for every strategy - they only
// ever read the parent array - so they live here once instead of being written out
// per arm. The one step the strategies genuinely differ in is LockedDescendantsOf,
// which stays abstract: Upgrade is defined in terms of it, and it is also the step
// the benchmark measures, which is why it is a public member rather than an
// implementation detail hidden inside Upgrade.
//
// Deliberately not IEnumerable, so it can be handed straight to a strategy's
// hoisted overload without ever being ambiguous with a LeetCode-shaped one (§17.4).
internal abstract class LockingTree
{
    // LeetCode's user ids are >= 1, so 0 doubles as "no user holds this node".
    private const int NoUser = 0;

    private readonly int[] _parent;
    private readonly int[] _lockedBy;

    protected LockingTree(int[] parent)
    {
        _parent = [.. parent];
        _lockedBy = new int[parent.Length];
    }

    public int NodeCount => _parent.Length;

    // Locks num for user, unless somebody already holds it.
    public bool Lock(int num, int user)
    {
        if (_lockedBy[num] != NoUser)
        {
            return false;
        }

        _lockedBy[num] = user;
        return true;
    }

    // Unlocks num, but only for the user actually holding it.
    public bool Unlock(int num, int user)
    {
        if (_lockedBy[num] != user)
        {
            return false;
        }

        _lockedBy[num] = NoUser;
        return true;
    }

    // Locks num for user and releases every locked node beneath it - legal only
    // when num is itself unlocked, no ancestor of num is locked, and at least one
    // descendant is.
    public bool Upgrade(int num, int user)
    {
        if (IsLocked(num) || HasLockedAncestor(num))
        {
            return false;
        }

        var lockedDescendants = LockedDescendantsOf(num);

        if (lockedDescendants.Length == 0)
        {
            return false;
        }

        foreach (var descendant in lockedDescendants)
        {
            _lockedBy[descendant] = NoUser;
        }

        _lockedBy[num] = user;
        return true;
    }

    // The locked nodes strictly beneath num, in ascending id order. The strategy
    // axis of this problem.
    public abstract int[] LockedDescendantsOf(int num);

    protected bool IsLocked(int num) => _lockedBy[num] != NoUser;

    // A negative entry marks the root, which is where every ancestor walk stops -
    // the same parent-array encoding DataStructures' ParentArrayTree materializes.
    protected int ParentOf(int num) => _parent[num];

    private bool HasLockedAncestor(int num)
    {
        for (var ancestor = ParentOf(num); ancestor >= 0; ancestor = ParentOf(ancestor))
        {
            if (IsLocked(ancestor))
            {
                return true;
            }
        }

        return false;
    }
}
