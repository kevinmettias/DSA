namespace DSAExperimentation.LeetCode.MinimumIncompatibility;

// LC 1681's one decision, named: which structure answers "has this value come round
// again" while one candidate group's members are walked. Both strategies run the
// identical bitmask recursion and differ only in how that single group is checked for
// a repeated value, so this interface is where the check's contract gets written down,
// which the bare `Func<Func<int, bool>>` the plan used to carry had nowhere to put.
//
// The set itself stays a plain HashSet<int> or Set<int> rather than becoming an object
// of its own: the arms differ in the SET they hand back, and giving each arm a
// stateless singleton over a scope type it does not have to name is what keeps both
// arms' group walks one method.
internal interface INoveltySet<TSeen>
{
    // The empty novelty set one candidate group's walk starts from. Freshness per group
    // is the contract, not a shortcut: a group abandoned halfway through must not leave
    // its values behind for the next candidate to trip over, which is exactly why the
    // plan carries this factory rather than one set.
    TSeen Fresh();

    // Admits `value` into that group's novelty set and answers whether this was its
    // first sighting there. A false is a repeated value, and a group holding one is
    // illegal - so a false is the whole of what makes the candidate group rejectable.
    bool Admit(TSeen seen, int value);
}
