namespace DSAExperimentation.LeetCode.Solutions.QueueReconstructionByHeight;

// LC 406's promise about its input: the people describe a queue that can be reconstructed, each with as
// many people at least as tall in front of them as their k says. LeetCode's judge never poses an input that
// breaks it. Both strategies insert each person at their k, past the end of the queue so far when no queue
// fits. Generated cases do not always keep the promise, so a harness that runs them asks this first. It
// builds the one queue that could fit, tallest first and fewest ahead first, then counts each person's
// taller-or-equal people ahead.
internal static class QueueReconstructionByHeightPromise
{
    public static bool IsKeptBy(int[][] people)
    {
        var queue = new List<int[]>();

        foreach (var person in people.OrderByDescending(person => person[0]).ThenBy(person => person[1]))
        {
            if (person[1] > queue.Count)
            {
                return false;
            }

            queue.Insert(person[1], person);
        }

        return queue.Select((person, position) => queue.Take(position).Count(ahead => ahead[0] >= person[0]) == person[1]).All(isCounted => isCounted);
    }
}
