namespace DSAExperimentation.Benchmarks.Fixtures;

// The target combination the LC 752 scenario LockWorkloads builds its deadends around.
// Separate from LockWorkloads because it is the scenario's value, which a benchmark has
// to name to drive the same case, not part of how the deadend set is chosen - and
// BuildDeadends has to leave it out of that set for the benchmark's target to stay
// reachable, so the two are one setting stated once.
internal static class LockScenario
{
    // Farthest possible combination from "0000": each wheel needs 5 turns (the max
    // over +1/-1 mod 10 arithmetic), forcing the full 20-turn BFS radius.
    public const string FarthestTarget = "5555";
}
