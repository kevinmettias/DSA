namespace DSAExperimentation.LeetCode.MaximumTotalBeautyOfTheGardens;

// LeetCode 2234 reports the two beauty rates as separate ints; they only ever travel
// together, and only the split scoring reads them.
internal readonly record struct BeautyWeights(int Full, int Partial);
