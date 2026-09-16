namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// The arrival rule a dynamic-cost grid search relaxes by, declared the way LC 1237's hidden
// CustomFunction is - a named operation rather than a raw callable - for the same reason:
// the contract an implementation is allowed to lean on needs somewhere to be written, and a
// bare Func<int, int, int> parameter has nowhere to put it.
//
// Both grid problems hand the shared relaxation one of these and nothing else, which is the
// one place they differ: LC 3341 waits where it stands until the room's moveTime allows the
// step, LC 2577 has to land on the cell's own parity after bouncing with a predecessor.
internal interface IArrivalRule
{
    // Earliest second a move may land on a cell whose own earliest entry is `requiredTime`,
    // stepping from `currentTime`. Never earlier than `currentTime + 1`, since the step
    // itself always costs that one second, and pure - the same pair always yields the same
    // arrival - which is what lets the relaxation probe a cell again as its best arrival
    // improves.
    int Arrive(int currentTime, int requiredTime);
}
