using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BoatsToSavePeople;

// LeetCode 881. Boats to Save People: sort weights ascending with this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence (AssignCookiesTests'
// shape), then a single greedy two-pointer pass pairing the lightest remaining
// person with the heaviest whenever they fit together - the heaviest person always
// needs a boat, so pairing them with anyone light enough is never worse than
// sending them alone.
public sealed partial class BoatsToSavePeopleTests
{
    [Fact]
    public void NumRescueBoats_LeetCodeExampleOne_PairsBothPeople()
    {
        int[] people = [1, 2];

        var boats = NumRescueBoats(people, limit: 3);

        Assert.Equal(1, boats);
    }

    [Fact]
    public void NumRescueBoats_LeetCodeExampleTwo_ReturnsThreeBoats()
    {
        int[] people = [3, 2, 2, 1];

        var boats = NumRescueBoats(people, limit: 3);

        Assert.Equal(3, boats);
    }

    [Fact]
    public void NumRescueBoats_LeetCodeExampleThree_NoOneFitsTogether()
    {
        int[] people = [3, 5, 3, 4];

        var boats = NumRescueBoats(people, limit: 5);

        Assert.Equal(4, boats);
    }

    private static int NumRescueBoats(int[] people, int limit)
    {
        var sorted = people.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var light = 0;
        var heavy = sorted.Length - 1;
        var boats = 0;

        while (light <= heavy)
        {
            if (sorted[light] + sorted[heavy] <= limit)
            {
                light++;
            }

            heavy--;
            boats++;
        }

        return boats;
    }
}
