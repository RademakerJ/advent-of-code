using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day10
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var joltages = new List<int>();
            while (!streamReader.EndOfStream)
            {
                joltages.Add(int.Parse(streamReader.ReadLine()));
            }

            joltages.Sort();
            joltages.Insert(0, 0);
            joltages.Add(joltages[joltages.Count - 1] + 3);

            var joltsMultiplied = GetMultipliedDiff(joltages);
            Console.WriteLine($"Jolts multiplied: {joltsMultiplied}"); // Problem 1
            Console.WriteLine($"All possible combinations: {GetNumberOfDistinctPossibilities(joltages)}"); // Problem 2
        }

        // Get all possibilities to connect the adapters.
        static long GetNumberOfDistinctPossibilities(List<int> joltages)
        {
            var dict = new Dictionary<int, long>();
            for (int i = 0; i < joltages.Count; i++)
            {
                dict[joltages[i]] = i == 0 ? 1 : 0;
            }

            foreach (var jolt in joltages)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var sum = jolt + i;
                    if (dict.ContainsKey(sum))
                    {
                        dict[sum] += dict[jolt];
                    }
                }
            }

            return dict[joltages.Max()];
        }

        // Multiplied difference of 1-jolts and 3-jolts
        static int GetMultipliedDiff(List<int> joltages)
        {
            var oneJoltDiffCount = 0;
            var threeJoltDiffCount = 0;

            for (int i = 0; i < joltages.Count - 1; i++)
            {
                var diff = (joltages[i + 1] - joltages[i]);

                oneJoltDiffCount += diff == 1 ? 1 : 0;
                threeJoltDiffCount += diff == 3 ? 1 : 0;
            }

            return oneJoltDiffCount * threeJoltDiffCount;
        }
    }
}
