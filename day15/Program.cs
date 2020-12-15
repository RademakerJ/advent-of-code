using System;
using System.Linq;
using System.Collections.Generic;

namespace day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = new List<int>() { 0, 3, 1, 6, 7, 5 };
            var lastIndices = new Dictionary<int, int>() { { 0, 0 }, { 3, 1 }, { 1, 2 }, { 6, 3 }, { 7, 4 }, { 5, 5 } };

            while (numbers.Count < 2020)
            {
                var lastNumber = numbers.Last();
                var currentIndex = numbers.Count - 1;

                if (!lastIndices.ContainsKey(lastNumber))
                {
                    numbers.Add(0);
                    lastIndices[lastNumber] = currentIndex;
                    continue;
                }
                numbers.Add(currentIndex - lastIndices[lastNumber]);
                lastIndices[lastNumber] = currentIndex;
            }

            Console.WriteLine($"Solution: {numbers.Last()}"); // Problem 1 & 2 (number count = 2020 & number count = 30000000)
        }
    }
}
